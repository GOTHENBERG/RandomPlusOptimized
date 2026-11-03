using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(StartingPawnUtility), "RandomizePawn")]
	internal class Patch_RandomizeMethod
	{
		// Token: 0x06000088 RID: 136 RVA: 0x0000597C File Offset: 0x00003B7C
		[HarmonyPrefix]
		private static bool Prefix(int pawnIndex)
		{
			if (!TutorSystem.AllowAction("RandomizePawn"))
			{
				return false;
			}
			RandomSettings.ResetRerollCounter();
			int num = 0;
			do
			{
				RandomSettings.Reroll(pawnIndex);
				num++;
			}
			while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
			TutorSystem.Notify_Event("RandomizePawn");
			return false;
		}
	}
}
