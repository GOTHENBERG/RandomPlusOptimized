using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x0200000B RID: 11
	public class ProviderTraits
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00004A02 File Offset: 0x00002C02
		public static List<Trait> Traits
		{
			get
			{
				if (ProviderTraits._instance == null)
				{
					ProviderTraits._instance = new ProviderTraits();
				}
				return ProviderTraits._instance.sortedTraits;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004A20 File Offset: 0x00002C20
		private ProviderTraits()
		{
			foreach (TraitDef traitDef in DefDatabase<TraitDef>.AllDefs)
			{
				foreach (Trait trait in traitDef.ToTraits())
				{
					this.traits.Add(trait);
				}
			}
			this.sortedTraits = new List<Trait>(this.traits);
			this.sortedTraits.Sort((Trait t1, Trait t2) => t1.LabelCap.CompareTo(t2.LabelCap));
		}

		// Token: 0x04000055 RID: 85
		private static ProviderTraits _instance;

		// Token: 0x04000056 RID: 86
		private List<Trait> traits = new List<Trait>();

		// Token: 0x04000057 RID: 87
		private List<Trait> sortedTraits = new List<Trait>();
	}
}
