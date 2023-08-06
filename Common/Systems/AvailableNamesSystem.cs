using Microsoft.Xna.Framework;
using MonoMod.Cil;
using RainbowWeaponForgery.Common.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RainbowWeaponForgery.Common.Systems;

public sealed class AvailableNamesSystem : ModSystem
{
	public static string[] PossibleNames { get; private set; }

	public override void Load()
	{
		PossibleNames = Array.Empty<string>();
		IL_Projectile.GetLastPrismHue += CollectNamesFromLastPrism;
		On_Projectile.GetLastPrismHue += HijackLastPrism;
		On_Projectile.GetFairyQueenWeaponsColor += HijackEoLWeapons;
	}

	public override void Unload()
	{
		PossibleNames = Array.Empty<string>();
		IL_Projectile.GetLastPrismHue -= CollectNamesFromLastPrism;
		On_Projectile.GetLastPrismHue -= HijackLastPrism;
		On_Projectile.GetFairyQueenWeaponsColor -= HijackEoLWeapons;
	}

	/// <summary>
	/// Programmatically extract all of the special names from <see cref="Projectile.GetLastPrismHue"/>.<br/>
	/// <see cref="Projectile.GetFairyQueenWeaponsColor(float, float, float?)"/> could also be used, as both methods contain the same names.
	/// </summary>
	private static void CollectNamesFromLastPrism(ILContext il)
	{
		ILCursor c = new(il);
		List<string> names = new();

		string tempName = null;
		while (c.TryGotoNext(MoveType.Before, i => i.MatchLdstr(out tempName)))
		{
			names.Add(tempName);
		}

		names.Sort();
		PossibleNames = names.ToArray();
	}

	private static float HijackLastPrism(On_Projectile.orig_GetLastPrismHue orig, Projectile self, float laserIndex, ref float laserLuminance, ref float laserAlphaMultiplier)
	{
		Player owner = Main.player[self.owner];
		string originalName = owner.name;
		if (owner.TryGetModPlayer(out ForgedNamePlayer fPlayer) && fPlayer.NameIsForged(out string newName))
		{
			owner.name = newName;
		}

		float hue = orig(self, laserIndex, ref laserLuminance, ref laserAlphaMultiplier);

		owner.name = originalName;
		return hue;
	}

	private static Color HijackEoLWeapons(On_Projectile.orig_GetFairyQueenWeaponsColor orig, Projectile self, float alphaChannelMultiplier, float lerpToWhite, float? rawHueOverride)
	{
		Player owner = Main.player[self.owner];
		string originalName = owner.name;
		if (owner.TryGetModPlayer(out ForgedNamePlayer fPlayer) && fPlayer.NameIsForged(out string newName))
		{
			owner.name = newName;
		}

		Color color = orig(self, alphaChannelMultiplier, lerpToWhite, rawHueOverride);

		owner.name = originalName;
		return color;
	}
}