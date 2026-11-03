using System;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000003 RID: 3
	public class TraitContainer : IExposable
	{
		// Token: 0x0600000A RID: 10 RVA: 0x0000213F File Offset: 0x0000033F
		public TraitContainer()
		{
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002147 File Offset: 0x00000347
		public TraitContainer(Trait trait, Action OnChange)
		{
			this.trait = trait;
			this.OnChange = OnChange;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002160 File Offset: 0x00000360
		public void ExposeData()
		{
			LoadSaveMode mode = Scribe.mode;
			if (mode != LoadSaveMode.Saving)
			{
				if (mode == LoadSaveMode.LoadingVars)
				{
					Scribe_Values.Look<string>(ref this.traitDefName, "traitDef", null, false);
					Scribe_Values.Look<int>(ref this.traitDegree, "traitDegree", 0, false);
					this.trait = new Trait(DefDatabase<TraitDef>.GetNamed(this.traitDefName, true), this.traitDegree, true);
				}
			}
			else
			{
				this.traitDefName = this.trait.def.defName;
				this.traitDegree = this.trait.Degree;
				Scribe_Values.Look<string>(ref this.traitDefName, "traitDef", null, false);
				Scribe_Values.Look<int>(ref this.traitDegree, "traitDegree", 0, false);
			}
			Scribe_Values.Look<TraitContainer.TraitFilterType>(ref this.traitFilter, "filterType", TraitContainer.TraitFilterType.Required, false);
		}

		// Token: 0x04000005 RID: 5
		public Trait trait;

		// Token: 0x04000006 RID: 6
		private string traitDefName;

		// Token: 0x04000007 RID: 7
		private int traitDegree;

		// Token: 0x04000008 RID: 8
		public TraitContainer.TraitFilterType traitFilter;

		// Token: 0x04000009 RID: 9
		private Action OnChange;

		// Token: 0x0200001C RID: 28
		public enum TraitFilterType : byte
		{
			// Token: 0x040000F1 RID: 241
			Required,
			// Token: 0x040000F2 RID: 242
			Optional,
			// Token: 0x040000F3 RID: 243
			Excluded
		}
	}
}
