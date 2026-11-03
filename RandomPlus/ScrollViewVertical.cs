using System;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x0200000D RID: 13
	public class ScrollViewVertical
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00005121 File Offset: 0x00003321
		public float ViewHeight
		{
			get
			{
				return this.viewRect.height;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000512E File Offset: 0x0000332E
		public float ViewWidth
		{
			get
			{
				return this.viewRect.width;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000513B File Offset: 0x0000333B
		public float ContentWidth
		{
			get
			{
				return this.contentRect.width;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00005148 File Offset: 0x00003348
		public float ContentHeight
		{
			get
			{
				return this.contentHeight;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00005150 File Offset: 0x00003350
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00005158 File Offset: 0x00003358
		public Vector2 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00005161 File Offset: 0x00003361
		public bool ScrollbarsVisible
		{
			get
			{
				return this.ContentHeight > this.ViewHeight;
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00005171 File Offset: 0x00003371
		public ScrollViewVertical()
		{
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000518B File Offset: 0x0000338B
		public ScrollViewVertical(bool consumeScrollEvents)
		{
			this.consumeScrollEvents = consumeScrollEvents;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000051AC File Offset: 0x000033AC
		public void Begin(Rect viewRect)
		{
			this.viewRect = viewRect;
			this.contentRect = new Rect(0f, 0f, viewRect.width - 16f, this.contentHeight);
			if (this.consumeScrollEvents)
			{
				Widgets.BeginScrollView(viewRect, ref this.position, this.contentRect, true);
				return;
			}
			ScrollViewVertical.BeginScrollView(viewRect, ref this.position, this.contentRect);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005218 File Offset: 0x00003418
		public void End(float yPosition)
		{
			this.contentHeight = yPosition;
			Widgets.EndScrollView();
			if (this.scrollTo != null)
			{
				Vector2 value = this.scrollTo.Value;
				if (value.y < 0f)
				{
					value.y = 0f;
				}
				else if (value.y > this.ContentHeight - this.ViewHeight - 1f)
				{
					value.y = this.ContentHeight - this.ViewHeight - 1f;
				}
				this.Position = value;
				this.scrollTo = null;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000052AD File Offset: 0x000034AD
		public void ScrollToTop()
		{
			this.scrollTo = new Vector2?(new Vector2(0f, 0f));
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000052C9 File Offset: 0x000034C9
		public void ScrollToBottom()
		{
			this.scrollTo = new Vector2?(new Vector2(0f, float.MaxValue));
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000052E5 File Offset: 0x000034E5
		public void ScrollTo(float y)
		{
			this.scrollTo = new Vector2?(new Vector2(0f, y));
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005300 File Offset: 0x00003500
		protected static void BeginScrollView(Rect outRect, ref Vector2 scrollPosition, Rect viewRect)
		{
			Vector2 vector = scrollPosition;
			Vector2 vector2 = GUI.BeginScrollView(outRect, scrollPosition, viewRect);
			Vector2 vector3;
			if (Event.current.type == EventType.MouseDown)
			{
				vector3 = vector;
			}
			else
			{
				vector3 = vector2;
			}
			if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(outRect))
			{
				vector3 += Event.current.delta * 40f;
			}
			scrollPosition = vector3;
		}

		// Token: 0x04000064 RID: 100
		public static readonly float ScrollbarSize = 15f;

		// Token: 0x04000065 RID: 101
		private float contentHeight;

		// Token: 0x04000066 RID: 102
		private Vector2 position = Vector2.zero;

		// Token: 0x04000067 RID: 103
		private Rect viewRect;

		// Token: 0x04000068 RID: 104
		private Rect contentRect;

		// Token: 0x04000069 RID: 105
		private bool consumeScrollEvents = true;

		// Token: 0x0400006A RID: 106
		private Vector2? scrollTo;
	}
}
