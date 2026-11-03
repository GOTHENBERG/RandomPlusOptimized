using System;
using HarmonyLib;
using RimWorld;

namespace RandomPlus
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "PreOpen")]
	internal class Patch_InitRandomSettings
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00005967 File Offset: 0x00003B67
		[HarmonyPostfix]
		private static void Postfix()
		{
			RandomSettings.Init();
			RandomSettings.ResetRerollCounter();
		}
	}
}
