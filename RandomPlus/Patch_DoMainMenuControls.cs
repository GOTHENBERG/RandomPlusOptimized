using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls")]
	internal class Patch_DoMainMenuControls
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00005BC8 File Offset: 0x00003DC8
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int num = -1;
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Newobj && list[i + 1].opcode == OpCodes.Stloc_2)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				List<CodeInstruction> list2 = new List<CodeInstruction>();
				list2.Add(new CodeInstruction(OpCodes.Ldloc_2, null));
				MethodInfo method = typeof(Patch_DoMainMenuControls).GetMethod("AddQuickGoToConfigPawnPage", BindingFlags.Static | BindingFlags.Public);
				list2.Add(new CodeInstruction(OpCodes.Call, method));
				list.InsertRange(num + 2, list2);
			}
			return list;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005C75 File Offset: 0x00003E75
		public static void AddQuickGoToConfigPawnPage(List<ListableOption> optList)
		{
			if (Event.current.type == EventType.KeyDown && DefDatabase<KeyBindingDef>.GetNamed("Dev_QuickGoToConfigPawnPage", true).JustPressed)
			{
				Patch_DoMainMenuControls.GoToConfigPawnPage();
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005C9C File Offset: 0x00003E9C
		public static void GoToConfigPawnPage()
		{
			try
			{
				Page_SelectScenario page_SelectScenario = new Page_SelectScenario();
				Find.WindowStack.Add(page_SelectScenario);
				MethodInfo method = typeof(Page_SelectScenario).GetMethod("CanDoNext", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(page_SelectScenario, new object[0]);
				}
				MethodInfo method2 = typeof(Page_SelectScenario).GetMethod("DoNext", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method2 != null)
				{
					method2.Invoke(page_SelectScenario, new object[0]);
				}
				Page_SelectStoryteller page_SelectStoryteller = (Page_SelectStoryteller)page_SelectScenario.next;
				MethodInfo method3 = typeof(Page_SelectStoryteller).GetMethod("CanDoNext", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method3 != null)
				{
					method3.Invoke(page_SelectStoryteller, new object[0]);
				}
				MethodInfo method4 = typeof(Page_SelectStoryteller).GetMethod("DoNext", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method4 != null)
				{
					method4.Invoke(page_SelectStoryteller, new object[0]);
				}
				Page_CreateWorldParams page_CreateWorldParams = (Page_CreateWorldParams)page_SelectStoryteller.next;
				FieldInfo field = typeof(Page_CreateWorldParams).GetField("planetCoverage", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null)
				{
					field.SetValue(page_CreateWorldParams, 0.1f);
				}
				MethodInfo method5 = typeof(Page_CreateWorldParams).GetMethod("CanDoNext", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method5 != null)
				{
					method5.Invoke(page_CreateWorldParams, new object[0]);
				}
				Page_SelectStartingSite page_select_site = (Page_SelectStartingSite)page_CreateWorldParams.next;
				LongEventHandler.QueueLongEvent(delegate
				{
					while (Find.World == null)
					{
						Thread.Sleep(100);
					}
					LongEventHandler.ExecuteWhenFinished(delegate
					{
						try
						{
							Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
						}
						catch
						{
							Find.WorldInterface.SelectedTile = Rand.Range(0, Find.WorldGrid.TilesCount);
						}
						MethodInfo method6 = typeof(Page_SelectStartingSite).GetMethod("CanDoNext", BindingFlags.Instance | BindingFlags.NonPublic);
						if (method6 != null)
						{
							method6.Invoke(page_select_site, new object[0]);
						}
						MethodInfo method7 = typeof(Page_SelectStartingSite).GetMethod("DoNext", BindingFlags.Instance | BindingFlags.NonPublic);
						if (method7 != null)
						{
							method7.Invoke(page_select_site, new object[0]);
						}
						if (ModsConfig.IdeologyActive)
						{
							Page_ChooseIdeoPreset page_ChooseIdeoPreset = (Page_ChooseIdeoPreset)page_select_site.next;
							IEnumerable<IdeoPresetDef> allDefs = DefDatabase<IdeoPresetDef>.AllDefs;
							FieldInfo field2 = typeof(Page_ChooseIdeoPreset).GetField("selectedIdeo", BindingFlags.Instance | BindingFlags.NonPublic);
							if (field2 != null)
							{
								field2.SetValue(page_ChooseIdeoPreset, GenCollection.RandomElement<IdeoPresetDef>(allDefs));
							}
							MethodInfo method8 = typeof(Page_ChooseIdeoPreset).GetMethod("CanDoNext", BindingFlags.Instance | BindingFlags.NonPublic);
							if (method8 != null)
							{
								method8.Invoke(page_ChooseIdeoPreset, new object[0]);
							}
							MethodInfo method9 = typeof(Page_ChooseIdeoPreset).GetMethod("DoNext", BindingFlags.Instance | BindingFlags.NonPublic);
							if (method9 != null)
							{
								method9.Invoke(page_ChooseIdeoPreset, new object[0]);
							}
						}
						Page_RandomEditor page_RandomEditor = new Page_RandomEditor();
						Find.WindowStack.Add(page_RandomEditor);
					});
				}, null, true, null, false, false, null);
			}
			catch (Exception ex)
			{
				Log.Error("RandomPlus: Failed to launch quick config page: " + ex.Message);
			}
		}
	}
}
