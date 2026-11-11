using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
    /// <summary>
    /// Manages non-blocking frame-sliced reroll sessions.
    /// Distributes candidate generation across frames to ensure the RimWorld UI maintains
    /// smooth 60 FPS while displaying live reroll counters, candidate previews, and an interactive Stop button.
    /// </summary>
    public static class SearchSession
    {
        public static bool IsActive { get; private set; } = false;
        public static int SearchingPawnIndex { get; private set; } = -1;
        public static string SampledCandidateInfo { get; private set; } = "";

        private static Pawn currentPawn;
        private static PawnGenerationRequest currentRequest;
        private static Faction biosFaction;
        private static XenotypeDef currentXenotype;
        private static bool canFastRegenerate;
        private static bool suppressGear;
        private static int outerRetries = 0;
        private static float nextSampleTime = 0f;
        private static readonly Stopwatch frameSw = new Stopwatch();

        public static void Start(int pawnIndex)
        {
            if (IsActive)
                return;

            List<Pawn> pawnList = RandomSettings.GetStartingAndOptionalPawns();
            if (pawnList == null || pawnIndex < 0 || pawnIndex >= pawnList.Count)
                return;

            SearchingPawnIndex = pawnIndex;
            currentPawn = pawnList[pawnIndex];
            outerRetries = 0;
            nextSampleTime = 0f;
            SampledCandidateInfo = "";

            RandomSettings.ResetRerollCounter();
            PrepareCandidateContext();

            // Check if initial pawn already satisfies filter
            if (RandomSettings.CheckPawnIsSatisfied(currentPawn) && StartingPawnUtility.WorkTypeRequirementsSatisfied())
            {
                IsActive = false;
                return;
            }

            IsActive = true;
            RandomSettings.IsRerolling = true;
        }

        private static void PrepareCandidateContext()
        {
            int index = StartingPawnUtility.PawnIndex(currentPawn);
            currentRequest = StartingPawnUtility.GetGenerationRequest(index);
            currentRequest.ValidateAndFix();

            Faction faction1;
            biosFaction = currentRequest.Faction ?? 
                (!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out faction1, false, true) 
                    ? Faction.OfAncients : faction1);

            currentXenotype = ModsConfig.BiotechActive ? PawnGenerator.GetXenotypeForGeneratedPawn(currentRequest) : null;
            
            bool isWanderers = Find.WindowStack.currentlyDrawnWindow is Dialog_ChooseNewWanderers;
            canFastRegenerate = RandomSettings.CanRegenerateInPlace(isWanderers) && 
                                (RandomSettings.PawnFilter.RerollAlgorithm == PawnFilter.RerollAlgorithmOptions.Fast);
            
            suppressGear = GenerationCompatibility.GearSuppressionIsSafe();
        }

        public static void PumpStep(double budgetMs = 8.0)
        {
            if (!IsActive)
                return;

            // Safety guard: if character creation window was closed, cancel and settle pawn
            if (Find.WindowStack.currentlyDrawnWindow == null || 
                !(Find.WindowStack.currentlyDrawnWindow is Page_ConfigureStartingPawns || 
                  Find.WindowStack.currentlyDrawnWindow is Dialog_ChooseNewWanderers))
            {
                Cancel();
                return;
            }

            frameSw.Restart();

            while (frameSw.Elapsed.TotalMilliseconds < budgetMs && 
                   RandomSettings.randomRerollCounter < RandomSettings.PawnFilter.RerollLimit)
            {
                bool matched = StepOneCandidate();
                if (matched)
                {
                    if (outerRetries >= 20 || StartingPawnUtility.WorkTypeRequirementsSatisfied())
                    {
                        End();
                        return;
                    }
                    outerRetries++;
                }
            }

            if (RandomSettings.randomRerollCounter >= RandomSettings.PawnFilter.RerollLimit)
            {
                End();
            }
        }

        private static bool StepOneCandidate()
        {
            RandomSettings.randomRerollCounter++;

            // Sampling candidate info for readable UI display every 100ms
            float now = Time.realtimeSinceStartup;
            if (now >= nextSampleTime)
            {
                nextSampleTime = now + 0.10f;
                string name = currentPawn?.LabelShortCap ?? "---";
                int age = currentPawn?.ageTracker?.AgeBiologicalYears ?? 0;
                SampledCandidateInfo = $"{name}, {age}";
            }

            if (!canFastRegenerate)
            {
                // Safe whole-pawn reroll path for third-party mod compatibility
                RandomSettings.SuppressGearGeneration = suppressGear;
                try
                {
                    SpouseRelationUtility.Notify_PawnRegenerated(currentPawn);
                    currentPawn = StartingPawnUtility.RandomizeInPlace(currentPawn);
                }
                finally
                {
                    RandomSettings.SuppressGearGeneration = false;
                }

                return RandomSettings.CheckPawnIsSatisfied(currentPawn);
            }

            // Zero-allocation in-place candidate generation
            try
            {
                if (RandomSettings.PawnFilter.Gender != Gender.None && currentPawn.gender != Gender.None)
                {
                    currentPawn.gender = RandomSettings.PawnFilter.Gender;
                }

                currentPawn.ageTracker = new Pawn_AgeTracker(currentPawn);
                RandomSettings.CallGenerateAge(currentPawn, currentRequest);

                if (!RandomSettings.CheckAgeIsSatisfied(currentPawn))
                    return false;

                currentPawn.story.traits = new TraitSet(currentPawn);
                currentPawn.skills = new Pawn_SkillTracker(currentPawn);

                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(currentPawn, biosFaction.def, currentRequest, currentXenotype);

                RandomSettings.CallGenerateTraits(currentPawn, currentRequest);
                RandomSettings.CallGenerateSkills(currentPawn, currentRequest);

                if (!RandomSettings.CheckTraitsIsSatisfied(currentPawn) || !RandomSettings.CheckSkillsIsSatisfied(currentPawn))
                    return false;

                bool healthOk = false;
                for (int i = 0; i < 5 && !healthOk; i++)
                {
                    currentPawn.health.Reset();
                    try
                    {
                        Find.Scenario.Notify_NewPawnGenerating(currentPawn, currentRequest.Context);
                        RandomSettings.CallGenerateHealth(currentPawn, currentRequest);
                        if (!(currentPawn.Dead || currentPawn.Destroyed || currentPawn.Downed))
                            healthOk = true;
                    }
                    catch { }
                }

                if (!healthOk || !RandomSettings.CheckHealthIsSatisfied(currentPawn))
                    return false;

                currentPawn.workSettings?.EnableAndInitialize();
                if (!RandomSettings.CheckWorkIsSatisfied(currentPawn))
                    return false;

                Find.Scenario.Notify_PawnGenerated(currentPawn, currentRequest.Context, true);
                if (!RandomSettings.CheckPawnIsSatisfied(currentPawn))
                    return false;

                if (ModsConfig.BiotechActive)
                {
                    currentPawn.genes = new Pawn_GeneTracker(currentPawn);
                    RandomSettings.CallGenerateGenes(currentPawn, currentXenotype, currentRequest);
                }

                RandomSettings.CallGenerateBodyType(currentPawn, currentRequest);
                RandomSettings.GeneratePawnStyle(currentPawn);

                return true;
            }
            catch (Exception ex)
            {
                Log.Warning($"RandomPlus: Candidate generation exception: {ex.Message}");
                try
                {
                    Find.WorldPawns.RemoveAndDiscardPawnViaGC(currentPawn);
                    SpouseRelationUtility.Notify_PawnRegenerated(currentPawn);
                    currentPawn = StartingPawnUtility.RandomizeInPlace(currentPawn);
                }
                catch { }
                return false;
            }
        }

        public static void End()
        {
            if (!IsActive) return;
            FinishPawn();
            IsActive = false;
        }

        public static void Cancel()
        {
            if (!IsActive) return;
            FinishPawn();
            IsActive = false;
        }

        private static void FinishPawn()
        {
            RandomSettings.IsRerolling = false;
            RandomSettings.SuppressGearGeneration = false;

            if (currentPawn != null)
            {
                try
                {
                    // Populate family relations once for the final accepted or interrupted pawn
                    RandomSettings.CallGeneratePawnRelations(currentPawn, ref currentRequest);
                    
                    // Generate gear and finalize clothing
                    PawnGenerator.RedressPawn(currentPawn, currentRequest);
                    currentPawn.workSettings?.EnableAndInitialize();

                    // Notify graphics engine and cache to redraw portrait cleanly
                    PortraitsCache.SetDirty(currentPawn);
                    currentPawn.Drawer?.renderer?.SetAllGraphicsDirty();
                }
                catch (Exception ex)
                {
                    Log.Warning($"RandomPlus: FinishPawn error: {ex.Message}");
                }
            }
        }
    }
}
