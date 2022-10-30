using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RainbowWeaponForgery.Common.Systems
{
	public sealed class RecipeGroupSystem : ModSystem
	{
		/// <summary>
		/// Any sort of legal document that can be used for forgery
		/// </summary>
		public static RecipeGroup LicensesGroup { get; private set; }

		public override void AddRecipeGroups()
		{
			LicensesGroup = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Language.GetTextValue($"Mods.{nameof(RainbowWeaponForgery)}.Misc.LicenseRecipeGroup")}", ItemID.LicenseCat, ItemID.LicenseDog, ItemID.LicenseBunny);
			RecipeGroup.RegisterGroup($"{nameof(RainbowWeaponForgery)}:Licenses", LicensesGroup);
		}
	}
}