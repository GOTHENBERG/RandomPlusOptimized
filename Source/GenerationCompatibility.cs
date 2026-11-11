using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;

namespace RandomPlus
{
    /// <summary>
    /// Decides how much of RimWorld's pawn generation the search is allowed to take
    /// shortcuts around, by asking Harmony what other third-party mods have patched.
    ///
    /// If foreign mods (e.g., Humanoid Alien Races, Combat Extended, Facial Animation)
    /// hooked generation entry points, RandomPlus safely falls back to standard
    /// whole-pawn generation to ensure 100% third-party mod compatibility.
    /// </summary>
    public static class GenerationCompatibility
    {
        public const string HarmonyId = "RandomPlus";

        public static Func<bool> ForeignPatchesOnGenerationEntryPoints = AnyForeignPatchOnEntryPoints;
        public static Func<bool> GearSuppressionIsSafe = GearGenerationIsUnpatched;

        public static void Install()
        {
            ForeignPatchesOnGenerationEntryPoints = AnyForeignPatchOnEntryPoints;
            GearSuppressionIsSafe = GearGenerationIsUnpatched;
        }

        private static IEnumerable<MethodBase> GenerationEntryPoints()
        {
            foreach (var m in MethodsNamed(typeof(PawnGenerator), "GeneratePawn")) yield return m;
            foreach (var m in MethodsNamed(typeof(PawnGenerator), "GenerateNewPawnInternal")) yield return m;
            foreach (var m in MethodsNamed(typeof(StartingPawnUtility), "NewGeneratedStartingPawn")) yield return m;
        }

        private static IEnumerable<MethodBase> GearGenerationMethods()
        {
            foreach (var m in MethodsNamed(typeof(PawnGenerator), "GenerateGearFor")) yield return m;
            foreach (var m in MethodsNamed(AccessTools.TypeByName("RimWorld.PawnApparelGenerator"), "GenerateStartingApparelFor")) yield return m;
            foreach (var m in MethodsNamed(AccessTools.TypeByName("RimWorld.PawnWeaponGenerator"), "TryGenerateWeaponFor")) yield return m;
            foreach (var m in MethodsNamed(AccessTools.TypeByName("RimWorld.PawnInventoryGenerator"), "GenerateInventoryFor")) yield return m;
        }

        private static IEnumerable<MethodBase> MethodsNamed(Type type, string name)
        {
            if (type == null) yield break;

            var methods = type.GetMethods(AccessTools.all);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name == name)
                    yield return methods[i];
            }
        }

        public static bool AnyForeignPatchOnEntryPoints() => AnyForeignPatch(GenerationEntryPoints());

        public static bool GearGenerationIsUnpatched() => !AnyForeignPatch(GearGenerationMethods());

        private static bool AnyForeignPatch(IEnumerable<MethodBase> methods)
        {
            try
            {
                foreach (var method in methods)
                {
                    var patches = Harmony.GetPatchInfo(method);
                    if (patches == null)
                        continue;

                    foreach (var owner in patches.Owners)
                    {
                        if (owner != HarmonyId)
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Warning($"RandomPlus: Could not inspect Harmony patches, taking safe path: {ex.Message}");
                return true;
            }
        }
    }

    /// <summary>
    /// Skips generating apparel, weapons and inventory for candidates the search
    /// is going to test and discard. Safe unless another mod hooked gear generation itself.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), "GenerateGearFor")]
    static class Patch_SuppressGearWhileSearching
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return !RandomSettings.SuppressGearGeneration;
        }
    }
}
