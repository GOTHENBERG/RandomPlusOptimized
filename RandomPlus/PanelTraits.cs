using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RandomPlus
{
	// Token: 0x0200000A RID: 10
	public class PanelTraits : PanelBase
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00003DB4 File Offset: 0x00001FB4
		public PanelTraits()
		{
			this.Resize(new Rect(340f, 40f, 320f, 180f));
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003E52 File Offset: 0x00002052
		public override string PanelHeader
		{
			get
			{
				return Translator.Translate("RandomPlus.PanelTraits.Header");
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003E64 File Offset: 0x00002064
		public override void Resize(Rect rect)
		{
			base.Resize(rect);
			float num = 10f;
			float num2 = 28f;
			this.SizeTrait = new Vector2(base.PanelRect.width - num * 2f - 20f, num2 + this.SizeFieldPadding.y * 2f);
			this.SizeField = new Vector2(this.SizeTrait.x - this.SizeFieldPadding.x * 2f, this.SizeTrait.y - this.SizeFieldPadding.y * 2f);
			this.RectScrollFrame = new Rect(num, base.BodyRect.y, base.PanelRect.width - num * 2f, base.BodyRect.height - num);
			this.RectScrollView = new Rect(0f, 0f, this.RectScrollFrame.width, this.RectScrollFrame.height - 20f);
			this.traitPoolLabelRect = new Rect(8f, this.RectScrollView.y + this.RectScrollView.height, 200f, 30f);
			this.traitPoolButtonRect = new Rect(190f, this.RectScrollView.y + this.RectScrollView.height + 1f, 104f, 20f);
			this.disabledLabelRect = new Rect(20f, 30f, 100f, 20f);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003FFC File Offset: 0x000021FC
		protected override void DrawPanelContent()
		{
			base.DrawPanelContent();
			if (Page_RandomEditor.MOD_WELL_MET)
			{
				Widgets.Label(this.disabledLabelRect, Translator.Translate("RandomPlus.PanelTraits.Disabled"));
				return;
			}
			float num = 0f;
			GUI.color = Color.white;
			GUI.BeginGroup(this.RectScrollFrame);
			try
			{
				if (!RandomSettings.PawnFilter.Traits.Any<TraitContainer>())
				{
					GUI.color = Style.ColorText;
					Widgets.Label(this.RectScrollView.InsetBy(6f, 0f, 0f, 0f), Translator.Translate("RandomPlus.PanelTraits.NoTraits"));
				}
				GUI.color = Color.white;
				this.scrollView.Begin(this.RectScrollView);
				int num2 = 0;
				foreach (TraitContainer traitContainer in RandomSettings.PawnFilter.Traits)
				{
					if (num2 >= this.fields.Count)
					{
						this.fields.Add(new Field());
					}
					Field field = this.fields[num2];
					GUI.color = Style.ColorPanelBackgroundItem;
					GUI.DrawTexture(new Rect(0f, num, this.SizeTrait.x - (float)(this.scrollView.ScrollbarsVisible ? 16 : 0), this.SizeTrait.y), BaseContent.WhiteTex);
					GUI.color = Color.white;
					Rect rect = new Rect(this.SizeFieldPadding.x, num + this.SizeFieldPadding.y, this.SizeField.x, this.SizeField.y);
					if (this.scrollView.ScrollbarsVisible)
					{
						rect.width -= 16f;
					}
					field.Rect = rect;
					Rect rect2 = rect;
					rect2.width -= 36f;
					field.ClickRect = new Rect?(rect2);
					if (traitContainer != null)
					{
						field.Label = traitContainer.trait.LabelCap + this.LabelForTraitFilter(num2) + " " + RandomSettings.GetTraitRollChanceText(traitContainer.trait.def);
						field.Tip = traitContainer.trait.CurrentData.description;
					}
					else
					{
						field.Label = null;
					}
					Trait localTrait = traitContainer.trait;
					int localIndex = num2;
					field.ClickAction = delegate
					{
						Trait localTrait2 = localTrait;
						Trait selectedTrait2 = localTrait2;
						this.ComputeDisallowedTraits(localTrait2);
						Dialog_Options<Trait> dialog_Options3 = new Dialog_Options<Trait>(ProviderTraits.Traits);
						dialog_Options3.NameFunc = (Trait t) => t.LabelCap;
						dialog_Options3.DescriptionFunc = (Trait t) => t.CurrentData.description;
						dialog_Options3.SelectedFunc = (Trait t) => ((selectedTrait2 != null && t != null) || selectedTrait2 == t) && selectedTrait2.def == t.def && selectedTrait2.Label == t.Label;
						dialog_Options3.SelectAction = delegate(Trait t)
						{
							selectedTrait2 = t;
						};
						dialog_Options3.EnabledFunc = (Trait t) => !this.disallowedTraitDefs.Contains(t.def) && !this.disallowedTraitLabels.Contains(t.Label);
						dialog_Options3.CloseAction = delegate
						{
							RandomSettings.PawnFilter.TraitUpdated(localIndex, selectedTrait2);
						};
						dialog_Options3.NoneSelectedFunc = () => selectedTrait2 == null;
						dialog_Options3.SelectNoneAction = delegate
						{
							selectedTrait2 = null;
						};
						Dialog_Options<Trait> dialog_Options4 = dialog_Options3;
						Find.WindowStack.Add(dialog_Options4);
					};
					field.Draw();
					Rect rect3 = new Rect(field.Rect.xMax - 32f, field.Rect.y + field.Rect.HalfHeight() - 6f, 12f, 12f);
					if (rect3.Contains(Event.current.mousePosition))
					{
						GUI.color = Style.ColorButtonHighlight;
					}
					else
					{
						GUI.color = Style.ColorButton;
					}
					GUI.DrawTexture(rect3, Textures.TextureButtonDelete);
					if (Widgets.ButtonInvisible(rect3, false))
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
						this.traitsToRemove.Add(traitContainer.trait);
					}
					Rect rect4 = new Rect(field.Rect.xMax + 12f, field.Rect.y + field.Rect.HalfHeight() - 6f, 12f, 12f);
					GUI.color = (rect4.Contains(Event.current.mousePosition) ? Style.ColorButtonHighlight : Style.ColorButton);
					GUI.DrawTexture(rect4, Textures.TextureButtonReset);
					if (Widgets.ButtonInvisible(rect4, false))
					{
						SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
						this.CycleTraitFilter(num2);
					}
					num2++;
					num += this.SizeTrait.y + this.SizeTraitMargin.y;
				}
				num -= this.SizeTraitMargin.y;
			}
			finally
			{
				this.scrollView.End(num);
				GUI.color = Color.white;
				this.drawTraitPool();
				GUI.EndGroup();
			}
			Rect rect5 = new Rect(base.PanelRect.width - 24f, 12f, 16f, 16f);
			Style.SetGUIColorForButton(rect5);
			bool flag = RandomSettings.PawnFilter.Traits.Count<TraitContainer>() < ProviderTraits.Traits.Count<Trait>();
			if (!flag)
			{
				GUI.color = Style.ColorButtonDisabled;
			}
			GUI.DrawTexture(rect5, Textures.TextureButtonAdd);
			if (flag && Widgets.ButtonInvisible(rect5, false))
			{
				this.ComputeDisallowedTraits(null);
				SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Low, null);
				Trait selectedTrait = null;
				Dialog_Options<Trait> dialog_Options = new Dialog_Options<Trait>(ProviderTraits.Traits);
				dialog_Options.ConfirmButtonLabel = Translator.Translate("RandomPlus.PanelTraits.AddDialog.AddButton");
				dialog_Options.NameFunc = (Trait t) => t.LabelCap + " " + RandomSettings.GetTraitRollChanceText(t.def);
				dialog_Options.DescriptionFunc = (Trait t) => t.CurrentData.description;
				dialog_Options.SelectedFunc = (Trait t) => selectedTrait == t;
				dialog_Options.SelectAction = delegate(Trait t)
				{
					selectedTrait = t;
				};
				dialog_Options.EnabledFunc = (Trait t) => !this.disallowedTraitDefs.Contains(t.def) && !this.disallowedTraitLabels.Contains(t.Label);
				dialog_Options.CloseAction = delegate
				{
					if (selectedTrait != null)
					{
						RandomSettings.PawnFilter.AddTrait(selectedTrait);
					}
				};
				Dialog_Options<Trait> dialog_Options2 = dialog_Options;
				Find.WindowStack.Add(dialog_Options2);
			}
			if (this.traitsToRemove.Count > 0)
			{
				foreach (Trait trait in this.traitsToRemove)
				{
					RandomSettings.PawnFilter.TraitRemoved(trait);
				}
				this.traitsToRemove.Clear();
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004608 File Offset: 0x00002808
		protected void ComputeDisallowedTraits(Trait traitToReplace)
		{
			this.disallowedTraitDefs.Clear();
			this.disallowedTraitLabels.Clear();
			List<string> exTags = new List<string>();
			foreach (TraitContainer traitContainer in RandomSettings.PawnFilter.Traits)
			{
				if (traitContainer.trait != traitToReplace)
				{
					if (traitContainer.traitFilter == TraitContainer.TraitFilterType.Required)
					{
						List<string> exclusionTags = traitContainer.trait.def.exclusionTags;
						if (exclusionTags != null)
						{
							exclusionTags.ForEach(delegate(string t)
							{
								GenCollection.AddDistinct<string>(exTags, t);
							});
						}
						if (traitContainer.trait.def.conflictingTraits != null)
						{
							foreach (TraitDef traitDef in traitContainer.trait.def.conflictingTraits)
							{
								this.disallowedTraitDefs.Add(traitDef);
							}
						}
						this.disallowedTraitDefs.Add(traitContainer.trait.def);
					}
					else
					{
						this.disallowedTraitLabels.Add(traitContainer.trait.Label);
					}
				}
			}
			foreach (TraitDef traitDef2 in DefDatabase<TraitDef>.AllDefs)
			{
				if (traitDef2.exclusionTags.Intersect<string>(exTags).Any<string>())
				{
					this.disallowedTraitDefs.Add(traitDef2);
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000047C4 File Offset: 0x000029C4
		protected void ClearTrait(int traitIndex)
		{
			RandomSettings.PawnFilter.TraitUpdated(traitIndex, null);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000047D2 File Offset: 0x000029D2
		public void ScrollToTop()
		{
			this.scrollView.ScrollToTop();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000047DF File Offset: 0x000029DF
		public void ScrollToBottom()
		{
			this.scrollView.ScrollToBottom();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000047EC File Offset: 0x000029EC
		protected void CycleTraitFilter(int index)
		{
			if (index < 0 || index >= RandomSettings.PawnFilter.Traits.Count<TraitContainer>())
			{
				return;
			}
			TraitContainer.TraitFilterType traitFilterType = RandomSettings.PawnFilter.Traits.ElementAt<TraitContainer>(index).traitFilter;
			switch (traitFilterType)
			{
			case TraitContainer.TraitFilterType.Required:
				traitFilterType = TraitContainer.TraitFilterType.Optional;
				break;
			case TraitContainer.TraitFilterType.Optional:
				traitFilterType = TraitContainer.TraitFilterType.Excluded;
				break;
			case TraitContainer.TraitFilterType.Excluded:
				traitFilterType = TraitContainer.TraitFilterType.Required;
				break;
			}
			RandomSettings.PawnFilter.Traits.ElementAt<TraitContainer>(index).traitFilter = traitFilterType;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000485C File Offset: 0x00002A5C
		protected string LabelForTraitFilter(int index)
		{
			if (index < 0 || index >= RandomSettings.PawnFilter.Traits.Count<TraitContainer>())
			{
				return "";
			}
			switch (RandomSettings.PawnFilter.Traits.ElementAt<TraitContainer>(index).traitFilter)
			{
			case TraitContainer.TraitFilterType.Required:
				return Translator.Translate("RandomPlus.PanelTraits.FilterType.Required");
			case TraitContainer.TraitFilterType.Optional:
				return Translator.Translate("RandomPlus.PanelTraits.FilterType.Pool");
			case TraitContainer.TraitFilterType.Excluded:
				return Translator.Translate("RandomPlus.PanelTraits.FilterType.Excluded");
			default:
				return "";
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000048E4 File Offset: 0x00002AE4
		public void drawTraitPool()
		{
			Widgets.Label(this.traitPoolLabelRect, Translator.Translate("RandomPlus.PanelTraits.PoolLabel"));
			if (Widgets.ButtonText(this.traitPoolButtonRect, RandomSettings.PawnFilter.RequiredTraitsInPool.ToString(), true, true, true, null))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				int[] array = new int[] { 0, 1, 2, 3 };
				for (int i = 0; i < array.Length; i++)
				{
					int rangeOption = array[i];
					FloatMenuOption floatMenuOption = new FloatMenuOption(rangeOption.ToString(), delegate
					{
						RandomSettings.PawnFilter.RequiredTraitsInPool = rangeOption;
					}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
					list.Add(floatMenuOption);
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		// Token: 0x04000046 RID: 70
		protected ScrollViewVertical scrollView = new ScrollViewVertical();

		// Token: 0x04000047 RID: 71
		protected List<Field> fields = new List<Field>();

		// Token: 0x04000048 RID: 72
		protected List<Trait> traitsToRemove = new List<Trait>();

		// Token: 0x04000049 RID: 73
		protected HashSet<TraitDef> disallowedTraitDefs = new HashSet<TraitDef>();

		// Token: 0x0400004A RID: 74
		protected HashSet<string> disallowedTraitLabels = new HashSet<string>();

		// Token: 0x0400004B RID: 75
		protected Dictionary<Trait, string> conflictingTraitList = new Dictionary<Trait, string>();

		// Token: 0x0400004C RID: 76
		protected Vector2 SizeField;

		// Token: 0x0400004D RID: 77
		protected Vector2 SizeTrait;

		// Token: 0x0400004E RID: 78
		protected Vector2 SizeFieldPadding = new Vector2(5f, 6f);

		// Token: 0x0400004F RID: 79
		protected Vector2 SizeTraitMargin = new Vector2(4f, -6f);

		// Token: 0x04000050 RID: 80
		protected Rect RectScrollFrame;

		// Token: 0x04000051 RID: 81
		protected Rect RectScrollView;

		// Token: 0x04000052 RID: 82
		protected Rect traitPoolLabelRect;

		// Token: 0x04000053 RID: 83
		protected Rect traitPoolButtonRect;

		// Token: 0x04000054 RID: 84
		protected Rect disabledLabelRect;
	}
}
