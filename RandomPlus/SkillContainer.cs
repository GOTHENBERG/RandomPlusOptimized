using System;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000002 RID: 2
	public class SkillContainer : IExposable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public SkillDef SkillDef
		{
			get
			{
				return this.skillDef;
			}
			set
			{
				this.skillDef = value;
				Action onChange = this.OnChange;
				if (onChange == null)
				{
					return;
				}
				onChange();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002071 File Offset: 0x00000271
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002079 File Offset: 0x00000279
		public Passion Passion
		{
			get
			{
				return this.passion;
			}
			set
			{
				this.passion = value;
				Action onChange = this.OnChange;
				if (onChange == null)
				{
					return;
				}
				onChange();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002092 File Offset: 0x00000292
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000209A File Offset: 0x0000029A
		public int MinValue
		{
			get
			{
				return this.minValue;
			}
			set
			{
				if (value > 20)
				{
					this.minValue = 20;
				}
				else if (value < 0)
				{
					this.minValue = 0;
				}
				else
				{
					this.minValue = value;
				}
				Action onChange = this.OnChange;
				if (onChange == null)
				{
					return;
				}
				onChange();
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020CF File Offset: 0x000002CF
		public SkillContainer()
		{
			this.passion = Passion.None;
			this.MinValue = 0;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020E5 File Offset: 0x000002E5
		public SkillContainer(SkillDef skillDef, Action OnChange)
		{
			this.skillDef = skillDef;
			this.passion = Passion.None;
			this.MinValue = 0;
			this.OnChange = OnChange;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002109 File Offset: 0x00000309
		public void ExposeData()
		{
			Scribe_Defs.Look<SkillDef>(ref this.skillDef, "skillDef");
			Scribe_Values.Look<Passion>(ref this.passion, "passion", Passion.None, false);
			Scribe_Values.Look<int>(ref this.minValue, "min_value", 0, false);
		}

		// Token: 0x04000001 RID: 1
		private SkillDef skillDef;

		// Token: 0x04000002 RID: 2
		private Passion passion;

		// Token: 0x04000003 RID: 3
		private int minValue;

		// Token: 0x04000004 RID: 4
		private Action OnChange;
	}
}
