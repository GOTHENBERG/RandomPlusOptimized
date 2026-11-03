using System;
using System.IO;
using Verse;

namespace RandomPlus
{
	// Token: 0x0200001A RID: 26
	public class SaveLoader
	{
		// Token: 0x060000D4 RID: 212 RVA: 0x00007B12 File Offset: 0x00005D12
		public static string GetFilePath()
		{
			return Path.Combine(Path.GetDirectoryName(GenFilePaths.ModsConfigFilePath), SaveLoader.filename);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007B28 File Offset: 0x00005D28
		public static void SaveOverwrite(int index, PawnFilter pawnFilter)
		{
			if (index >= 0)
			{
				RandomSettings.pawnFilterList[index] = pawnFilter;
			}
			SaveLoader.SaveAll();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007B3F File Offset: 0x00005D3F
		public static void Save(PawnFilter pawnFilter)
		{
			if (!RandomSettings.pawnFilterList.Contains(pawnFilter))
			{
				RandomSettings.pawnFilterList.Add(pawnFilter);
			}
			SaveLoader.SaveAll();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007B60 File Offset: 0x00005D60
		public static void SaveAll()
		{
			try
			{
				Scribe.saver.InitSaving(SaveLoader.GetFilePath(), "RandomPlus");
				Scribe_Collections.Look<PawnFilter>(ref RandomSettings.pawnFilterList, "list", LookMode.Deep, null);
			}
			catch (Exception ex)
			{
				Log.Error("Failed to save file");
				throw ex;
			}
			finally
			{
				Scribe.saver.FinalizeSaving();
				Scribe.mode = LoadSaveMode.Inactive;
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007BCC File Offset: 0x00005DCC
		public static void Load(PawnFilter pawnFilter)
		{
			RandomSettings.PawnFilter = pawnFilter;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007BD4 File Offset: 0x00005DD4
		public static void LoadAll()
		{
			string filePath = SaveLoader.GetFilePath();
			if (!File.Exists(filePath))
			{
				return;
			}
			try
			{
				Scribe.loader.InitLoading(SaveLoader.GetFilePath());
				Scribe_Collections.Look<PawnFilter>(ref RandomSettings.pawnFilterList, "list", LookMode.Deep, Array.Empty<object>());
			}
			catch (Exception)
			{
				Log.Error("Failed to load file");
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			finally
			{
				Scribe.loader.FinalizeLoading();
				Scribe.mode = LoadSaveMode.Inactive;
				if (ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
				{
					foreach (PawnFilter pawnFilter in RandomSettings.pawnFilterList)
					{
						pawnFilter.RerollAlgorithm = PawnFilter.RerollAlgorithmOptions.Normal;
					}
				}
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007CAC File Offset: 0x00005EAC
		public static void Delete(PawnFilter pawnFilter)
		{
			RandomSettings.pawnFilterList.Remove(pawnFilter);
			SaveLoader.SaveAll();
		}

		// Token: 0x040000EE RID: 238
		public static readonly string filename = "RandomPlus.xml";
	}
}
