using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace RandomPlus
{
	// Token: 0x02000019 RID: 25
	public class RandomSettings
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00006D22 File Offset: 0x00004F22
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00006D29 File Offset: 0x00004F29
		public static PawnFilter PawnFilter
		{
			get
			{
				return RandomSettings.pawnFilter;
			}
			set
			{
				RandomSettings.pawnFilter = value;
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006D31 File Offset: 0x00004F31
		public static int RandomRerollCounter()
		{
			return RandomSettings.randomRerollCounter;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006D38 File Offset: 0x00004F38
		public static void Init()
		{
			RandomSettings.pawnFilter = new PawnFilter();
			try
			{
				RandomSettings.randomAgeMethodInfo = typeof(PawnGenerator).GetMethod("GenerateRandomAge", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.randomTraitMethodInfo = typeof(PawnGenerator).GetMethod("GenerateTraits", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.randomSkillMethodInfo = typeof(PawnGenerator).GetMethod("GenerateSkills", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.randomHealthMethodInfo = typeof(PawnGenerator).GetMethod("GenerateInitialHediffs", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.randomBodyTypeMethodInfo = typeof(PawnGenerator).GetMethod("GenerateBodyType", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.randomGeneMethodInfo = typeof(PawnGenerator).GetMethod("GenerateGenes", BindingFlags.Static | BindingFlags.NonPublic);
				RandomSettings.startingAndOptionalPawnsPropertyInfo = typeof(StartingPawnUtility).GetProperty("StartingAndOptionalPawns", BindingFlags.Static | BindingFlags.NonPublic);
			}
			catch (Exception ex)
			{
				Log.Error("RandomPlus: Failed to initialize reflection methods: " + ex.Message);
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006E38 File Offset: 0x00005038
		public static void ResetRerollCounter()
		{
			RandomSettings.randomRerollCounter = 0;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006E40 File Offset: 0x00005040
		public static void Reroll(int pawnIndex)
		{
			Pawn pawn = ((List<Pawn>)RandomSettings.startingAndOptionalPawnsPropertyInfo.GetValue(null))[pawnIndex];
			SpouseRelationUtility.Notify_PawnRegenerated(pawn);
			pawn = StartingPawnUtility.RandomizeInPlace(pawn);
			RandomSettings.randomRerollCounter++;
			if (RandomSettings.CheckPawnIsSatisfied(pawn))
			{
				return;
			}
			if (RandomSettings.PawnFilter.RerollAlgorithm == PawnFilter.RerollAlgorithmOptions.Normal || Find.WindowStack.currentlyDrawnWindow is Dialog_ChooseNewWanderers)
			{
				while (!RandomSettings.CheckPawnIsSatisfied(pawn))
				{
					SpouseRelationUtility.Notify_PawnRegenerated(pawn);
					pawn = StartingPawnUtility.RandomizeInPlace(pawn);
					RandomSettings.randomRerollCounter++;
				}
				return;
			}
			PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(StartingPawnUtility.PawnIndex(pawn));
			generationRequest.ValidateAndFix();
			Faction faction2;
			Faction faction = generationRequest.Faction ?? ((!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out faction2, false, true, TechLevel.Undefined, TechLevel.Undefined, false, false)) ? Faction.OfAncients : faction2);
			XenotypeDef xenotypeDef = (ModsConfig.BiotechActive ? PawnGenerator.GetXenotypeForGeneratedPawn(generationRequest) : null);
			while (RandomSettings.randomRerollCounter < RandomSettings.PawnFilter.RerollLimit)
			{
				try
				{
					RandomSettings.randomRerollCounter++;
					PawnGenerator.RedressPawn(pawn, generationRequest);
					pawn.ageTracker = new Pawn_AgeTracker(pawn);
					if (RandomSettings.randomAgeMethodInfo != null)
					{
						RandomSettings.randomAgeMethodInfo.Invoke(null, new object[] { pawn, generationRequest });
					}
					else
					{
						pawn.ageTracker.AgeBiologicalTicks = (long)Rand.Range(16, 65) * 3600000L;
						pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
					}
					if (RandomSettings.CheckAgeIsSatisfied(pawn))
					{
						pawn.story.traits = new TraitSet(pawn);
						pawn.skills = new Pawn_SkillTracker(pawn);
						PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, faction.def, generationRequest, xenotypeDef);
						MethodInfo methodInfo = RandomSettings.randomTraitMethodInfo;
						if (methodInfo != null)
						{
							methodInfo.Invoke(null, new object[] { pawn, generationRequest });
						}
						MethodInfo methodInfo2 = RandomSettings.randomSkillMethodInfo;
						if (methodInfo2 != null)
						{
							methodInfo2.Invoke(null, new object[] { pawn, generationRequest });
						}
						if (RandomSettings.CheckSkillsIsSatisfied(pawn) && RandomSettings.CheckTraitsIsSatisfied(pawn))
						{
							bool flag = false;
							int num = 0;
							while (num < 100 && !flag)
							{
								pawn.health.Reset();
								try
								{
									Find.Scenario.Notify_NewPawnGenerating(pawn, generationRequest.Context);
									MethodInfo methodInfo3 = RandomSettings.randomHealthMethodInfo;
									if (methodInfo3 != null)
									{
										methodInfo3.Invoke(null, new object[] { pawn, generationRequest });
									}
									if (!pawn.Dead && !pawn.Destroyed && !pawn.Downed)
									{
										flag = true;
									}
								}
								catch (Exception ex)
								{
									Log.Warning(string.Format("RandomPlus: Health generation failed on attempt {0}: {1}", num, ex.Message));
								}
								num++;
							}
							if (RandomSettings.CheckHealthIsSatisfied(pawn))
							{
								Pawn_WorkSettings workSettings = pawn.workSettings;
								if (workSettings != null)
								{
									workSettings.EnableAndInitialize();
								}
								if (RandomSettings.CheckWorkIsSatisfied(pawn))
								{
									Find.Scenario.Notify_PawnGenerated(pawn, generationRequest.Context, true);
									if (RandomSettings.CheckPawnIsSatisfied(pawn))
									{
										if (ModsConfig.BiotechActive)
										{
											pawn.genes = new Pawn_GeneTracker(pawn);
											MethodInfo methodInfo4 = RandomSettings.randomGeneMethodInfo;
											if (methodInfo4 != null)
											{
												methodInfo4.Invoke(null, new object[] { pawn, xenotypeDef, generationRequest });
											}
										}
										MethodInfo methodInfo5 = RandomSettings.randomBodyTypeMethodInfo;
										if (methodInfo5 != null)
										{
											methodInfo5.Invoke(null, new object[] { pawn, generationRequest });
										}
										RandomSettings.GeneratePawnStyle(pawn);
										break;
									}
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					Log.Warning(string.Format("RandomPlus: Error during pawn generation (attempt {0}): {1}", RandomSettings.randomRerollCounter, ex2.Message));
					try
					{
						Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);
						SpouseRelationUtility.Notify_PawnRegenerated(pawn);
						pawn = StartingPawnUtility.RandomizeInPlace(pawn);
					}
					catch (Exception ex3)
					{
						Log.Error("RandomPlus: Critical error in pawn cleanup: " + ex3.Message);
						break;
					}
				}
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000723C File Offset: 0x0000543C
		public static bool CheckPawnIsSatisfied(Pawn pawn)
		{
			return RandomSettings.RandomRerollCounter() >= RandomSettings.PawnFilter.RerollLimit || (RandomSettings.CheckGenderIsSatisfied(pawn) && RandomSettings.CheckSkillsIsSatisfied(pawn) && RandomSettings.CheckTraitsIsSatisfied(pawn) && RandomSettings.CheckHealthIsSatisfied(pawn) && RandomSettings.CheckWorkIsSatisfied(pawn) && RandomSettings.CheckAgeIsSatisfied(pawn));
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000729C File Offset: 0x0000549C
		public static bool CheckAgeIsSatisfied(Pawn pawn)
		{
			return (RandomSettings.pawnFilter.ageRange.min == PawnFilter.MinAgeDefault && RandomSettings.pawnFilter.ageRange.max == PawnFilter.MaxAgeDefault) || (RandomSettings.pawnFilter.ageRange.min <= pawn.ageTracker.AgeBiologicalYears && (RandomSettings.pawnFilter.ageRange.max == PawnFilter.MaxAgeDefault || RandomSettings.pawnFilter.ageRange.max >= pawn.ageTracker.AgeBiologicalYears));
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00007326 File Offset: 0x00005526
		public static bool CheckGenderIsSatisfied(Pawn pawn)
		{
			return RandomSettings.pawnFilter.Gender == Gender.None || pawn.gender == Gender.None || RandomSettings.pawnFilter.Gender == pawn.gender;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00007354 File Offset: 0x00005554
		public static bool CheckSkillsIsSatisfied(Pawn pawn)
		{
			List<SkillRecord> skills = pawn.skills.skills;
			using (IEnumerator<SkillContainer> enumerator = RandomSettings.pawnFilter.Skills.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillContainer skillFilter = enumerator.Current;
					if (skillFilter.Passion != Passion.None || skillFilter.MinValue > 0)
					{
						SkillRecord skillRecord = GenCollection.FirstOrDefault<SkillRecord>(skills, (SkillRecord i) => i.def == skillFilter.SkillDef);
						if (skillRecord != null)
						{
							if (skillRecord.passion < skillFilter.Passion || skillRecord.Level < skillFilter.MinValue)
							{
								return false;
							}
						}
						else
						{
							Log.Error("RandomPlus: Skill record not found - this shouldn't happen!");
						}
					}
				}
			}
			if (RandomSettings.pawnFilter.passionRange.min > PawnFilter.PassionMinDefault || RandomSettings.pawnFilter.passionRange.max < PawnFilter.PassionMaxDefault)
			{
				int num = skills.Where<SkillRecord>((SkillRecord skill) => skill.passion > Passion.None).Count<SkillRecord>();
				if (num < RandomSettings.pawnFilter.passionRange.min || num > RandomSettings.pawnFilter.passionRange.max)
				{
					return false;
				}
			}
			if (RandomSettings.pawnFilter.skillRange.min != PawnFilter.SkillMinDefault || RandomSettings.pawnFilter.skillRange.max != PawnFilter.SkillMaxDefault)
			{
				int num2 = 0;
				for (int j = 0; j < skills.Count; j++)
				{
					SkillRecord skillRecord2 = skills[j];
					if (RandomSettings.PawnFilter.countOnlyHighestAttack && j == 0)
					{
						SkillRecord skillRecord3 = skills[1];
						num2 += ((skillRecord3.Level > skillRecord2.Level) ? skillRecord3.Level : skillRecord2.Level);
						j = 1;
					}
					else if (RandomSettings.PawnFilter.countOnlyPassion)
					{
						if (skillRecord2.passion > 0)
						{
							num2 += skillRecord2.Level;
						}
					}
					else
					{
						num2 += skillRecord2.Level;
					}
				}
				if (RandomSettings.pawnFilter.skillRange.min > num2 || RandomSettings.pawnFilter.skillRange.max < num2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007598 File Offset: 0x00005798
		public static bool CheckTraitsIsSatisfied(Pawn pawn)
		{
			if (Page_RandomEditor.MOD_WELL_MET)
			{
				return true;
			}
			foreach (TraitContainer traitContainer in RandomSettings.pawnFilter.Traits)
			{
				bool flag = RandomSettings.HasTrait(pawn, traitContainer.trait);
				TraitContainer.TraitFilterType traitFilter = traitContainer.traitFilter;
				if (traitFilter != TraitContainer.TraitFilterType.Required)
				{
					if (traitFilter == TraitContainer.TraitFilterType.Excluded)
					{
						if (flag)
						{
							return false;
						}
					}
				}
				else if (!flag)
				{
					return false;
				}
			}
			if (RandomSettings.pawnFilter.RequiredTraitsInPool > 0 && RandomSettings.pawnFilter.RequiredTraitsInPool <= RandomSettings.pawnFilter.Traits.Count<TraitContainer>())
			{
				int num = 0;
				foreach (TraitContainer traitContainer2 in RandomSettings.pawnFilter.Traits.Where<TraitContainer>((TraitContainer t) => t.traitFilter == TraitContainer.TraitFilterType.Optional))
				{
					if (RandomSettings.HasTrait(pawn, traitContainer2.trait))
					{
						num++;
						if (RandomSettings.pawnFilter.RequiredTraitsInPool == num)
						{
							break;
						}
					}
				}
				if (num < RandomSettings.pawnFilter.RequiredTraitsInPool)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000076E8 File Offset: 0x000058E8
		private static bool IsGeneAffectedHealth(Hediff hediff)
		{
			if (!ModsConfig.BiotechActive)
			{
				return false;
			}
			Hediff_ChemicalDependency hediff_ChemicalDependency = hediff as Hediff_ChemicalDependency;
			return hediff_ChemicalDependency != null && hediff_ChemicalDependency.LinkedGene != null;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007714 File Offset: 0x00005914
		public static bool CheckHealthIsSatisfied(Pawn pawn)
		{
			switch (RandomSettings.pawnFilter.FilterHealthCondition)
			{
			case PawnFilter.HealthOptions.OnlyStartCondition:
				if (GenCollection.FirstOrDefault<Hediff>(pawn.health.hediffSet.hediffs, (Hediff hediff) => hediff.def.defName != "CryptosleepSickness" && hediff.def.defName != "Malnutrition" && !RandomSettings.IsGeneAffectedHealth(hediff)) != null)
				{
					return false;
				}
				break;
			case PawnFilter.HealthOptions.NoPain:
				if (GenCollection.FirstOrDefault<Hediff>(pawn.health.hediffSet.hediffs, (Hediff hediff) => hediff.PainOffset > 0f && !RandomSettings.IsGeneAffectedHealth(hediff)) != null)
				{
					return false;
				}
				break;
			case PawnFilter.HealthOptions.NoAddiction:
				if (GenCollection.FirstOrDefault<Hediff>(pawn.health.hediffSet.hediffs, (Hediff hediff) => hediff is Hediff_Addiction && !RandomSettings.IsGeneAffectedHealth(hediff)) != null)
				{
					return false;
				}
				break;
			case PawnFilter.HealthOptions.AllowNone:
				if (ModsConfig.BiotechActive)
				{
					if (pawn.health.hediffSet.hediffs.Where<Hediff>((Hediff i) => !RandomSettings.IsGeneAffectedHealth(i)).Count<Hediff>() > 0)
					{
						return false;
					}
				}
				else if (pawn.health.hediffSet.hediffs.Count > 0)
				{
					return false;
				}
				break;
			}
			return true;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000785C File Offset: 0x00005A5C
		public static bool CheckWorkIsSatisfied(Pawn pawn)
		{
			switch (RandomSettings.pawnFilter.FilterIncapable)
			{
			case PawnFilter.IncapableOptions.NoDumbLabor:
				if ((pawn.story.DisabledWorkTagsBackstoryAndTraits & WorkTags.ManualDumb) == WorkTags.ManualDumb)
				{
					return false;
				}
				break;
			case PawnFilter.IncapableOptions.AllowNone:
				if (pawn.story.DisabledWorkTagsBackstoryAndTraits != WorkTags.None)
				{
					return false;
				}
				break;
			}
			return true;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000078AC File Offset: 0x00005AAC
		public static bool HasTrait(Pawn pawn, Trait trait)
		{
			return pawn.story.traits.allTraits.Find((Trait t) => (t == null && trait == null) || (trait != null && t != null && trait.Label.Equals(t.Label))) != null;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000078EA File Offset: 0x00005AEA
		public static void SetGenderFilter(Gender gender)
		{
			RandomSettings.pawnFilter.Gender = gender;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000078F8 File Offset: 0x00005AF8
		private static float GetTotalTraitCommonality(Gender gender)
		{
			if (gender == Gender.Male && RandomSettings._cacheTraitCommonalityMale > 0f)
			{
				return RandomSettings._cacheTraitCommonalityMale;
			}
			if (gender == Gender.Female && RandomSettings._cacheTraitCommonalityFemale > 0f)
			{
				return RandomSettings._cacheTraitCommonalityFemale;
			}
			float num = 0f;
			foreach (TraitDef traitDef in DefDatabase<TraitDef>.AllDefsListForReading)
			{
				num += traitDef.GetGenderSpecificCommonality(gender);
			}
			if (gender == Gender.Male)
			{
				RandomSettings._cacheTraitCommonalityMale = num;
			}
			if (gender == Gender.Female)
			{
				RandomSettings._cacheTraitCommonalityFemale = num;
			}
			return num;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007994 File Offset: 0x00005B94
		public static float GetTraitRollChance(TraitDef traitDef, Gender gender = Gender.Male)
		{
			float totalTraitCommonality = RandomSettings.GetTotalTraitCommonality(gender);
			return traitDef.GetGenderSpecificCommonality(gender) * 100f / totalTraitCommonality;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000079B8 File Offset: 0x00005BB8
		public static string GetTraitRollChanceText(TraitDef traitDef)
		{
			string text = RandomSettings.GetTraitRollChance(traitDef, Gender.Male).ToString("0.0");
			string text2 = RandomSettings.GetTraitRollChance(traitDef, Gender.Female).ToString("0.0");
			if (traitDef.GetGenderSpecificCommonality(Gender.Male) == traitDef.GetGenderSpecificCommonality(Gender.Female))
			{
				return "(" + text + "%)";
			}
			return string.Concat(new string[] { "(♂:", text, "%,♀:", text2, "%)" });
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007A3C File Offset: 0x00005C3C
		public static void GeneratePawnStyle(Pawn pawn)
		{
			if (pawn.RaceProps.Humanlike)
			{
				try
				{
					pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
					if (pawn.style != null)
					{
						pawn.style.beardDef = ((pawn.gender == Gender.Male) ? PawnStyleItemChooser.RandomBeardFor(pawn) : BeardDefOf.NoBeard);
						if (ModsConfig.IdeologyActive)
						{
							pawn.style.FaceTattoo = PawnStyleItemChooser.RandomTattooFor(pawn, TattooType.Face);
							pawn.style.BodyTattoo = PawnStyleItemChooser.RandomTattooFor(pawn, TattooType.Body);
						}
						else
						{
							pawn.style.SetupTattoos_NoIdeology();
						}
					}
				}
				catch (Exception ex)
				{
					Log.Warning("RandomPlus: Failed to generate pawn style: " + ex.Message);
				}
			}
		}

		// Token: 0x040000E1 RID: 225
		private static MethodInfo randomAgeMethodInfo;

		// Token: 0x040000E2 RID: 226
		private static MethodInfo randomTraitMethodInfo;

		// Token: 0x040000E3 RID: 227
		private static MethodInfo randomSkillMethodInfo;

		// Token: 0x040000E4 RID: 228
		private static MethodInfo randomHealthMethodInfo;

		// Token: 0x040000E5 RID: 229
		private static MethodInfo randomBodyTypeMethodInfo;

		// Token: 0x040000E6 RID: 230
		private static MethodInfo randomGeneMethodInfo;

		// Token: 0x040000E7 RID: 231
		private static PropertyInfo startingAndOptionalPawnsPropertyInfo;

		// Token: 0x040000E8 RID: 232
		public static int MinSkillRange;

		// Token: 0x040000E9 RID: 233
		public static int randomRerollCounter = 0;

		// Token: 0x040000EA RID: 234
		public static List<PawnFilter> pawnFilterList = new List<PawnFilter>();

		// Token: 0x040000EB RID: 235
		private static PawnFilter pawnFilter;

		// Token: 0x040000EC RID: 236
		private static float _cacheTraitCommonalityMale;

		// Token: 0x040000ED RID: 237
		private static float _cacheTraitCommonalityFemale;
	}
}
