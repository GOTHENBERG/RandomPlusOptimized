using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x0200000C RID: 12
	public class SaveLoadDialog : Window
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00004AFC File Offset: 0x00002CFC
		public SaveLoadDialog()
			: base(null)
		{
			this.closeOnCancel = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
			this.forcePause = true;
			SaveLoader.LoadAll();
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00004B4A File Offset: 0x00002D4A
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(600f, 400f);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004B5B File Offset: 0x00002D5B
		public override void PostClose()
		{
			GUI.FocusControl(null);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004B64 File Offset: 0x00002D64
		public override void DoWindowContents(Rect inRect)
		{
			int num = 16;
			int num2 = 20;
			Vector2 vector = new Vector2(150f, 30f);
			Vector2 vector2 = new Vector2(inRect.width - vector.x - (float)num - (float)num2, 36f);
			List<PawnFilter> list = RandomSettings.pawnFilterList.ToList<PawnFilter>();
			float num3 = (float)list.Count * vector2.y;
			Rect rect = new Rect(0f, 0f, vector2.x, num3);
			inRect.height -= 40f;
			Rect rect2 = new Rect(0f, 0f, inRect.width - vector.x - (float)num, inRect.height - vector.y - (float)num - 20f);
			Widgets.BeginScrollView(rect2, ref this.scrollPosition, rect, true);
			try
			{
				float num4 = 0f;
				int num5 = 0;
				for (int i = 0; i < list.Count; i++)
				{
					PawnFilter pawnFilter = list[i];
					Rect rect3 = new Rect(0f, num4, vector2.x, vector2.y);
					if (this.selectedIndex == i)
					{
						GUI.DrawTexture(rect3, Textures.TextureHighlightRow);
					}
					else if (num5 % 2 == 0)
					{
						GUI.DrawTexture(rect3, Textures.TextureAlternateRow);
					}
					Color color = ((this.selectedIndex == i) ? new Color(0.7f, 0.7f, 0.7f, 1f) : Color.white);
					if (Widgets.ButtonText(rect3, "", false, true, color, true, null))
					{
						this.selectedIndex = i;
						SaveLoadDialog.Filename = pawnFilter.name;
					}
					GUI.BeginGroup(new Rect(rect3.x + 3f, rect3.y + 3f, rect3.width - 6f, rect3.height - 6f));
					try
					{
						GUI.color = SaveLoadDialog.ManualSaveTextColor;
						Rect rect4 = new Rect(15f, 0f, vector2.x, vector2.y);
						Text.Anchor = TextAnchor.MiddleLeft;
						Text.Font = GameFont.Small;
						Widgets.Label(rect4, pawnFilter.name);
						GUI.color = Color.white;
					}
					finally
					{
						GUI.EndGroup();
					}
					num4 += vector2.y;
					num5++;
				}
			}
			finally
			{
				Widgets.EndScrollView();
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
			}
			GUI.BeginGroup(new Rect(rect.x + rect.width + (float)num + (float)num2, 0f, vector.x, rect2.height));
			try
			{
				if (this.selectedIndex == -1)
				{
					GUI.enabled = false;
				}
				Rect rect5 = new Rect(0f, 0f, vector.x, vector.y);
				if (Widgets.ButtonText(rect5, Translator.Translate("RandomPlus.SaveLoadDialog.LoadButton"), true, false, true, null))
				{
					SaveLoader.Load(RandomSettings.pawnFilterList[this.selectedIndex]);
					this.Close(true);
				}
				if (Widgets.ButtonText(rect5.OffsetBy(new Vector2(0f, vector.y + (float)num)), Translator.Translate("RandomPlus.SaveLoadDialog.DeleteButton"), true, false, true, null))
				{
					SaveLoader.Delete(RandomSettings.pawnFilterList[this.selectedIndex]);
					this.selectedIndex = -1;
				}
				if (this.selectedIndex == -1)
				{
					GUI.enabled = true;
				}
			}
			finally
			{
				GUI.EndGroup();
			}
			this.DrawFooter(GenUI.AtZero(inRect));
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004F20 File Offset: 0x00003120
		protected void DrawFooter(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
			float num = inRect.height - 52f;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.SetNextControlName("ColonistNameField");
			string text = Widgets.TextField(new Rect(5f, num, 400f, 35f), SaveLoadDialog.Filename);
			if (GenText.IsValidFilename(text))
			{
				SaveLoadDialog.Filename = text;
				int num2 = RandomSettings.pawnFilterList.FindIndex((PawnFilter i) => i.name == SaveLoadDialog.Filename);
				if (num2 >= 0)
				{
					this.selectedIndex = num2;
				}
				else
				{
					this.selectedIndex = -1;
				}
			}
			if (!this.focusedColonistNameArea)
			{
				GUI.FocusControl("ColonistNameField");
				this.focusedColonistNameArea = true;
			}
			Rect rect = new Rect(420f, num, inRect.width - 400f - 20f, 35f);
			GUI.SetNextControlName("SaveButton");
			string text2 = ((this.selectedIndex >= 0) ? Translator.Translate("RandomPlus.SaveLoadDialog.OverwriteButton") : Translator.Translate("RandomPlus.SaveLoadDialog.SaveButton"));
			if (Widgets.ButtonText(rect, text2, true, false, true, null) || flag)
			{
				if (SaveLoadDialog.Filename.Length == 0)
				{
					Messages.Message(Translator.Translate("NeedAName"), MessageTypeDefOf.RejectInput, true);
				}
				else if (this.selectedIndex >= 0)
				{
					RandomSettings.PawnFilter.name = text;
					SaveLoader.SaveOverwrite(this.selectedIndex, RandomSettings.PawnFilter);
					this.Close(true);
				}
				else
				{
					RandomSettings.PawnFilter.name = text;
					SaveLoader.Save(RandomSettings.PawnFilter);
					this.Close(true);
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();
		}

		// Token: 0x04000058 RID: 88
		protected const float DeleteButtonSpace = 5f;

		// Token: 0x04000059 RID: 89
		protected const float MapDateExtraLeftMargin = 220f;

		// Token: 0x0400005A RID: 90
		private static readonly Color ManualSaveTextColor = new Color(1f, 1f, 0.6f);

		// Token: 0x0400005B RID: 91
		private static readonly Color AutosaveTextColor = new Color(0.75f, 0.75f, 0.75f);

		// Token: 0x0400005C RID: 92
		protected const float MapEntrySpacing = 8f;

		// Token: 0x0400005D RID: 93
		protected const float BoxMargin = 20f;

		// Token: 0x0400005E RID: 94
		protected const float MapNameExtraLeftMargin = 15f;

		// Token: 0x0400005F RID: 95
		protected const float MapEntryMargin = 6f;

		// Token: 0x04000060 RID: 96
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x04000061 RID: 97
		protected static string Filename = "";

		// Token: 0x04000062 RID: 98
		private bool focusedColonistNameArea;

		// Token: 0x04000063 RID: 99
		private int selectedIndex = -1;
	}
}
