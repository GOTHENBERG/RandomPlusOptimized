using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000016 RID: 22
	public class Page_RandomEditor : Page
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00005E38 File Offset: 0x00004038
		public Page_RandomEditor()
		{
			this.closeOnCancel = true;
			this.closeOnAccept = true;
			this.closeOnClickedOutside = true;
			this.doCloseButton = true;
			this.doCloseX = true;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00005E64 File Offset: 0x00004064
		public override Vector2 InitialSize
		{
			get
			{
				float num = 1f;
				return new Vector2(694f * num, 630f * num);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00005E8A File Offset: 0x0000408A
		public override string PageTitle
		{
			get
			{
				return Translator.Translate("RandomPlus.RandomEditor.Header");
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005E9C File Offset: 0x0000409C
		public override void PreOpen()
		{
			base.PreOpen();
			if (ModsConfig.IsActive("Lakuna.WellMet"))
			{
				Page_RandomEditor.MOD_WELL_MET = true;
			}
			this.panelSkills = new PanelSkills();
			this.panelTraits = new PanelTraits();
			this.panelOthers = new PanelOthers();
			float uiscale = Prefs.UIScale;
			this.RectButtonResetAll = new Rect(this.InitialSize.x / uiscale - (float)(Page_RandomEditor.ButtonWidth + 50), (float)(Page_RandomEditor.ButtonHeight - 8), (float)Page_RandomEditor.ButtonWidth, (float)Page_RandomEditor.ButtonHeight);
			this.RectButtonSaveLoad = new Rect(this.InitialSize.x / uiscale - (float)(Page_RandomEditor.ButtonWidth * 2 + 60), (float)(Page_RandomEditor.ButtonHeight - 8), (float)Page_RandomEditor.ButtonWidth, (float)Page_RandomEditor.ButtonHeight);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005F58 File Offset: 0x00004158
		public override void DoWindowContents(Rect inRect)
		{
			base.DrawPageTitle(inRect);
			if (Prefs.DevMode && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
			{
				try
				{
					GenCommandLine.Restart();
				}
				catch (Exception ex)
				{
					Log.Warning("RandomPlus: Failed to restart via GenCommandLine: " + ex.Message);
				}
			}
			try
			{
				PanelSkills panelSkills = this.panelSkills;
				if (panelSkills != null)
				{
					panelSkills.Draw();
				}
				PanelTraits panelTraits = this.panelTraits;
				if (panelTraits != null)
				{
					panelTraits.Draw();
				}
				PanelOthers panelOthers = this.panelOthers;
				if (panelOthers != null)
				{
					panelOthers.Draw();
				}
			}
			catch (Exception ex2)
			{
				Log.Error("RandomPlus: Error drawing panels: " + ex2.Message);
			}
			try
			{
				if (Widgets.ButtonText(this.RectButtonSaveLoad, Translator.Translate("RandomPlus.RandomEditor.SaveLoadButton"), true, false, true, null))
				{
					Find.WindowStack.Add(new SaveLoadDialog());
				}
				if (Widgets.ButtonText(this.RectButtonResetAll, Translator.Translate("RandomPlus.RandomEditor.ResetAllButton"), true, true, true, null))
				{
					PawnFilter pawnFilter = RandomSettings.PawnFilter;
					if (pawnFilter != null)
					{
						pawnFilter.ResetAll();
					}
				}
			}
			catch (Exception ex3)
			{
				Log.Error("RandomPlus: Error drawing buttons: " + ex3.Message);
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000060B0 File Offset: 0x000042B0
		public override void WindowUpdate()
		{
			base.WindowUpdate();
			if (Event.current.type == EventType.Layout)
			{
				float uiscale = Prefs.UIScale;
				this.RectButtonResetAll = new Rect(this.InitialSize.x / uiscale - (float)(Page_RandomEditor.ButtonWidth + 50), (float)(Page_RandomEditor.ButtonHeight - 8), (float)Page_RandomEditor.ButtonWidth, (float)Page_RandomEditor.ButtonHeight);
				this.RectButtonSaveLoad = new Rect(this.InitialSize.x / uiscale - (float)(Page_RandomEditor.ButtonWidth * 2 + 60), (float)(Page_RandomEditor.ButtonHeight - 8), (float)Page_RandomEditor.ButtonWidth, (float)Page_RandomEditor.ButtonHeight);
			}
		}

		// Token: 0x040000AB RID: 171
		public static bool MOD_WELL_MET = false;

		// Token: 0x040000AC RID: 172
		private PanelSkills panelSkills;

		// Token: 0x040000AD RID: 173
		private PanelTraits panelTraits;

		// Token: 0x040000AE RID: 174
		private PanelOthers panelOthers;

		// Token: 0x040000AF RID: 175
		private static readonly int ButtonWidth = 100;

		// Token: 0x040000B0 RID: 176
		private static readonly int ButtonHeight = 20;

		// Token: 0x040000B1 RID: 177
		private Rect RectButtonResetAll;

		// Token: 0x040000B2 RID: 178
		private Rect RectButtonSaveLoad;
	}
}
