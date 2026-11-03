using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000014 RID: 20
	[HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
	internal class Patch_RandomEditButton
	{
		// Token: 0x0600008A RID: 138 RVA: 0x000059D4 File Offset: 0x00003BD4
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int num = -1;
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Ldloc_1 && list[i + 1].opcode == OpCodes.Brfalse)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				MethodInfo method = typeof(Patch_RandomEditButton).GetMethod("InjectCustomUI", BindingFlags.Static | BindingFlags.Public);
				list.Insert(num + 2, new CodeInstruction(OpCodes.Call, method));
			}
			return list;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005A64 File Offset: 0x00003C64
		public static void InjectCustomUI()
		{
			float num = 1f;
			Rect rect = new Rect(540f * num, 6f * num, 50f * num, 30f * num);
			if (ModsConfig.IsActive("hahkethomemah.simplepersonalities"))
			{
				rect.x -= 130f * num;
			}
			if (Widgets.ButtonText(rect, Translator.Translate("RandomPlus.FilterButton"), true, false, true, null))
			{
				Page_RandomEditor page_RandomEditor = new Page_RandomEditor();
				Find.WindowStack.Add(page_RandomEditor);
			}
			Rect rect2 = new Rect(640f * num, 4f * num, 200f * num, 30f * num);
			if (ModsConfig.IdeologyActive)
			{
				rect2.y += 40f * num;
			}
			if (ModsConfig.BiotechActive)
			{
				rect2.y += 60f * num;
			}
			if (RandomSettings.PawnFilter == null)
			{
				RandomSettings.Init();
			}
			string text = Translator.Translate("RandomPlus.RerollLabel") + RandomSettings.RandomRerollCounter().ToString() + "/" + RandomSettings.PawnFilter.RerollLimit.ToString();
			Color color = GUI.color;
			if (RandomSettings.RandomRerollCounter() >= RandomSettings.PawnFilter.RerollLimit)
			{
				GUI.color = Color.red;
			}
			Widgets.Label(rect2, text);
			GUI.color = color;
		}
	}
}
