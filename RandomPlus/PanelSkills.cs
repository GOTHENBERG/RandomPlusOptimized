using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RandomPlus
{
	// Token: 0x02000009 RID: 9
	public class PanelSkills : PanelBase
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00003315 File Offset: 0x00001515
		public PanelSkills()
		{
			this.Resize(new Rect(0f, 40f, 320f, 500f));
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00003347 File Offset: 0x00001547
		public override string PanelHeader
		{
			get
			{
				return Translator.Translate("RandomPlus.PanelSkills.Header");
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003358 File Offset: 0x00001558
		public override void Resize(Rect rect)
		{
			base.Resize(rect);
			float num = 16f;
			float num2 = 10f;
			float num3 = 10f;
			float num4 = 4f;
			float num5 = base.BodyRect.y + num4;
			GameFont font = Text.Font;
			Text.Font = GameFont.Small;
			Vector2 vector = new Vector2(float.MinValue, float.MinValue);
			foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefs)
			{
				Vector2 vector2 = Text.CalcSize(skillDef.skillLabel);
				vector2 += new Vector2(4f, 0f);
				vector.x = Mathf.Max(vector2.x, vector.x);
				vector.y = Mathf.Max(vector2.y, vector.y);
			}
			Text.Font = font;
			float num6 = 4f;
			float num7 = base.PanelRect.width - num - num2;
			Vector2 vector3 = new Vector2(24f, 24f);
			float num8 = 2f;
			Vector2 vector4 = new Vector2(16f, 16f);
			float num9 = 32f;
			Vector2 vector5 = new Vector2(num7 - vector3.x - num8 - num9 - vector.x - num6, 22f);
			PanelSkills.RectButtonClearSkills = new Rect(base.PanelRect.width - 38f, 8f, 23f, 21f);
			PanelSkills.RectLabel = new Rect(0f, 0f, vector.x, vector.y);
			PanelSkills.RectPassion = new Rect(PanelSkills.RectLabel.xMax + num6, vector.y * 0.5f - vector3.y * 0.5f, vector3.x, vector3.y);
			PanelSkills.RectSkillBar = new Rect(PanelSkills.RectPassion.xMax + num8, vector.y * 0.5f - vector5.y * 0.5f, vector5.x, vector5.y);
			PanelSkills.RectButtonDecrement = new Rect(PanelSkills.RectSkillBar.xMax, vector.y * 0.5f - vector4.y * 0.5f, vector4.x, vector4.y);
			PanelSkills.RectButtonIncrement = new Rect(PanelSkills.RectButtonDecrement.xMax, vector.y * 0.5f - vector4.y * 0.5f, vector4.x, vector4.y);
			PanelSkills.RectScrollFrame = new Rect(num, num5, num7, base.BodyRect.height - num4 - num3);
			PanelSkills.RectScrollView = new Rect(0f, 0f, PanelSkills.RectScrollFrame.width, PanelSkills.RectScrollFrame.height - 60f);
			PanelSkills.totalPassionLabelRect = new Rect(0f, 318f, PanelSkills.RectScrollFrame.width - 8f, 30f);
			PanelSkills.totalPassionRangeRect = PanelSkills.totalPassionLabelRect.OffsetBy(0f, 18f);
			PanelSkills.totalSkillLabelRect = PanelSkills.totalPassionLabelRect.OffsetBy(0f, 48f);
			PanelSkills.optionCountOnlyHighestAttackBoxRect = PanelSkills.totalSkillLabelRect.OffsetBy(10f, 16f);
			PanelSkills.optionCountOnlyHighestAttackBoxRect.width = PanelSkills.optionCountOnlyHighestAttackBoxRect.width - 10f;
			PanelSkills.optionCountOnlyPassionBoxRect = PanelSkills.optionCountOnlyHighestAttackBoxRect.OffsetBy(0f, 24f);
			PanelSkills.totalSkillRangeRect = PanelSkills.totalSkillLabelRect.OffsetBy(0f, 62f);
			PanelSkills.totalPassionRangeRect.height = 18f;
			PanelSkills.totalSkillRangeRect.height = 18f;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003738 File Offset: 0x00001938
		protected override void DrawPanelContent()
		{
			base.DrawPanelContent();
			Style.SetGUIColorForButton(PanelSkills.RectButtonClearSkills);
			GUI.DrawTexture(PanelSkills.RectButtonClearSkills, Textures.TextureButtonClearSkills);
			if (Widgets.ButtonInvisible(PanelSkills.RectButtonClearSkills, false))
			{
				SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Low, null);
				RandomSettings.PawnFilter.ResetSkills();
			}
			int num = RandomSettings.PawnFilter.Skills.Count<SkillContainer>();
			float num2 = 26f;
			bool flag = num2 * (float)num > PanelSkills.RectScrollView.height;
			float num3 = 0f;
			GUI.BeginGroup(PanelSkills.RectScrollFrame);
			try
			{
				this.scrollView.Begin(PanelSkills.RectScrollView);
				Text.Font = GameFont.Small;
				foreach (SkillContainer skillContainer in RandomSettings.PawnFilter.Skills)
				{
					SkillDef skillDef = skillContainer.SkillDef;
					GUI.color = Style.ColorText;
					Rect rect = PanelSkills.RectLabel;
					rect.y += num3;
					Widgets.Label(rect, GenText.CapitalizeFirst(skillDef.skillLabel));
					rect = PanelSkills.RectPassion;
					rect.y += num3;
					Texture2D texture2D;
					if (skillContainer.Passion == Passion.Minor)
					{
						texture2D = Textures.TexturePassionMinor;
					}
					else if (skillContainer.Passion == Passion.Major)
					{
						texture2D = Textures.TexturePassionMajor;
					}
					else
					{
						texture2D = Textures.TexturePassionNone;
					}
					GUI.color = Color.white;
					GUI.DrawTexture(rect, texture2D);
					if (Widgets.ButtonInvisible(rect, false))
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
						if (Event.current.button != 1)
						{
							this.IncreasePassion(skillContainer);
						}
						else
						{
							this.DecreasePassion(skillContainer);
						}
					}
					rect = PanelSkills.RectSkillBar;
					rect.y += num3;
					if (flag)
					{
						rect.width -= 16f;
					}
					this.DrawSkill(skillContainer, rect);
					rect = PanelSkills.RectButtonDecrement;
					rect.y += num3;
					rect.x -= (float)(flag ? 16 : 0);
					if (rect.Contains(Event.current.mousePosition))
					{
						GUI.color = Style.ColorButtonHighlight;
					}
					else
					{
						GUI.color = Style.ColorButton;
					}
					GUI.DrawTexture(rect, Textures.TextureButtonPrevious);
					if (Widgets.ButtonInvisible(rect, false))
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
						skillContainer.MinValue--;
					}
					rect = PanelSkills.RectButtonIncrement;
					rect.y += num3;
					rect.x -= (float)(flag ? 16 : 0);
					if (rect.Contains(Event.current.mousePosition))
					{
						GUI.color = Style.ColorButtonHighlight;
					}
					else
					{
						GUI.color = Style.ColorButton;
					}
					GUI.DrawTexture(rect, Textures.TextureButtonNext);
					if (Widgets.ButtonInvisible(rect, false))
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
						skillContainer.MinValue++;
					}
					num3 += num2;
				}
				this.scrollView.End(num3);
			}
			finally
			{
				GUI.color = Color.white;
				string text = string.Format(Translator.Translate("RandomPlus.PanelSkills.PassionSlider"), RandomSettings.PawnFilter.passionRange.min, RandomSettings.PawnFilter.passionRange.max);
				Widgets.Label(PanelSkills.totalPassionLabelRect, text);
				Widgets.IntRange(PanelSkills.totalPassionRangeRect, 2042, ref RandomSettings.PawnFilter.passionRange, 0, PawnFilter.PassionMaxDefault, "", 2);
				string text2 = string.Format(Translator.Translate("RandomPlus.PanelSkills.SkillSlider"), RandomSettings.PawnFilter.skillRange.min, (RandomSettings.PawnFilter.skillRange.max == PawnFilter.SkillMaxDefault) ? "∞" : RandomSettings.PawnFilter.skillRange.max.ToString());
				Widgets.Label(PanelSkills.totalSkillLabelRect, text2);
				Widgets.IntRange(PanelSkills.totalSkillRangeRect, 2043, ref RandomSettings.PawnFilter.skillRange, 0, PawnFilter.SkillMaxDefault, "", 2);
				Widgets.CheckboxLabeled(PanelSkills.optionCountOnlyHighestAttackBoxRect, "*" + Translator.Translate("RandomPlus.PanelSkills.CountOnlyHighestAttack"), ref RandomSettings.PawnFilter.countOnlyHighestAttack, false, null, null, false, false);
				Widgets.CheckboxLabeled(PanelSkills.optionCountOnlyPassionBoxRect, "*" + Translator.Translate("RandomPlus.PanelSkills.CountOnlyPassion"), ref RandomSettings.PawnFilter.countOnlyPassion, false, null, null, false, false);
				GUI.EndGroup();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public static void FillableBar(Rect rect, float fillPercent, Texture2D fillTex)
		{
			rect.width *= fillPercent;
			GUI.DrawTexture(rect, fillTex);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003BEC File Offset: 0x00001DEC
		private void DrawSkill(SkillContainer filter, Rect rect)
		{
			int minValue = filter.MinValue;
			float num = ((minValue > 0) ? ((float)minValue) : 0f) / 20f;
			PanelSkills.FillableBar(rect, num, Textures.TextureSkillBarFill);
			PanelSkills.FillableBar(rect, 0f, Textures.TextureSkillBarFill);
			GUI.color = new Color(0.25f, 0.25f, 0.25f);
			Widgets.DrawBox(rect, 1, null);
			GUI.color = Style.ColorText;
			if (Widgets.ButtonInvisible(rect, false))
			{
				float num2 = Event.current.mousePosition.x - rect.x;
				int num3;
				if (Mathf.Floor(num2 / rect.width * 20f) == 0f)
				{
					if (num2 <= 1f)
					{
						num3 = 0;
					}
					else
					{
						num3 = 1;
					}
				}
				else
				{
					num3 = Mathf.CeilToInt(num2 / rect.width * 20f);
				}
				SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
				filter.MinValue = num3;
			}
			string text = GenString.ToStringCached(minValue);
			Text.Anchor = TextAnchor.MiddleLeft;
			rect.x += 3f;
			rect.y += 1f;
			Widgets.Label(rect, text);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003D21 File Offset: 0x00001F21
		protected void IncreasePassion(SkillContainer filter)
		{
			if (filter.Passion == Passion.None)
			{
				filter.Passion = Passion.Minor;
				return;
			}
			if (filter.Passion == Passion.Minor)
			{
				filter.Passion = Passion.Major;
				return;
			}
			if (filter.Passion == Passion.Major)
			{
				filter.Passion = Passion.None;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003D54 File Offset: 0x00001F54
		protected void DecreasePassion(SkillContainer filter)
		{
			if (filter.Passion == Passion.None)
			{
				filter.Passion = Passion.Major;
				return;
			}
			if (filter.Passion == Passion.Minor)
			{
				filter.Passion = Passion.None;
				return;
			}
			if (filter.Passion == Passion.Major)
			{
				filter.Passion = Passion.Minor;
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003D87 File Offset: 0x00001F87
		public void ScrollToTop()
		{
			this.scrollView.ScrollToTop();
		}

		// Token: 0x04000036 RID: 54
		protected ScrollViewVertical scrollView = new ScrollViewVertical();

		// Token: 0x04000037 RID: 55
		private static Color ColorSkillDisabled = new Color(1f, 1f, 1f, 0.5f);

		// Token: 0x04000038 RID: 56
		protected static Rect RectButtonClearSkills;

		// Token: 0x04000039 RID: 57
		protected static Rect RectLabel;

		// Token: 0x0400003A RID: 58
		protected static Rect RectPassion;

		// Token: 0x0400003B RID: 59
		protected static Rect RectSkillBar;

		// Token: 0x0400003C RID: 60
		protected static Rect RectButtonDecrement;

		// Token: 0x0400003D RID: 61
		protected static Rect RectButtonIncrement;

		// Token: 0x0400003E RID: 62
		protected static Rect RectScrollFrame;

		// Token: 0x0400003F RID: 63
		protected static Rect RectScrollView;

		// Token: 0x04000040 RID: 64
		protected static Rect totalPassionLabelRect;

		// Token: 0x04000041 RID: 65
		protected static Rect totalPassionRangeRect;

		// Token: 0x04000042 RID: 66
		protected static Rect totalSkillLabelRect;

		// Token: 0x04000043 RID: 67
		protected static Rect totalSkillRangeRect;

		// Token: 0x04000044 RID: 68
		protected static Rect optionCountOnlyHighestAttackBoxRect;

		// Token: 0x04000045 RID: 69
		protected static Rect optionCountOnlyPassionBoxRect;
	}
}
