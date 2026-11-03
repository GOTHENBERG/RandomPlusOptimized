using System;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000008 RID: 8
	public class PanelBase : IPanel
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000030E4 File Offset: 0x000012E4
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000030EC File Offset: 0x000012EC
		public Rect PanelRect { get; protected set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000030F5 File Offset: 0x000012F5
		// (set) Token: 0x06000040 RID: 64 RVA: 0x000030FD File Offset: 0x000012FD
		public Rect BodyRect { get; protected set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003106 File Offset: 0x00001306
		public virtual Color ColorPanelBackground
		{
			get
			{
				return Style.ColorPanelBackground;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000042 RID: 66 RVA: 0x0000310D File Offset: 0x0000130D
		public virtual string PanelHeader
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003110 File Offset: 0x00001310
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00003118 File Offset: 0x00001318
		public string Warning { get; set; }

		// Token: 0x06000046 RID: 70 RVA: 0x0000312C File Offset: 0x0000132C
		public virtual void Resize(Rect rect)
		{
			this.PanelRect = rect;
			this.BodyRect = new Rect(0f, 0f, rect.width, rect.height);
			if (this.PanelHeader != null)
			{
				this.BodyRect = new Rect(0f, 36f, rect.width, rect.height - 36f);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003194 File Offset: 0x00001394
		public virtual void Draw()
		{
			this.DrawPanelBackground();
			this.DrawPanelHeader();
			GUI.BeginGroup(this.PanelRect);
			try
			{
				this.DrawPanelContent();
			}
			finally
			{
				GUI.EndGroup();
			}
			GUI.color = Color.white;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000031E0 File Offset: 0x000013E0
		protected virtual void DrawPanelBackground()
		{
			GUI.color = this.ColorPanelBackground;
			GUI.DrawTexture(this.PanelRect, BaseContent.WhiteTex);
			GUI.color = Color.white;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003208 File Offset: 0x00001408
		protected virtual void DrawPanelHeader()
		{
			if (this.PanelHeader == null)
			{
				return;
			}
			Rect rect = new Rect(15f + this.PanelRect.xMin, 5f + this.PanelRect.yMin, this.PanelRect.width - 30f, 40f);
			if (!string.IsNullOrEmpty(this.Warning))
			{
				Rect rect2 = new Rect(8f + this.PanelRect.xMin, 9f + this.PanelRect.yMin, 20f, 20f);
				GUI.DrawTexture(rect2, Textures.TextureAlertSmall);
				TooltipHandler.TipRegion(rect2, this.Warning);
			}
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(rect, this.PanelHeader);
			Text.Font = font;
			Text.Anchor = anchor;
			GUI.color = color;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000032FF File Offset: 0x000014FF
		protected virtual void DrawPanelContent()
		{
			GUI.color = Style.ColorTextPanelHeader;
			GUI.color = Color.white;
		}
	}
}
