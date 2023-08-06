using RainbowWeaponForgery.Common.Players;
using RainbowWeaponForgery.Common.Systems;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RainbowWeaponForgery;

public sealed class RainbowWeaponForgery : Mod
{
	public override object Call(params object[] args)
	{
		if (args is ["IsNameForged", Player player])
		{
			if (!player.TryGetModPlayer(out ForgedNamePlayer fPlayer))
			{
				return new Tuple<bool, string>(false, player.name);
			}

			return new Tuple<bool, string>(fPlayer.NameIsForged(out string name), name);
		}
		else if (args is ["AddLegalDocument", int type])
		{
			return RecipeGroupSystem.LicensesGroup.ValidItems.Add(type);
		}

		return null;
	}
}