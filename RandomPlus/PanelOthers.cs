using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000017 RID: 23
	public class PanelOthers : PanelBase
	{
		// Token: 0x06000098 RID: 152 RVA: 0x0000615C File Offset: 0x0000435C
		public PanelOthers()
		{
			this.Resize(new Rect(340f, 240f, 320f, 226f));
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000061AD File Offset: 0x000043AD
		public override string PanelHeader
		{
			get
			{
				return Translator.Translate("RandomPlus.PanelOthers.Header");
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000061C0 File Offset: 0x000043C0
		public override void Resize(Rect rect)
		{
			base.Resize(rect);
			this.randomRerollAlgorithmLabelRect = new Rect(18f, 42f, 150f, 30f);
			this.randomRerollAlgorithmButtonRect = new Rect(150f, 43f, 154f, 20f);
			this.randomRerollLimitLabelRect = this.randomRerollAlgorithmLabelRect.OffsetBy(PanelOthers.otherLabelOffset);
			this.randomRerollLimitButtonRect = this.randomRerollAlgorithmButtonRect.OffsetBy(PanelOthers.otherButtonOffset);
			this.genderLabelRect = this.randomRerollLimitLabelRect.OffsetBy(PanelOthers.otherLabelOffset);
			this.genderButtonRect = this.randomRerollLimitButtonRect.OffsetBy(PanelOthers.otherButtonOffset);
			this.healthConditionLabelRect = this.genderLabelRect.OffsetBy(PanelOthers.otherLabelOffset);
			this.healthConditionButtonRect = this.genderButtonRect.OffsetBy(PanelOthers.otherButtonOffset);
			this.incapableLabelRect = this.healthConditionLabelRect.OffsetBy(PanelOthers.otherLabelOffset);
			this.incapableButtonRect = this.healthConditionButtonRect.OffsetBy(PanelOthers.otherButtonOffset);
			this.ageLabelRect = this.incapableLabelRect.OffsetBy(0f, 28f);
			this.ageLabelRect.width = 284f;
			this.ageRangeRect = this.ageLabelRect.OffsetBy(0f, 8f);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006308 File Offset: 0x00004508
		protected override void DrawPanelContent()
		{
			base.DrawPanelContent();
			GUI.BeginGroup(this.mainRect);
			try
			{
				Widgets.Label(this.randomRerollAlgorithmLabelRect, Translator.Translate("RandomPlus.PanelOthers.RerollAlgorithmLabel"));
				this.drawRerollAlgorithm(this.randomRerollAlgorithmButtonRect);
				Widgets.Label(this.randomRerollLimitLabelRect, Translator.Translate("RandomPlus.PanelOthers.RerollLimitLabel"));
				this.drawRerollLimit(this.randomRerollLimitButtonRect);
				Widgets.Label(this.genderLabelRect, Translator.Translate("RandomPlus.PanelOthers.GenderLabel"));
				this.drawGender(this.genderButtonRect);
				Widgets.Label(this.healthConditionLabelRect, Translator.Translate("RandomPlus.PanelOthers.HealthOptionLabel"));
				this.drawHealthCondition(this.healthConditionButtonRect);
				Widgets.Label(this.incapableLabelRect, Translator.Translate("RandomPlus.PanelOthers.IncapableOptionLabel"));
				this.drawIncapable(this.incapableButtonRect);
				string text = RandomSettings.PawnFilter.ageRange.min.ToString();
				string text2 = ((RandomSettings.PawnFilter.ageRange.max == PawnFilter.MaxAgeDefault) ? "∞" : RandomSettings.PawnFilter.ageRange.max.ToString());
				string text3 = string.Format(Translator.Translate("RandomPlus.PanelOthers.AgeLabel"), text, text2);
				Widgets.Label(this.ageLabelRect, text3);
				Widgets.IntRange(this.ageRangeRect, 20, ref RandomSettings.PawnFilter.ageRange, 0, PawnFilter.MaxAgeDefault, "", 2);
			}
			finally
			{
				GUI.EndGroup();
			}
			GUI.color = Color.white;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006488 File Offset: 0x00004688
		public void drawRerollAlgorithm(Rect rect)
		{
			this.drawButton(rect, PawnFilter.RerollAlgorithmOptionValues[(int)RandomSettings.PawnFilter.RerollAlgorithm], typeof(PawnFilter.RerollAlgorithmOptions), PawnFilter.RerollAlgorithmOptionValues, PanelOthers.rerollAlgorithmCallback, true);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000064B8 File Offset: 0x000046B8
		public void drawRerollLimit(Rect rect)
		{
			this.drawButton(rect, RandomSettings.PawnFilter.RerollLimit.ToString(), typeof(PawnFilter.RerollLimitOptions), PawnFilter.RerollLimitOptionValues, PanelOthers.rerollLimitCallback, true);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000064F4 File Offset: 0x000046F4
		public void drawGender(Rect rect)
		{
			string[] array = (from gender in Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList<Gender>()
				select GenderUtility.GetLabel(gender, false)).ToArray<string>();
			this.drawButton(rect, GenderUtility.GetLabel(RandomSettings.PawnFilter.Gender, false), typeof(Gender), array, PanelOthers.genderCallback, false);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000656C File Offset: 0x0000476C
		public void drawHealthCondition(Rect rect)
		{
			this.drawButton(rect, PawnFilter.HealthOptionValues[(int)RandomSettings.PawnFilter.FilterHealthCondition], typeof(PawnFilter.HealthOptions), PawnFilter.HealthOptionValues, PanelOthers.healthCallback, true);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000659A File Offset: 0x0000479A
		public void drawIncapable(Rect rect)
		{
			this.drawButton(rect, PawnFilter.IncapableOptionValues[(int)RandomSettings.PawnFilter.FilterIncapable], typeof(PawnFilter.IncapableOptions), PawnFilter.IncapableOptionValues, PanelOthers.incapableCallback, true);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000065C8 File Offset: 0x000047C8
		public void drawButton(Rect rect, string label, Type enumOptionType, string[] displayedNameArray, Action<Enum> callback, bool translate = true)
		{
			string text = (translate ? Translator.Translate(label).CapitalizeFirst().ToString() : GenText.CapitalizeFirst(label));
			if (Widgets.ButtonText(rect, text, true, true, true, null))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				Enum[] array = Enum.GetValues(enumOptionType).Cast<Enum>().ToArray<Enum>();
				for (int i = 0; i < array.Length; i++)
				{
					Enum option = array[i];
					if (displayedNameArray.Length > i)
					{
						FloatMenuOption floatMenuOption = new FloatMenuOption(translate ? Translator.Translate(displayedNameArray[i]).CapitalizeFirst().ToString() : GenText.CapitalizeFirst(displayedNameArray[i]), delegate
						{
							Action<Enum> callback2 = callback;
							if (callback2 == null)
							{
								return;
							}
							callback2(option);
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						list.Add(floatMenuOption);
					}
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		// Token: 0x040000B3 RID: 179
		private Rect mainRect = new Rect(0f, 0f, 500f, 500f);

		// Token: 0x040000B4 RID: 180
		private static readonly Vector2 otherLabelOffset = new Vector2(0f, 24f);

		// Token: 0x040000B5 RID: 181
		private static readonly Vector2 otherButtonOffset = new Vector2(0f, 24f);

		// Token: 0x040000B6 RID: 182
		private Rect randomRerollAlgorithmLabelRect;

		// Token: 0x040000B7 RID: 183
		private Rect randomRerollAlgorithmButtonRect;

		// Token: 0x040000B8 RID: 184
		private Rect randomRerollLimitLabelRect;

		// Token: 0x040000B9 RID: 185
		private Rect randomRerollLimitButtonRect;

		// Token: 0x040000BA RID: 186
		private Rect genderLabelRect;

		// Token: 0x040000BB RID: 187
		private Rect genderButtonRect;

		// Token: 0x040000BC RID: 188
		private Rect ageLabelRect;

		// Token: 0x040000BD RID: 189
		private Rect ageRangeRect;

		// Token: 0x040000BE RID: 190
		private Rect healthConditionLabelRect;

		// Token: 0x040000BF RID: 191
		private Rect healthConditionButtonRect;

		// Token: 0x040000C0 RID: 192
		private Rect incapableLabelRect;

		// Token: 0x040000C1 RID: 193
		private Rect incapableButtonRect;

		// Token: 0x040000C2 RID: 194
		private static readonly Action<Enum> rerollAlgorithmCallback = delegate(Enum val)
		{
			RandomSettings.PawnFilter.RerollAlgorithm = (PawnFilter.RerollAlgorithmOptions)val;
		};

		// Token: 0x040000C3 RID: 195
		private static readonly Action<Enum> rerollLimitCallback = delegate(Enum val)
		{
			RandomSettings.PawnFilter.RerollLimit = (int)((PawnFilter.RerollLimitOptions)val);
		};

		// Token: 0x040000C4 RID: 196
		private static readonly Action<Enum> genderCallback = delegate(Enum val)
		{
			RandomSettings.SetGenderFilter((Gender)val);
		};

		// Token: 0x040000C5 RID: 197
		private static readonly Action<Enum> healthCallback = delegate(Enum val)
		{
			RandomSettings.PawnFilter.FilterHealthCondition = (PawnFilter.HealthOptions)val;
		};

		// Token: 0x040000C6 RID: 198
		private static readonly Action<Enum> incapableCallback = delegate(Enum val)
		{
			RandomSettings.PawnFilter.FilterIncapable = (PawnFilter.IncapableOptions)val;
		};
	}
}
