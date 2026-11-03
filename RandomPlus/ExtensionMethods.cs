using System;
using System.Collections.Generic;
using RimWorld;

namespace RandomPlus
{
	// Token: 0x02000010 RID: 16
	public static class ExtensionMethods
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00005936 File Offset: 0x00003B36
		public static IEnumerable<Trait> ToTraits(this TraitDef traitDef)
		{
			List<TraitDegreeData> degreeData = traitDef.degreeDatas;
			int count = degreeData.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					Trait trait = new Trait(traitDef, degreeData[i].degree, true);
					yield return trait;
				}
			}
			else
			{
				yield return new Trait(traitDef, 0, true);
			}
			yield break;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005946 File Offset: 0x00003B46
		public static bool ContainsTrait(this IEnumerable<TraitContainer> traits, Trait trait)
		{
			return true;
		}
	}
}
