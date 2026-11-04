using Verse;
using RimWorld;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse.AI;

namespace RandomPlus
{
    public class RandomSettings
    {
        // Reflection MethodInfos (kept for fallback)
        private static MethodInfo randomAgeMethodInfo;
        private static MethodInfo randomTraitMethodInfo;
        private static MethodInfo randomSkillMethodInfo;
        private static MethodInfo randomHealthMethodInfo;
        private static MethodInfo randomBodyTypeMethodInfo;
        private static MethodInfo randomGeneMethodInfo;
        private static PropertyInfo startingAndOptionalPawnsPropertyInfo;

        // Compiled Delegates for zero-overhead, zero-allocation native invocation
        private static Action<Pawn, PawnGenerationRequest> generateRandomAge;
        private static Action<Pawn, PawnGenerationRequest> generateTraits;
        private static Action<Pawn, PawnGenerationRequest> generateSkills;
        private static Action<Pawn, PawnGenerationRequest> generateInitialHediffs;
        private static Action<Pawn, PawnGenerationRequest> generateBodyType;
        private static Action<Pawn, XenotypeDef, PawnGenerationRequest> generateGenes;
        private static Func<List<Pawn>> getStartingAndOptionalPawns;

        public static int MinSkillRange;
        public static int randomRerollCounter = 0;
        public static List<PawnFilter> pawnFilterList = new List<PawnFilter>();

        private static PawnFilter pawnFilter;
        public static PawnFilter PawnFilter
        {
            get { return pawnFilter; }
            set { pawnFilter = value; }
        }

        public static int RandomRerollCounter()
        {
            return randomRerollCounter;
        }

        public static void Init()
        {
            pawnFilter = new PawnFilter();

            try
            {
                randomAgeMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateRandomAge", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomAgeMethodInfo != null)
                {
                    generateRandomAge = (Action<Pawn, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, PawnGenerationRequest>), randomAgeMethodInfo);
                }

                randomTraitMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateTraits", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomTraitMethodInfo != null)
                {
                    generateTraits = (Action<Pawn, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, PawnGenerationRequest>), randomTraitMethodInfo);
                }

                randomSkillMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateSkills", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomSkillMethodInfo != null)
                {
                    generateSkills = (Action<Pawn, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, PawnGenerationRequest>), randomSkillMethodInfo);
                }

                randomHealthMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateInitialHediffs", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomHealthMethodInfo != null)
                {
                    generateInitialHediffs = (Action<Pawn, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, PawnGenerationRequest>), randomHealthMethodInfo);
                }

                randomBodyTypeMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateBodyType", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomBodyTypeMethodInfo != null)
                {
                    generateBodyType = (Action<Pawn, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, PawnGenerationRequest>), randomBodyTypeMethodInfo);
                }

                randomGeneMethodInfo = typeof(PawnGenerator)
                    .GetMethod("GenerateGenes", BindingFlags.NonPublic | BindingFlags.Static);
                if (randomGeneMethodInfo != null)
                {
                    generateGenes = (Action<Pawn, XenotypeDef, PawnGenerationRequest>)Delegate.CreateDelegate(
                        typeof(Action<Pawn, XenotypeDef, PawnGenerationRequest>), randomGeneMethodInfo);
                }

                startingAndOptionalPawnsPropertyInfo = typeof(StartingPawnUtility)
                    .GetProperty("StartingAndOptionalPawns", BindingFlags.NonPublic | BindingFlags.Static);
                if (startingAndOptionalPawnsPropertyInfo != null && startingAndOptionalPawnsPropertyInfo.GetGetMethod(true) != null)
                {
                    getStartingAndOptionalPawns = (Func<List<Pawn>>)Delegate.CreateDelegate(
                        typeof(Func<List<Pawn>>), startingAndOptionalPawnsPropertyInfo.GetGetMethod(true));
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"RandomPlus: One or more compiled delegates failed to bind; falling back to reflection: {ex.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateAge(Pawn pawn, PawnGenerationRequest request)
        {
            if (generateRandomAge != null)
                generateRandomAge(pawn, request);
            else if (randomAgeMethodInfo != null)
                randomAgeMethodInfo.Invoke(null, new object[] { pawn, request });
            else
            {
                pawn.ageTracker.AgeBiologicalTicks = (long)(Rand.Range(16, 65) * 3600000L);
                pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateTraits(Pawn pawn, PawnGenerationRequest request)
        {
            if (generateTraits != null)
                generateTraits(pawn, request);
            else
                randomTraitMethodInfo?.Invoke(null, new object[] { pawn, request });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateSkills(Pawn pawn, PawnGenerationRequest request)
        {
            if (generateSkills != null)
                generateSkills(pawn, request);
            else
                randomSkillMethodInfo?.Invoke(null, new object[] { pawn, request });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateHealth(Pawn pawn, PawnGenerationRequest request)
        {
            if (generateInitialHediffs != null)
                generateInitialHediffs(pawn, request);
            else
                randomHealthMethodInfo?.Invoke(null, new object[] { pawn, request });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateBodyType(Pawn pawn, PawnGenerationRequest request)
        {
            if (generateBodyType != null)
                generateBodyType(pawn, request);
            else
                randomBodyTypeMethodInfo?.Invoke(null, new object[] { pawn, request });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CallGenerateGenes(Pawn pawn, XenotypeDef xenotype, PawnGenerationRequest request)
        {
            if (generateGenes != null)
                generateGenes(pawn, xenotype, request);
            else
                randomGeneMethodInfo?.Invoke(null, new object[] { pawn, xenotype, request });
        }

        public static void ResetRerollCounter()
        {
            randomRerollCounter = 0;
        }

        public static void Reroll(int pawnIndex)
        {
            List<Pawn> pawnList = getStartingAndOptionalPawns != null 
                ? getStartingAndOptionalPawns() 
                : (List<Pawn>)startingAndOptionalPawnsPropertyInfo.GetValue(null);

            Pawn pawn = pawnList[pawnIndex];

            SpouseRelationUtility.Notify_PawnRegenerated(pawn);
            pawn = StartingPawnUtility.RandomizeInPlace(pawn);

            randomRerollCounter++;

            if (CheckPawnIsSatisfied(pawn))
                return;

            // Normal reroll mode (or choose wanderers dialog)
            if (PawnFilter.RerollAlgorithm == PawnFilter.RerollAlgorithmOptions.Normal ||
                Find.WindowStack.currentlyDrawnWindow is Dialog_ChooseNewWanderers)
            {
                while (randomRerollCounter < PawnFilter.RerollLimit)
                {
                    if (CheckPawnIsSatisfied(pawn))
                        break;

                    SpouseRelationUtility.Notify_PawnRegenerated(pawn);
                    pawn = StartingPawnUtility.RandomizeInPlace(pawn);

                    randomRerollCounter++;
                }
                return;
            }

            // Fast Reroll Mode
            int index = StartingPawnUtility.PawnIndex(pawn);
            PawnGenerationRequest request = StartingPawnUtility.GetGenerationRequest(index);
            request.ValidateAndFix();

            Faction faction1;
            Faction faction2 = request.Faction ?? 
                (!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out faction1, false, true) 
                    ? Faction.OfAncients : faction1);
            
            XenotypeDef xenotype = ModsConfig.BiotechActive ? PawnGenerator.GetXenotypeForGeneratedPawn(request) : null;

            while (randomRerollCounter < PawnFilter.RerollLimit)
            {
                try
                {
                    randomRerollCounter++;

                    // Fast-Reject: Gender check if specified
                    if (pawnFilter.Gender != Gender.None && pawn.gender != Gender.None)
                    {
                        pawn.gender = pawnFilter.Gender;
                    }

                    // Age generation (Zero-allocation via compiled delegate)
                    pawn.ageTracker = new Pawn_AgeTracker(pawn);
                    CallGenerateAge(pawn, request);
                    
                    if (!CheckAgeIsSatisfied(pawn))
                        continue;

                    pawn.story.traits = new TraitSet(pawn);
                    pawn.skills = new Pawn_SkillTracker(pawn);

                    // Backstory generation (required for backstory trait rules & skill bonuses)
                    PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, faction2.def, request, xenotype);
                    
                    // Generate Traits & Skills (Zero-allocation via compiled delegates)
                    CallGenerateTraits(pawn, request);
                    CallGenerateSkills(pawn, request);
                    
                    // Fail-Fast: Skills & Traits check (95%+ rejection rate before health)
                    if (!CheckTraitsIsSatisfied(pawn) || !CheckSkillsIsSatisfied(pawn))
                        continue;

                    // Health generation loop: Up to 5 attempts to roll a non-downed/non-dead pawn
                    bool healthGenSuccess = false;
                    for (int i = 0; i < 5 && !healthGenSuccess; i++)
                    {
                        pawn.health.Reset();
                        try
                        {
                            Find.Scenario.Notify_NewPawnGenerating(pawn, request.Context);
                            CallGenerateHealth(pawn, request);
                            
                            if (!(pawn.Dead || pawn.Destroyed || pawn.Downed))
                            {
                                healthGenSuccess = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"RandomPlus: Health generation failed on attempt {i}: {ex.Message}");
                        }
                    }
                    
                    if (!healthGenSuccess || !CheckHealthIsSatisfied(pawn))
                        continue;

                    pawn.workSettings?.EnableAndInitialize();
                    if (!CheckWorkIsSatisfied(pawn))
                        continue;

                    // Custom scenario e.g. forced traits
                    Find.Scenario.Notify_PawnGenerated(pawn, request.Context, true);
                    if (!CheckPawnIsSatisfied(pawn))
                        continue;

                    // Genes (Biotech)
                    if (ModsConfig.BiotechActive)
                    {
                        pawn.genes = new Pawn_GeneTracker(pawn);
                        CallGenerateGenes(pawn, xenotype, request);
                    }
                    
                    CallGenerateBodyType(pawn, request);
                    GeneratePawnStyle(pawn);

                    // OPTIMIZATION: Redress pawn apparel/weapons ONLY ONCE at the end
                    // when all filters have successfully matched!
                    PawnGenerator.RedressPawn(pawn, request);

                    return;
                }
                catch (Exception ex)
                {
                    Log.Warning($"RandomPlus: Error during pawn generation (attempt {randomRerollCounter}): {ex.Message}");
                    try
                    {
                        Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);
                        SpouseRelationUtility.Notify_PawnRegenerated(pawn);
                        pawn = StartingPawnUtility.RandomizeInPlace(pawn);
                    }
                    catch (Exception ex2)
                    {
                        Log.Error($"RandomPlus: Critical error in pawn cleanup: {ex2.Message}");
                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPawnIsSatisfied(Pawn pawn)
        {
            if (RandomRerollCounter() >= PawnFilter.RerollLimit)
            {
                return true;
            }

            // Fail-Fast order: cheapest checks first
            if (!CheckGenderIsSatisfied(pawn))
                return false;
            if (!CheckAgeIsSatisfied(pawn))
                return false;
            if (!CheckTraitsIsSatisfied(pawn))
                return false;
            if (!CheckSkillsIsSatisfied(pawn))
                return false;
            if (!CheckHealthIsSatisfied(pawn))
                return false;
            if (!CheckWorkIsSatisfied(pawn))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckAgeIsSatisfied(Pawn pawn)
        {
            if (pawnFilter.ageRange.min != PawnFilter.MinAgeDefault ||
                pawnFilter.ageRange.max != PawnFilter.MaxAgeDefault)
            {
                int bioAge = pawn.ageTracker.AgeBiologicalYears;
                if (pawnFilter.ageRange.min > bioAge ||
                    (pawnFilter.ageRange.max != PawnFilter.MaxAgeDefault && pawnFilter.ageRange.max < bioAge))
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenderIsSatisfied(Pawn pawn)
        {
            if (pawnFilter.Gender != Gender.None && pawn.gender != Gender.None)
            {
                if (pawnFilter.Gender != pawn.gender)
                    return false;
            }
            return true;
        }

        public static bool CheckSkillsIsSatisfied(Pawn pawn)
        {
            List<SkillRecord> skillList = pawn.skills.skills;
            int skillListCount = skillList.Count;

            // 1. Check individual skill requirements (Zero-allocation)
            var filterSkills = pawnFilter.Skills;
            int filterSkillsCount = filterSkills.Count;
            for (int f = 0; f < filterSkillsCount; f++)
            {
                var skillFilter = filterSkills[f];
                if (skillFilter.Passion != Passion.None || skillFilter.MinValue > 0)
                {
                    SkillRecord skillRecord = pawn.skills.GetSkill(skillFilter.SkillDef);
                    if (skillRecord != null)
                    {
                        if (skillRecord.passion < skillFilter.Passion || skillRecord.Level < skillFilter.MinValue)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Log.Error("RandomPlus: Skill record not found - this shouldn't happen!");
                    }
                }
            }

            // 2. Check total passion range (Zero-allocation loop)
            if (pawnFilter.passionRange.min > PawnFilter.PassionMinDefault ||
                pawnFilter.passionRange.max < PawnFilter.PassionMaxDefault)
            {
                int totalPassions = 0;
                for (int i = 0; i < skillListCount; i++)
                {
                    if (skillList[i].passion > Passion.None)
                        totalPassions++;
                }

                if (totalPassions < pawnFilter.passionRange.min ||
                    totalPassions > pawnFilter.passionRange.max)
                {
                    return false;
                }
            }

            // 3. Check total skill range (Zero-allocation loop)
            if (pawnFilter.skillRange.min != PawnFilter.SkillMinDefault ||
                pawnFilter.skillRange.max != PawnFilter.SkillMaxDefault)
            {
                int skillTotalCounter = 0;
                for (int i = 0; i < skillListCount; i++)
                {
                    var skill = skillList[i];
                    if (PawnFilter.countOnlyHighestAttack)
                    {
                        if (i == 0) // Shooting[i=0] Melee[i=1]
                        {
                            var meleeSkill = skillList[1];
                            skillTotalCounter += meleeSkill.Level > skill.Level ? meleeSkill.Level : skill.Level;
                            i = 1; // skip next loop (melee)
                            continue;
                        }
                    }
                    if (PawnFilter.countOnlyPassion)
                    {
                        if (skill.passion > Passion.None)
                            skillTotalCounter += skill.Level;
                    }
                    else
                    {
                        skillTotalCounter += skill.Level;
                    }
                }
                
                if (pawnFilter.skillRange.min > skillTotalCounter ||
                    pawnFilter.skillRange.max < skillTotalCounter)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckTraitsIsSatisfied(Pawn pawn)
        {
            if (Page_RandomEditor.MOD_WELL_MET)
                return true;

            var traitFilterList = pawnFilter.Traits;
            int traitFilterCount = traitFilterList.Count;

            // 1. Check Required and Excluded traits (Zero-allocation)
            for (int i = 0; i < traitFilterCount; i++)
            {
                var traitContainer = traitFilterList[i];
                if (traitContainer.traitFilter == TraitContainer.TraitFilterType.Required)
                {
                    if (!HasTrait(pawn, traitContainer.trait))
                        return false;
                }
                else if (traitContainer.traitFilter == TraitContainer.TraitFilterType.Excluded)
                {
                    if (HasTrait(pawn, traitContainer.trait))
                        return false;
                }
            }

            // 2. Check Optional trait pool (Zero-allocation)
            if (pawnFilter.RequiredTraitsInPool > 0 &&
                pawnFilter.RequiredTraitsInPool <= traitFilterCount)
            {
                int pawnHasTraitCounter = 0;
                for (int i = 0; i < traitFilterCount; i++)
                {
                    var traitContainer = traitFilterList[i];
                    if (traitContainer.traitFilter == TraitContainer.TraitFilterType.Optional)
                    {
                        if (HasTrait(pawn, traitContainer.trait))
                        {
                            pawnHasTraitCounter++;
                            if (pawnFilter.RequiredTraitsInPool == pawnHasTraitCounter)
                                break;
                        }
                    }
                }
                if (pawnHasTraitCounter < pawnFilter.RequiredTraitsInPool)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGeneAffectedHealth(Hediff hediff)
        {
            if (!ModsConfig.BiotechActive)
                return false;

            if (hediff is Hediff_ChemicalDependency chemicalDependency && chemicalDependency.LinkedGene != null)
                return true;

            return false;
        }

        public static bool CheckHealthIsSatisfied(Pawn pawn)
        {
            var hediffs = pawn.health.hediffSet.hediffs;
            int count = hediffs.Count;

            switch (pawnFilter.FilterHealthCondition)
            {
                case PawnFilter.HealthOptions.AllowAll:
                    break;

                case PawnFilter.HealthOptions.OnlyStartCondition:
                    for (int i = 0; i < count; i++)
                    {
                        var hediff = hediffs[i];
                        string defName = hediff.def.defName;
                        if (defName != "CryptosleepSickness" && defName != "Malnutrition" && !IsGeneAffectedHealth(hediff))
                            return false;
                    }
                    break;

                case PawnFilter.HealthOptions.NoPain:
                    for (int i = 0; i < count; i++)
                    {
                        var hediff = hediffs[i];
                        if (hediff.PainOffset > 0f && !IsGeneAffectedHealth(hediff))
                            return false;
                    }
                    break;

                case PawnFilter.HealthOptions.NoAddiction:
                    for (int i = 0; i < count; i++)
                    {
                        var hediff = hediffs[i];
                        if (hediff is Hediff_Addiction && !IsGeneAffectedHealth(hediff))
                            return false;
                    }
                    break;

                case PawnFilter.HealthOptions.AllowNone:
                    if (ModsConfig.BiotechActive)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (!IsGeneAffectedHealth(hediffs[i]))
                                return false;
                        }
                    }
                    else
                    {
                        if (count > 0)
                            return false;
                    }
                    break;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckWorkIsSatisfied(Pawn pawn)
        {
            switch (pawnFilter.FilterIncapable)
            {
                case PawnFilter.IncapableOptions.AllowAll:
                    break;
                case PawnFilter.IncapableOptions.NoDumbLabor:
                    if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & WorkTags.ManualDumb) == WorkTags.ManualDumb)
                        return false;
                    break;
                case PawnFilter.IncapableOptions.AllowNone:
                    if (pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.None)
                        return false;
                    break;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasTrait(Pawn pawn, Trait trait)
        {
            if (trait == null || pawn?.story?.traits?.allTraits == null)
                return false;

            // Zero-allocation: Uses RimWorld's native TraitSet.HasTrait(TraitDef, int degree)
            // which performs direct reference and integer comparisons over allTraits.
            return pawn.story.traits.HasTrait(trait.def, trait.Degree);
        }

        public static void SetGenderFilter(Gender gender)
        {
            pawnFilter.Gender = gender;
        }

        private static float _cacheTraitCommonalityMale;
        private static float _cacheTraitCommonalityFemale;

        private static float GetTotalTraitCommonality(Gender gender)
        {
            if (gender == Gender.Male && _cacheTraitCommonalityMale > 0)
                return _cacheTraitCommonalityMale;
            if (gender == Gender.Female && _cacheTraitCommonalityFemale > 0)
                return _cacheTraitCommonalityFemale;

            float total = 0;
            var defs = DefDatabase<TraitDef>.AllDefsListForReading;
            int count = defs.Count;
            for (int i = 0; i < count; i++)
            {
                total += defs[i].GetGenderSpecificCommonality(gender);
            }

            if (gender == Gender.Male)
                _cacheTraitCommonalityMale = total;
            if (gender == Gender.Female)
                _cacheTraitCommonalityFemale = total;

            return total;
        }

        public static float GetTraitRollChance(TraitDef traitDef, Gender gender = Gender.Male)
        {
            float total = GetTotalTraitCommonality(gender);
            return traitDef.GetGenderSpecificCommonality(gender) * 100 / total;
        }

        public static string GetTraitRollChanceText(TraitDef traitDef)
        {
            string pecentMale = GetTraitRollChance(traitDef, Gender.Male).ToString("0.0");
            string pecentFemale = GetTraitRollChance(traitDef, Gender.Female).ToString("0.0");

            if (traitDef.GetGenderSpecificCommonality(Gender.Male) == traitDef.GetGenderSpecificCommonality(Gender.Female))
                return $"({pecentMale}%)";
            return $"(♂:{pecentMale}%,♀:{pecentFemale}%)";
        }

        public static void GeneratePawnStyle(Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike)
            {
                try
                {
                    pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                    if (pawn.style != null)
                    {
                        pawn.style.beardDef = pawn.gender == Gender.Male ? PawnStyleItemChooser.RandomBeardFor(pawn) : BeardDefOf.NoBeard;
                        if (ModsConfig.IdeologyActive)
                        {
                            pawn.style.FaceTattoo = PawnStyleItemChooser.RandomTattooFor(pawn, TattooType.Face);
                            pawn.style.BodyTattoo = PawnStyleItemChooser.RandomTattooFor(pawn, TattooType.Body);
                        }
                        else
                        {
                            pawn.style.SetupTattoos_NoIdeology();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"RandomPlus: Failed to generate pawn style: {ex.Message}");
                }
            }
        }
    }
}
