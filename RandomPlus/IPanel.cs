using System;
using UnityEngine;

namespace RandomPlus
{
	// Token: 0x02000007 RID: 7
	public interface IPanel
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003A RID: 58
		Rect PanelRect { get; }

		// Token: 0x0600003B RID: 59
		void Resize(Rect rect);

		// Token: 0x0600003C RID: 60
		void Draw();
	}
}
