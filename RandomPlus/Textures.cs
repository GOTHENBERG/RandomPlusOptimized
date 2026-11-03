using System;
using UnityEngine;
using Verse;

namespace RandomPlus
{
	// Token: 0x0200000F RID: 15
	[StaticConstructorOnStartup]
	public static class Textures
	{
		// Token: 0x0600007E RID: 126 RVA: 0x000055FF File Offset: 0x000037FF
		static Textures()
		{
			Textures.LoadTextures();
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00005606 File Offset: 0x00003806
		public static bool Loaded
		{
			get
			{
				return Textures.loaded;
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000560D File Offset: 0x0000380D
		public static void Reset()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				Textures.LoadTextures();
			});
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005634 File Offset: 0x00003834
		private static void LoadTextures()
		{
			Textures.loaded = false;
			Textures.TexturePassionMajor = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajor", true);
			Textures.TexturePassionMinor = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinor", true);
			Textures.TextureRadioButtonOff = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff", true);
			Textures.TexturePortraitBackground = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/CharMakerPortraitBG", true);
			Textures.TextureFieldAtlas = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/FieldAtlas", true);
			Textures.TextureButtonPrevious = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonPrevious", true);
			Textures.TextureButtonNext = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonNext", true);
			Textures.TextureButtonRandom = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonRandom", true);
			Textures.TextureButtonRandomLarge = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonRandomLarge", true);
			Textures.TexturePassionNone = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/NoPassion", true);
			Textures.TextureButtonDelete = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonDelete", true);
			Textures.TextureButtonDeleteTab = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonDeleteTab", true);
			Textures.TextureButtonDeleteTabHighlight = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonDeleteTabHighlight", true);
			Textures.TextureButtonGenderFemale = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonGenderFemale", true);
			Textures.TextureButtonGenderMale = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonGenderMale", true);
			Textures.TextureButtonReset = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonReset", true);
			Textures.TextureButtonClearSkills = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonClear", true);
			Textures.TextureDropdownIndicator = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/DropdownIndicator", true);
			Textures.TextureAlert = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/Alert", true);
			Textures.TextureAlertSmall = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/AlertSmall", true);
			Textures.TextureDerivedRelationship = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/DerivedRelationship", true);
			Textures.TextureButtonAdd = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ButtonAdd", true);
			Textures.TextureDeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
			Textures.TextureSortAscending = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/SortAscending", true);
			Textures.TextureSortDescending = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/SortDescending", true);
			Textures.TextureArrowLeft = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ArrowLeft", true);
			Textures.TextureArrowRight = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ArrowRight", true);
			Textures.TextureArrowDown = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/ArrowDown", true);
			Textures.TextureGenderFemaleLarge = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/GenderFemaleLarge", true);
			Textures.TextureGenderMaleLarge = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/GenderMaleLarge", true);
			Textures.TextureGenderlessLarge = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/GenderlessLarge", true);
			Textures.TextureCheckbox = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/Checkbox", true);
			Textures.TextureCheckboxSelected = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/CheckboxSelected", true);
			Textures.TextureCheckboxPartiallySelected = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/CheckboxPartiallySelected", true);
			Textures.TextureDottedLine = ContentFinder<Texture2D>.Get("EdB/PrepareCarefully/DottedLine", true);
			Textures.TextureTabAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TabAtlas", true);
			Textures.TextureButtonBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG", true);
			Textures.TextureButtonBGAtlasMouseover = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover", true);
			Textures.TextureButtonBGAtlasClick = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick", true);
			Textures.TextureButtonInfo = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
			Textures.TextureAlternateRow = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.05f));
			Textures.TextureSkillBarFill = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
			Textures.TextureHighlightRow = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.2f));
			Textures.loaded = true;
		}

		// Token: 0x0400007F RID: 127
		private static bool loaded;

		// Token: 0x04000080 RID: 128
		public static Texture2D TexturePassionMajor;

		// Token: 0x04000081 RID: 129
		public static Texture2D TexturePassionMinor;

		// Token: 0x04000082 RID: 130
		public static Texture2D TextureFieldAtlas;

		// Token: 0x04000083 RID: 131
		public static Texture2D TexturePortraitBackground;

		// Token: 0x04000084 RID: 132
		public static Texture2D TextureButtonPrevious;

		// Token: 0x04000085 RID: 133
		public static Texture2D TextureButtonNext;

		// Token: 0x04000086 RID: 134
		public static Texture2D TextureButtonRandom;

		// Token: 0x04000087 RID: 135
		public static Texture2D TextureButtonRandomLarge;

		// Token: 0x04000088 RID: 136
		public static Texture2D TexturePassionNone;

		// Token: 0x04000089 RID: 137
		public static Texture2D TextureButtonDelete;

		// Token: 0x0400008A RID: 138
		public static Texture2D TextureButtonDeleteTab;

		// Token: 0x0400008B RID: 139
		public static Texture2D TextureButtonDeleteTabHighlight;

		// Token: 0x0400008C RID: 140
		public static Texture2D TextureButtonGenderFemale;

		// Token: 0x0400008D RID: 141
		public static Texture2D TextureButtonGenderMale;

		// Token: 0x0400008E RID: 142
		public static Texture2D TextureButtonReset;

		// Token: 0x0400008F RID: 143
		public static Texture2D TextureButtonClearSkills;

		// Token: 0x04000090 RID: 144
		public static Texture2D TextureDropdownIndicator;

		// Token: 0x04000091 RID: 145
		public static Texture2D TextureAlert;

		// Token: 0x04000092 RID: 146
		public static Texture2D TextureAlertSmall;

		// Token: 0x04000093 RID: 147
		public static Texture2D TextureDerivedRelationship;

		// Token: 0x04000094 RID: 148
		public static Texture2D TextureButtonAdd;

		// Token: 0x04000095 RID: 149
		public static Texture2D TextureRadioButtonOff;

		// Token: 0x04000096 RID: 150
		public static Texture2D TextureDeleteX;

		// Token: 0x04000097 RID: 151
		public static Texture2D TextureAlternateRow;

		// Token: 0x04000098 RID: 152
		public static Texture2D TextureSkillBarFill;

		// Token: 0x04000099 RID: 153
		public static Texture2D TextureSortAscending;

		// Token: 0x0400009A RID: 154
		public static Texture2D TextureSortDescending;

		// Token: 0x0400009B RID: 155
		public static Texture2D TextureTabAtlas;

		// Token: 0x0400009C RID: 156
		public static Texture2D TextureButtonBGAtlas;

		// Token: 0x0400009D RID: 157
		public static Texture2D TextureButtonBGAtlasMouseover;

		// Token: 0x0400009E RID: 158
		public static Texture2D TextureButtonBGAtlasClick;

		// Token: 0x0400009F RID: 159
		public static Texture2D TextureArrowLeft;

		// Token: 0x040000A0 RID: 160
		public static Texture2D TextureArrowRight;

		// Token: 0x040000A1 RID: 161
		public static Texture2D TextureArrowDown;

		// Token: 0x040000A2 RID: 162
		public static Texture2D TextureGenderFemaleLarge;

		// Token: 0x040000A3 RID: 163
		public static Texture2D TextureGenderMaleLarge;

		// Token: 0x040000A4 RID: 164
		public static Texture2D TextureGenderlessLarge;

		// Token: 0x040000A5 RID: 165
		public static Texture2D TextureButtonInfo;

		// Token: 0x040000A6 RID: 166
		public static Texture2D TextureCheckbox;

		// Token: 0x040000A7 RID: 167
		public static Texture2D TextureCheckboxSelected;

		// Token: 0x040000A8 RID: 168
		public static Texture2D TextureCheckboxPartiallySelected;

		// Token: 0x040000A9 RID: 169
		public static Texture2D TextureDottedLine;

		// Token: 0x040000AA RID: 170
		public static Texture2D TextureHighlightRow;
	}
}
