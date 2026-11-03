using System;
using UnityEngine;

namespace RandomPlus
{
	// Token: 0x0200000E RID: 14
	public static class Style
	{
		// Token: 0x0600007A RID: 122 RVA: 0x00005377 File Offset: 0x00003577
		public static void SetGUIColorForButton(Rect rect)
		{
			if (rect.Contains(Event.current.mousePosition))
			{
				GUI.color = Style.ColorButtonHighlight;
				return;
			}
			GUI.color = Style.ColorButton;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000053A1 File Offset: 0x000035A1
		public static void SetGUIColorForButton(Rect rect, bool selected)
		{
			if (selected)
			{
				GUI.color = Style.ColorButtonSelected;
				return;
			}
			if (rect.Contains(Event.current.mousePosition))
			{
				GUI.color = Style.ColorButtonHighlight;
				return;
			}
			GUI.color = Style.ColorButton;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000053D9 File Offset: 0x000035D9
		public static void SetGUIColorForButton(Rect rect, bool selected, Color color, Color hoverColor, Color selectedColor)
		{
			if (selected)
			{
				GUI.color = selectedColor;
				return;
			}
			if (rect.Contains(Event.current.mousePosition))
			{
				GUI.color = hoverColor;
				return;
			}
			GUI.color = color;
		}

		// Token: 0x0400006B RID: 107
		public static Color ColorText = new Color(0.8f, 0.8f, 0.8f);

		// Token: 0x0400006C RID: 108
		public static Color ColorTextPanelHeader = new Color(0.8117647f, 0.8117647f, 0.8117647f);

		// Token: 0x0400006D RID: 109
		public static Color ColorPanelBackground = new Color(0.14117648f, 0.14509805f, 0.14901961f);

		// Token: 0x0400006E RID: 110
		public static Color ColorPanelBackgroundDeep = new Color(0.09411765f, 0.09411765f, 0.11372549f);

		// Token: 0x0400006F RID: 111
		public static Color ColorPanelBackgroundItem = new Color(0.16862746f, 0.17254902f, 0.1764706f);

		// Token: 0x04000070 RID: 112
		public static Color ColorPanelBackgroundScrollView = new Color(0.11764706f, 0.12156863f, 0.1254902f);

		// Token: 0x04000071 RID: 113
		public static Color ColorButton = new Color(0.623529f, 0.623529f, 0.623529f);

		// Token: 0x04000072 RID: 114
		public static Color ColorButtonHighlight = new Color(0.97647f, 0.97647f, 0.97647f);

		// Token: 0x04000073 RID: 115
		public static Color ColorButtonDisabled = new Color(0.27647f, 0.27647f, 0.27647f);

		// Token: 0x04000074 RID: 116
		public static Color ColorButtonSelected = new Color(1f, 1f, 1f);

		// Token: 0x04000075 RID: 117
		public static Color ColorControlDisabled = new Color(1f, 1f, 1f, 0.27647f);

		// Token: 0x04000076 RID: 118
		public static Color ColorTableHeader = new Color(0.11764706f, 0.12156863f, 0.1254902f);

		// Token: 0x04000077 RID: 119
		public static Color ColorTableHeaderBorder = new Color(0.24705882f, 0.2509804f, 0.25490198f);

		// Token: 0x04000078 RID: 120
		public static Color ColorTableRow1 = new Color(0.18431373f, 0.19215687f, 0.19607843f);

		// Token: 0x04000079 RID: 121
		public static Color ColorTableRow2 = new Color(0.21176471f, 0.21960784f, 0.22352941f);

		// Token: 0x0400007A RID: 122
		public static Color ColorTableRowSelected = new Color(0.047058824f, 0.047058824f, 0.047058824f);

		// Token: 0x0400007B RID: 123
		public static Color ColorWindowBackground = new Color(0.08235294f, 0.09803922f, 0.11372549f);

		// Token: 0x0400007C RID: 124
		public static Vector2 SizePanelMargin = new Vector2(12f, 12f);

		// Token: 0x0400007D RID: 125
		public static Vector2 SizePanelPadding = new Vector2(12f, 12f);

		// Token: 0x0400007E RID: 126
		public static Vector2 SizeTextFieldArrowMargin = new Vector2(4f, 0f);
	}
}
