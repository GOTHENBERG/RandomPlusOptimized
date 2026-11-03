using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RandomPlus
{
	// Token: 0x02000006 RID: 6
	public class Field
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002D69 File Offset: 0x00000F69
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002D71 File Offset: 0x00000F71
		public Rect Rect
		{
			get
			{
				return this.rect;
			}
			set
			{
				this.rect = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002D7A File Offset: 0x00000F7A
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002D82 File Offset: 0x00000F82
		public Rect? ClickRect { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002D8B File Offset: 0x00000F8B
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002D93 File Offset: 0x00000F93
		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002D9C File Offset: 0x00000F9C
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002DA4 File Offset: 0x00000FA4
		public string Tip
		{
			get
			{
				return this.tip;
			}
			set
			{
				this.tip = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002DAD File Offset: 0x00000FAD
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002DB5 File Offset: 0x00000FB5
		public Action ClickAction
		{
			get
			{
				return this.clickAction;
			}
			set
			{
				this.clickAction = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002DBE File Offset: 0x00000FBE
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002DC6 File Offset: 0x00000FC6
		public Action PreviousAction
		{
			get
			{
				return this.previousAction;
			}
			set
			{
				this.previousAction = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002DCF File Offset: 0x00000FCF
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002DD7 File Offset: 0x00000FD7
		public Action NextAction
		{
			get
			{
				return this.nextAction;
			}
			set
			{
				this.nextAction = value;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002DE0 File Offset: 0x00000FE0
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002DE8 File Offset: 0x00000FE8
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002DF1 File Offset: 0x00000FF1
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002DF9 File Offset: 0x00000FF9
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002E04 File Offset: 0x00001004
		public void Draw()
		{
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			GameFont font = Text.Font;
			try
			{
				Rect rect = this.rect;
				if (this.previousAction != null)
				{
					rect.x += 12f;
					rect.width -= 12f;
				}
				if (this.nextAction != null)
				{
					rect.width -= 12f;
				}
				if (!this.enabled)
				{
					GUI.color = Style.ColorControlDisabled;
					Widgets.DrawAtlas(rect, Textures.TextureFieldAtlas);
				}
				else
				{
					GUI.color = Color.white;
					Widgets.DrawAtlas(rect, Textures.TextureFieldAtlas);
					Text.Anchor = TextAnchor.MiddleCenter;
					Rect rect2 = new Rect(this.rect.x, this.rect.y + 1f, this.rect.width, this.rect.height);
					if (this.clickAction != null && rect.Contains(Event.current.mousePosition))
					{
						GUI.color = Color.white;
					}
					else
					{
						GUI.color = this.Color;
					}
					if (this.label != null)
					{
						Widgets.Label(rect2, this.label);
					}
					GUI.color = Color.white;
					if (this.tip != null)
					{
						TooltipHandler.TipRegion(rect, this.tip);
					}
					if (this.previousAction != null)
					{
						Rect rect3 = new Rect(rect.x - 17f, rect.MiddleY() - 8f, 16f, 16f);
						if (rect3.Contains(Event.current.mousePosition))
						{
							GUI.color = Style.ColorButtonHighlight;
						}
						else
						{
							GUI.color = Style.ColorButton;
						}
						GUI.DrawTexture(rect3, Textures.TextureButtonPrevious);
						if (Widgets.ButtonInvisible(rect3, false))
						{
							SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
							this.previousAction();
						}
					}
					if (this.nextAction != null)
					{
						Rect rect4 = new Rect(rect.xMax + 1f, rect.MiddleY() - 8f, 16f, 16f);
						if (rect4.Contains(Event.current.mousePosition))
						{
							GUI.color = Style.ColorButtonHighlight;
						}
						else
						{
							GUI.color = Style.ColorButton;
						}
						GUI.DrawTexture(rect4, Textures.TextureButtonNext);
						if (Widgets.ButtonInvisible(rect4, false))
						{
							SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
							this.nextAction();
						}
					}
					if (this.clickAction != null)
					{
						if (this.ClickRect == null)
						{
							if (Widgets.ButtonInvisible(rect, false))
							{
								this.clickAction();
							}
						}
						else if (Widgets.ButtonInvisible(this.ClickRect.Value, false))
						{
							this.clickAction();
						}
					}
				}
			}
			finally
			{
				Text.Anchor = anchor;
				GUI.color = color;
				Text.Font = font;
			}
		}

		// Token: 0x0400002A RID: 42
		private Rect rect;

		// Token: 0x0400002B RID: 43
		private string label;

		// Token: 0x0400002C RID: 44
		private string tip;

		// Token: 0x0400002D RID: 45
		private Action nextAction;

		// Token: 0x0400002E RID: 46
		private Action previousAction;

		// Token: 0x0400002F RID: 47
		private Action clickAction;

		// Token: 0x04000030 RID: 48
		private Color color = Style.ColorText;

		// Token: 0x04000031 RID: 49
		private bool enabled = true;
	}
}
