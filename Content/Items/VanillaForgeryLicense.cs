using Microsoft.Xna.Framework;
using RainbowWeaponForgery.Common.Players;
using RainbowWeaponForgery.Common.Systems;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace RainbowWeaponForgery.Content.Items;

public sealed class VanillaForgeryLicense : ModItem, IHaveSpecialShiftClickBehavior
{
	// Needed to get the tooltip's color.
	private Projectile _projectileForColor;

	private int _currentlyForgedNameIndex;

	public override void SetStaticDefaults()
	{
		SacrificeTotal = 1;
	}

	public override void SetDefaults()
	{
		Item.Size = new(16, 16);
		Item.accessory = true;
		Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 1));
		Item.canBePlacedInVanityRegardlessOfConditions = true;

		_currentlyForgedNameIndex = 0;
		_projectileForColor = new()
		{
			owner = Main.myPlayer
		};
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (!hideVisual && player.TryGetModPlayer(out ForgedNamePlayer fPlayer))
		{
			fPlayer.currentlyForgedNameIndex = _currentlyForgedNameIndex;
		}
	}

	public override void UpdateVanity(Player player)
	{
		if (player.TryGetModPlayer(out ForgedNamePlayer fPlayer))
		{
			fPlayer.currentlyForgedNameIndex = _currentlyForgedNameIndex;
		}
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		string name = AvailableNamesSystem.PossibleNames[_currentlyForgedNameIndex];
		Player player = Main.LocalPlayer;
		if (player.name != name)
		{
			// Update currentlyForgedIndex so that the hovered item's index is used instead.
			// Without this, the equipped index's color shows up in the inventory.

			ForgedNamePlayer fPlayer = player.GetModPlayer<ForgedNamePlayer>();
			int originalForgery = fPlayer.currentlyForgedNameIndex; // Oxymoron much?
			fPlayer.currentlyForgedNameIndex = _currentlyForgedNameIndex;

			Color rainbowColor = Color.White;

			if (player.HeldItem.type == ItemID.LastPrism)
			{
				float laserLuminance = 0.5f;
				float laserAlphaMultiplier = 0f;
				float hue = _projectileForColor.GetLastPrismHue(0f, ref laserLuminance, ref laserAlphaMultiplier);
				rainbowColor = Main.hslToRgb(hue, 1f, laserLuminance);
			}
			else
			{
				rainbowColor = _projectileForColor.GetFairyQueenWeaponsColor();
			}

			string text = Language.GetTextValueWith($"Mods.{nameof(RainbowWeaponForgery)}.ExtraTooltip.CurrentName", new { Name = name });
			TooltipLine currentNameLine = new(Mod, $"{nameof(RainbowWeaponForgery)}:CurrentName", text)
			{
				OverrideColor = rainbowColor
			};
			tooltips.Add(currentNameLine);

			fPlayer.currentlyForgedNameIndex = originalForgery;
		}
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupSystem.LicensesGroup)
			.AddIngredient(ItemID.BlackInk)
			.AddTile(TileID.TinkerersWorkbench)
			.Register();
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Add(nameof(_currentlyForgedNameIndex), _currentlyForgedNameIndex);
	}

	public override void LoadData(TagCompound tag)
	{
		tag.TryGet(nameof(_currentlyForgedNameIndex), out _currentlyForgedNameIndex);
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write7BitEncodedInt(_currentlyForgedNameIndex);
	}

	public override void NetReceive(BinaryReader reader)
	{
		_currentlyForgedNameIndex = reader.Read7BitEncodedInt();
	}

	public override ModItem Clone(Item newEntity)
	{
		VanillaForgeryLicense newItem = (VanillaForgeryLicense)base.Clone(newEntity);
		newItem._projectileForColor = new()
		{
			owner = _projectileForColor.owner
		};
		return newItem;
	}

	int[] IHaveSpecialShiftClickBehavior.AllowedContexts => new int[]
	{
		ItemSlot.Context.InventoryItem,
		ItemSlot.Context.EquipAccessory,
		ItemSlot.Context.EquipAccessoryVanity,
		ItemSlot.Context.ModdedAccessorySlot,
		ItemSlot.Context.ModdedVanityAccessorySlot
	};

	void IHaveSpecialShiftClickBehavior.OnShiftClick(int context, out bool shouldSync)
	{
		shouldSync = true;

		int totalNames = AvailableNamesSystem.PossibleNames.Length;
		if (!ItemSlot.ControlInUse)
		{
			_currentlyForgedNameIndex++;
			if (_currentlyForgedNameIndex >= totalNames)
			{
				_currentlyForgedNameIndex = 0;
			}
		}
		else
		{
			_currentlyForgedNameIndex--;
			if (_currentlyForgedNameIndex < 0)
			{
				_currentlyForgedNameIndex = totalNames - 1;
			}
		}
	}
}