using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000004 RID: 4
	public class Dialog_Options<T> : Window
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002221 File Offset: 0x00000421
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002229 File Offset: 0x00000429
		public string HeaderLabel
		{
			get
			{
				return this.headerLabel;
			}
			set
			{
				this.headerLabel = value;
				this.ComputeSizes();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002238 File Offset: 0x00000438
		public Dialog_Options(IEnumerable<T> options)
			: base(null)
		{
			this.closeOnCancel = true;
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
			this.forcePause = true;
			this.options = options;
			this.ComputeSizes();
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002467 File Offset: 0x00000667
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000246F File Offset: 0x0000066F
		public IEnumerable<T> Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002478 File Offset: 0x00000678
		protected void ComputeSizes()
		{
			float num = 0f;
			if (this.HeaderLabel != null)
			{
				num = this.HeaderHeight;
			}
			this.ContentSize = new Vector2(this.WindowSize.x - this.WindowPadding * 2f - this.ContentMargin.x * 2f, this.WindowSize.y - this.WindowPadding * 2f - this.ContentMargin.y * 2f - this.FooterHeight - num);
			this.ContentRect = new Rect(this.ContentMargin.x, this.ContentMargin.y + num, this.ContentSize.x, this.ContentSize.y);
			this.ScrollRect = new Rect(0f, 0f, this.ContentRect.width, this.ContentRect.height);
			this.HeaderRect = new Rect(this.ContentMargin.x, this.ContentMargin.y, this.ContentSize.x, this.HeaderHeight);
			this.FooterRect = new Rect(this.ContentMargin.x, this.ContentRect.y + this.ContentSize.y + 20f, this.ContentSize.x, this.FooterHeight);
			this.SingleButtonRect = new Rect(this.ContentSize.x / 2f - this.ButtonSize.x / 2f, this.FooterHeight / 2f - this.ButtonSize.y / 2f, this.ButtonSize.x, this.ButtonSize.y);
			this.CancelButtonRect = new Rect(0f, this.FooterHeight / 2f - this.ButtonSize.y / 2f, this.ButtonSize.x, this.ButtonSize.y);
			this.ConfirmButtonRect = new Rect(this.ContentSize.x - this.ButtonSize.x, this.FooterHeight / 2f - this.ButtonSize.y / 2f, this.ButtonSize.x, this.ButtonSize.y);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000026D8 File Offset: 0x000008D8
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(this.WindowSize.x, this.WindowSize.y);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026F8 File Offset: 0x000008F8
		public override void DoWindowContents(Rect inRect)
		{
			GUI.color = Color.white;
			if (this.HeaderLabel != null)
			{
				Text.Font = GameFont.Medium;
				Widgets.Label(this.HeaderRect, this.HeaderLabel);
			}
			Text.Font = GameFont.Small;
			GUI.BeginGroup(this.ContentRect);
			this.ScrollView.Begin(this.ScrollRect);
			float num = 0f;
			if (this.IncludeNone)
			{
				float num2 = Text.CalcHeight(Translator.Translate("EdB.PC.Common.NoOptionSelected"), this.ContentSize.x - 32f);
				if (num2 < 30f)
				{
					num2 = 30f;
				}
				this.NoneEnabledFunc();
				bool flag = this.NoneSelectedFunc();
				if (Widgets.RadioButtonLabeled(new Rect(0f, num, this.ContentSize.x - 32f, num2), Translator.Translate("EdB.PC.Common.NoOptionSelected"), flag, false))
				{
					this.SelectNoneAction();
				}
				num += num2;
				num += 2f;
			}
			Rect rect = new Rect(0f, num, this.ContentSize.x - 32f, 0f);
			foreach (T t in this.options)
			{
				string text = this.NameFunc(t);
				bool flag2 = this.SelectedFunc(t);
				bool flag3 = this.EnabledFunc == null || this.EnabledFunc(t);
				float num3 = Text.CalcHeight(text, this.ContentSize.x - 32f);
				if (num3 < 30f)
				{
					num3 = 30f;
				}
				rect.height = num3;
				Vector2 vector = Text.CalcSize(text);
				if (vector.x > this.ContentSize.x - 32f)
				{
					vector.x = this.ContentSize.x;
				}
				vector.y = num3;
				if (flag3)
				{
					GUI.color = Color.white;
					if (Widgets.RadioButtonLabeled(rect, text, flag2, false))
					{
						this.SelectAction(t);
					}
				}
				else
				{
					GUI.color = new Color(0.65f, 0.65f, 0.65f);
					Widgets.Label(new Rect(0f, num + 2f, this.ContentSize.x - 32f, num3), text);
					Texture2D textureRadioButtonOff = Textures.TextureRadioButtonOff;
					Vector2 vector2 = new Vector2(rect.x + rect.width - 24f, rect.y + rect.height / 2f - 12f);
					GUI.color = new Color(1f, 1f, 1f, 0.28f);
					GUI.DrawTexture(new Rect(vector2.x, vector2.y, 24f, 24f), textureRadioButtonOff);
					GUI.color = Color.white;
				}
				if (this.DescriptionFunc != null)
				{
					TooltipHandler.TipRegion(new Rect(rect.x, rect.y, vector.x, vector.y), this.DescriptionFunc(t));
				}
				num += num3;
				num += 2f;
				rect.y = num;
			}
			this.ScrollView.End(num);
			GUI.EndGroup();
			GUI.color = Color.white;
			GUI.BeginGroup(this.FooterRect);
			Rect rect2 = this.SingleButtonRect;
			if (this.CancelButtonLabel != null)
			{
				if (Widgets.ButtonText(this.CancelButtonRect, this.CancelButtonLabel, true, true, true, null))
				{
					this.Close(true);
				}
				rect2 = this.ConfirmButtonRect;
			}
			if (Widgets.ButtonText(rect2, this.ConfirmButtonLabel, true, true, true, null))
			{
				string text2 = this.ConfirmValidation();
				if (text2 != null)
				{
					Messages.Message(Translator.Translate(text2), MessageTypeDefOf.RejectInput, true);
				}
				else
				{
					this.Confirm();
				}
			}
			GUI.EndGroup();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002B1C File Offset: 0x00000D1C
		protected void Confirm()
		{
			this.confirmButtonClicked = true;
			this.Close(true);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002B2C File Offset: 0x00000D2C
		public override void PostClose()
		{
			if (this.ConfirmButtonLabel != null)
			{
				if (this.confirmButtonClicked && this.CloseAction != null)
				{
					this.CloseAction();
					return;
				}
			}
			else if (this.CloseAction != null)
			{
				this.CloseAction();
			}
		}

		// Token: 0x0400000A RID: 10
		protected Vector2 ContentMargin = new Vector2(10f, 18f);

		// Token: 0x0400000B RID: 11
		protected Vector2 WindowSize = new Vector2(440f, 584f);

		// Token: 0x0400000C RID: 12
		protected Vector2 ButtonSize = new Vector2(140f, 40f);

		// Token: 0x0400000D RID: 13
		protected float HeaderHeight = 32f;

		// Token: 0x0400000E RID: 14
		protected Vector2 ContentSize;

		// Token: 0x0400000F RID: 15
		protected float FooterHeight = 40f;

		// Token: 0x04000010 RID: 16
		protected Rect ContentRect;

		// Token: 0x04000011 RID: 17
		protected Rect ScrollRect;

		// Token: 0x04000012 RID: 18
		protected Rect FooterRect;

		// Token: 0x04000013 RID: 19
		protected Rect HeaderRect;

		// Token: 0x04000014 RID: 20
		protected Rect CancelButtonRect;

		// Token: 0x04000015 RID: 21
		protected Rect ConfirmButtonRect;

		// Token: 0x04000016 RID: 22
		protected Rect SingleButtonRect;

		// Token: 0x04000017 RID: 23
		protected ScrollViewVertical ScrollView = new ScrollViewVertical();

		// Token: 0x04000018 RID: 24
		protected IEnumerable<T> options;

		// Token: 0x04000019 RID: 25
		protected bool confirmButtonClicked;

		// Token: 0x0400001A RID: 26
		protected float WindowPadding = 18f;

		// Token: 0x0400001B RID: 27
		public bool IncludeNone;

		// Token: 0x0400001C RID: 28
		protected string headerLabel;

		// Token: 0x0400001D RID: 29
		public string ConfirmButtonLabel = Translator.Translate("RandomPlus.Dialog.Close");

		// Token: 0x0400001E RID: 30
		public string CancelButtonLabel;

		// Token: 0x0400001F RID: 31
		public Action Initialize = delegate
		{
		};

		// Token: 0x04000020 RID: 32
		public Func<T, string> NameFunc = (T t) => "";

		// Token: 0x04000021 RID: 33
		public Func<T, string> DescriptionFunc;

		// Token: 0x04000022 RID: 34
		public Func<T, bool> SelectedFunc = (T t) => false;

		// Token: 0x04000023 RID: 35
		public Func<T, bool> EnabledFunc = (T t) => true;

		// Token: 0x04000024 RID: 36
		public Func<string> ConfirmValidation = () => null;

		// Token: 0x04000025 RID: 37
		public Action<T> SelectAction = delegate(T t)
		{
		};

		// Token: 0x04000026 RID: 38
		public Action CloseAction = delegate
		{
		};

		// Token: 0x04000027 RID: 39
		public Func<bool> NoneEnabledFunc = () => true;

		// Token: 0x04000028 RID: 40
		public Func<bool> NoneSelectedFunc = () => false;

		// Token: 0x04000029 RID: 41
		public Action SelectNoneAction = delegate
		{
		};
	}
}
