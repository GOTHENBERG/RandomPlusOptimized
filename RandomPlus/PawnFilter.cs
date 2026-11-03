using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000018 RID: 24
	public class PawnFilter : IExposable
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00006782 File Offset: 0x00004982
		public static string[] RerollAlgorithmOptionValues
		{
			get
			{
				if (ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
				{
					return new string[] { "RandomPlus.PanelOthers.RerollAlgorithmOptionValues.Normal" };
				}
				return PawnFilter._RerollAlgorithmOptionValues;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000067A4 File Offset: 0x000049A4
		public static PawnFilter.RerollAlgorithmOptions DefaultRerollAlgorithm
		{
			get
			{
				ModsConfig.IsActive("erdelf.HumanoidAlienRaces");
				return PawnFilter.RerollAlgorithmOptions.Normal;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000067B2 File Offset: 0x000049B2
		public IEnumerable<SkillContainer> Skills
		{
			get
			{
				return this.skills;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000067C2 File Offset: 0x000049C2
		public IEnumerable<TraitContainer> Traits
		{
			get
			{
				return this.traits;
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000067D2 File Offset: 0x000049D2
		public void AddTrait(Trait trait)
		{
			this.traits.Add(new TraitContainer(trait, new Action(this.OnChange)));
			this.OnChange();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000067F7 File Offset: 0x000049F7
		public void TraitUpdated(int index, Trait trait)
		{
			this.traits[index].trait = trait;
			this.OnChange();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006814 File Offset: 0x00004A14
		public void TraitRemoved(Trait trait)
		{
			TraitContainer traitContainer = GenCollection.FirstOrDefault<TraitContainer>(this.traits, (TraitContainer tc) => tc.trait == trait);
			this.traits.Remove(traitContainer);
			this.OnChange();
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00006859 File Offset: 0x00004A59
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00006861 File Offset: 0x00004A61
		public int RequiredTraitsInPool
		{
			get
			{
				return this._RequiredTraitsInPool;
			}
			set
			{
				this._RequiredTraitsInPool = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000686A File Offset: 0x00004A6A
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00006872 File Offset: 0x00004A72
		public Gender Gender
		{
			get
			{
				return this.gender;
			}
			set
			{
				this.gender = value;
				this.OnChange();
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00006881 File Offset: 0x00004A81
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00006889 File Offset: 0x00004A89
		public PawnFilter.RerollAlgorithmOptions RerollAlgorithm
		{
			get
			{
				return this.rerollAlgorithm;
			}
			set
			{
				this.rerollAlgorithm = value;
				this.OnChange();
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00006898 File Offset: 0x00004A98
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000068A0 File Offset: 0x00004AA0
		public int RerollLimit
		{
			get
			{
				return this.rerollLimit;
			}
			set
			{
				this.rerollLimit = value;
				this.OnChange();
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000068AF File Offset: 0x00004AAF
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000068B7 File Offset: 0x00004AB7
		public PawnFilter.HealthOptions FilterHealthCondition
		{
			get
			{
				return this.filterHealthCondition;
			}
			set
			{
				this.filterHealthCondition = value;
				this.OnChange();
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000068C6 File Offset: 0x00004AC6
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x000068CE File Offset: 0x00004ACE
		public PawnFilter.IncapableOptions FilterIncapable
		{
			get
			{
				return this.filterIncapable;
			}
			set
			{
				this.filterIncapable = value;
				this.OnChange();
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000068E0 File Offset: 0x00004AE0
		public PawnFilter()
		{
			this.ResetAll();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006930 File Offset: 0x00004B30
		public void ResetSkills()
		{
			this.skills.Clear();
			foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefs)
			{
				this.skills.Add(new SkillContainer(skillDef, new Action(this.OnChange)));
			}
			this.passionRange = new IntRange(PawnFilter.PassionMinDefault, PawnFilter.PassionMaxDefault);
			this.skillRange = new IntRange(PawnFilter.SkillMinDefault, PawnFilter.SkillMaxDefault);
			this.countOnlyHighestAttack = false;
			this.countOnlyPassion = false;
			this.OnChange();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000069DC File Offset: 0x00004BDC
		public void ResetTraits()
		{
			this.traits.Clear();
			this.RequiredTraitsInPool = PawnFilter.DefaultPoolSize;
			this.OnChange();
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000069FC File Offset: 0x00004BFC
		public void ResetOther()
		{
			this.RerollAlgorithm = PawnFilter.DefaultRerollAlgorithm;
			this.gender = Gender.None;
			this.rerollLimit = (int)PawnFilter.DefaultRerollLimit;
			this.filterHealthCondition = PawnFilter.HealthOptions.AllowAll;
			this.filterIncapable = PawnFilter.IncapableOptions.AllowAll;
			this.ageRange = new IntRange(PawnFilter.MinAgeDefault, PawnFilter.MaxAgeDefault);
			this.OnChange();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006A4F File Offset: 0x00004C4F
		public void ResetAll()
		{
			this.ResetSkills();
			this.ResetTraits();
			this.ResetOther();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00006A63 File Offset: 0x00004C63
		public void OnChange()
		{
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00006A68 File Offset: 0x00004C68
		public void ExposeData()
		{
			int num = 1;
			Scribe_Values.Look<string>(ref this.name, "name", "", false);
			Scribe_Values.Look<int>(ref num, "version", 1, false);
			Scribe_Collections.Look<SkillContainer>(ref this.skills, "skills", LookMode.Deep, null);
			Scribe_Collections.Look<TraitContainer>(ref this.traits, "traits", LookMode.Deep, null);
			Scribe_Values.Look<int>(ref this._RequiredTraitsInPool, "poolSize", PawnFilter.DefaultPoolSize, false);
			Scribe_Values.Look<int>(ref this.passionRange.min, "passionRangeMin", PawnFilter.PassionMinDefault, false);
			Scribe_Values.Look<int>(ref this.passionRange.max, "passionRangeMax", PawnFilter.PassionMaxDefault, false);
			Scribe_Values.Look<int>(ref this.skillRange.min, "skillRangeMin", PawnFilter.SkillMinDefault, false);
			Scribe_Values.Look<int>(ref this.skillRange.max, "skillRangeMax", PawnFilter.SkillMaxDefault, false);
			Scribe_Values.Look<bool>(ref this.countOnlyHighestAttack, "countOnlyHighestAttack", false, false);
			Scribe_Values.Look<bool>(ref this.countOnlyPassion, "countOnlyPassion", false, false);
			Scribe_Values.Look<int>(ref this.ageRange.min, "ageRangeMin", PawnFilter.MinAgeDefault, false);
			Scribe_Values.Look<int>(ref this.ageRange.max, "ageRangeMax", PawnFilter.MaxAgeDefault, false);
			Scribe_Values.Look<PawnFilter.RerollAlgorithmOptions>(ref this.rerollAlgorithm, "rerollAlgorithm", PawnFilter.DefaultRerollAlgorithm, false);
			Scribe_Values.Look<int>(ref this.rerollLimit, "rerollLimit", (int)PawnFilter.DefaultRerollLimit, false);
			Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
			Scribe_Values.Look<PawnFilter.HealthOptions>(ref this.filterHealthCondition, "healthCondition", PawnFilter.HealthOptions.AllowAll, false);
			Scribe_Values.Look<PawnFilter.IncapableOptions>(ref this.filterIncapable, "incapable", PawnFilter.IncapableOptions.AllowAll, false);
			if (Scribe.mode != LoadSaveMode.Saving)
			{
			}
		}

		// Token: 0x040000C7 RID: 199
		public static readonly int PassionMinDefault = 0;

		// Token: 0x040000C8 RID: 200
		public static readonly int PassionMaxDefault = DefDatabase<SkillDef>.AllDefs.ToArray<SkillDef>().Length;

		// Token: 0x040000C9 RID: 201
		public static readonly int SkillMinDefault = 0;

		// Token: 0x040000CA RID: 202
		public static readonly int SkillMaxDefault = DefDatabase<SkillDef>.AllDefs.ToArray<SkillDef>().Length * 8;

		// Token: 0x040000CB RID: 203
		public static readonly int MinAgeDefault = 0;

		// Token: 0x040000CC RID: 204
		public static readonly int MaxAgeDefault = 120;

		// Token: 0x040000CD RID: 205
		public static readonly int DefaultPoolSize = 0;

		// Token: 0x040000CE RID: 206
		public static readonly string[] _RerollAlgorithmOptionValues = new string[] { "RandomPlus.PanelOthers.RerollAlgorithmOptionValues.Normal", "RandomPlus.PanelOthers.RerollAlgorithmOptionValues.Fast" };

		// Token: 0x040000CF RID: 207
		public static readonly string[] RerollLimitOptionValues = new string[] { "100", "250", "500", "1000", "2500", "5000", "10000", "50000" };

		// Token: 0x040000D0 RID: 208
		public static readonly PawnFilter.RerollLimitOptions DefaultRerollLimit = PawnFilter.RerollLimitOptions.N1000;

		// Token: 0x040000D1 RID: 209
		public static readonly string[] HealthOptionValues = new string[] { "RandomPlus.PanelOthers.HealthOptions.AllowAll", "RandomPlus.PanelOthers.HealthOptions.OnlyStartConditions", "RandomPlus.PanelOthers.HealthOptions.NoPain", "RandomPlus.PanelOthers.HealthOptions.NoAddiction", "RandomPlus.PanelOthers.HealthOptions.AllowNone" };

		// Token: 0x040000D2 RID: 210
		public static readonly string[] IncapableOptionValues = new string[] { "RandomPlus.PanelOthers.IncapableOptions.AllowAll", "RandomPlus.PanelOthers.IncapableOptions.NoDumbLabor", "RandomPlus.PanelOthers.IncapableOptions.AllowNone" };

		// Token: 0x040000D3 RID: 211
		public string name;

		// Token: 0x040000D4 RID: 212
		private List<SkillContainer> skills = new List<SkillContainer>();

		// Token: 0x040000D5 RID: 213
		private List<TraitContainer> traits = new List<TraitContainer>();

		// Token: 0x040000D6 RID: 214
		private int _RequiredTraitsInPool = PawnFilter.DefaultPoolSize;

		// Token: 0x040000D7 RID: 215
		public IntRange passionRange;

		// Token: 0x040000D8 RID: 216
		public IntRange skillRange;

		// Token: 0x040000D9 RID: 217
		public bool countOnlyHighestAttack;

		// Token: 0x040000DA RID: 218
		public bool countOnlyPassion;

		// Token: 0x040000DB RID: 219
		public IntRange ageRange;

		// Token: 0x040000DC RID: 220
		private Gender gender;

		// Token: 0x040000DD RID: 221
		private PawnFilter.RerollAlgorithmOptions rerollAlgorithm = PawnFilter.DefaultRerollAlgorithm;

		// Token: 0x040000DE RID: 222
		private int rerollLimit = (int)PawnFilter.DefaultRerollLimit;

		// Token: 0x040000DF RID: 223
		private PawnFilter.HealthOptions filterHealthCondition;

		// Token: 0x040000E0 RID: 224
		private PawnFilter.IncapableOptions filterIncapable;

		// Token: 0x0200002C RID: 44
		public enum RerollAlgorithmOptions
		{
			// Token: 0x04000123 RID: 291
			Normal,
			// Token: 0x04000124 RID: 292
			Fast
		}

		// Token: 0x0200002D RID: 45
		public enum RerollLimitOptions
		{
			// Token: 0x04000126 RID: 294
			N100 = 100,
			// Token: 0x04000127 RID: 295
			N250 = 250,
			// Token: 0x04000128 RID: 296
			N500 = 500,
			// Token: 0x04000129 RID: 297
			N1000 = 1000,
			// Token: 0x0400012A RID: 298
			N2500 = 2500,
			// Token: 0x0400012B RID: 299
			N5000 = 5000,
			// Token: 0x0400012C RID: 300
			N10000 = 10000,
			// Token: 0x0400012D RID: 301
			N50000 = 50000
		}

		// Token: 0x0200002E RID: 46
		public enum HealthOptions
		{
			// Token: 0x0400012F RID: 303
			AllowAll,
			// Token: 0x04000130 RID: 304
			OnlyStartCondition,
			// Token: 0x04000131 RID: 305
			NoPain,
			// Token: 0x04000132 RID: 306
			NoAddiction,
			// Token: 0x04000133 RID: 307
			AllowNone
		}

		// Token: 0x0200002F RID: 47
		public enum IncapableOptions
		{
			// Token: 0x04000135 RID: 309
			AllowAll,
			// Token: 0x04000136 RID: 310
			NoDumbLabor,
			// Token: 0x04000137 RID: 311
			AllowNone
		}
	}
}
