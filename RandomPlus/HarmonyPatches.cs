using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000011 RID: 17
	[StaticConstructorOnStartup]
	internal class HarmonyPatches
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00005949 File Offset: 0x00003B49
		static HarmonyPatches()
		{
			new Harmony("RandomPlus").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
