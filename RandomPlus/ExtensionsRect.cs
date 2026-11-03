using System;
using UnityEngine;

namespace RandomPlus
{
	// Token: 0x02000005 RID: 5
	public static class ExtensionsRect
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002B65 File Offset: 0x00000D65
		public static float MiddleX(this Rect rect)
		{
			return rect.x + rect.width * 0.5f;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002B7C File Offset: 0x00000D7C
		public static float MiddleY(this Rect rect)
		{
			return rect.y + rect.height * 0.5f;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002B93 File Offset: 0x00000D93
		public static float HalfWidth(this Rect rect)
		{
			return rect.width * 0.5f;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002BA2 File Offset: 0x00000DA2
		public static float HalfHeight(this Rect rect)
		{
			return rect.height * 0.5f;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002BB1 File Offset: 0x00000DB1
		public static Rect OffsetBy(this Rect rect, Vector2 offset)
		{
			return new Rect(rect.position + offset, rect.size);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002BCC File Offset: 0x00000DCC
		public static Rect OffsetBy(this Rect rect, float x, float y)
		{
			return new Rect(rect.position + new Vector2(x, y), rect.size);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002BED File Offset: 0x00000DED
		public static Rect MoveTo(this Rect rect, Vector2 position)
		{
			return new Rect(position, rect.size);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002BFC File Offset: 0x00000DFC
		public static Rect MoveTo(this Rect rect, float x, float y)
		{
			return new Rect(new Vector2(x, y), rect.size);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002C11 File Offset: 0x00000E11
		public static Rect InsetBy(this Rect rect, float left, float top, float right, float bottom)
		{
			return new Rect(rect.x + left, rect.y + top, rect.width - left - right, rect.height - top - bottom);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002C44 File Offset: 0x00000E44
		public static Rect InsetBy(this Rect rect, Vector2 topLeft, Vector2 bottomRight)
		{
			return new Rect(rect.x + topLeft.x, rect.y + topLeft.y, rect.width - topLeft.x - bottomRight.x, rect.height - topLeft.y - bottomRight.y);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002C9C File Offset: 0x00000E9C
		public static Rect InsetBy(this Rect rect, float amount)
		{
			return rect.InsetBy(amount, amount, amount, amount);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002CA8 File Offset: 0x00000EA8
		public static Rect InsetBy(this Rect rect, Vector2 amount)
		{
			return rect.InsetBy(amount, amount);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002CB2 File Offset: 0x00000EB2
		public static Rect InsetBy(this Rect rect, float xAmount, float yAmount)
		{
			return rect.InsetBy(new Vector2(xAmount, yAmount), new Vector2(xAmount, yAmount));
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002CC8 File Offset: 0x00000EC8
		public static Rect Combined(this Rect rect, Rect other)
		{
			Vector2 vector = new Vector2(Mathf.Min(rect.xMin, other.xMin), Mathf.Min(rect.yMin, other.yMin));
			Vector2 vector2 = new Vector2(Mathf.Max(rect.xMax, other.xMax), Mathf.Max(rect.yMax, other.yMax));
			return new Rect(vector, vector2 - vector);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002D3C File Offset: 0x00000F3C
		public static bool Mouseover(this Rect rect)
		{
			return rect.Contains(Event.current.mousePosition);
		}
	}
}
