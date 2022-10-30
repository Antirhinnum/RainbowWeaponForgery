using RainbowWeaponForgery.Common.Systems;
using Terraria;
using Terraria.ModLoader;

namespace RainbowWeaponForgery.Common.Players;

public sealed class ForgedNamePlayer : ModPlayer
{
	/// <summary>
	/// The index in <see cref="AvailableNamesSystem.PossibleNames"/> of the player's currently-forged name.
	/// </summary>
	internal int currentlyForgedNameIndex;

	/// <summary>
	/// The name this player should use for special effects.
	/// </summary>
	public string CurrentlyForgedName => AvailableNamesSystem.PossibleNames.IndexInRange(currentlyForgedNameIndex) ? AvailableNamesSystem.PossibleNames[currentlyForgedNameIndex] : Player.name;

	/// <summary>
	/// If true, then the player is using <paramref name="name"/> for special effects.
	/// </summary>
	public bool NameIsForged(out string name)
	{
		name = CurrentlyForgedName;
		return Player.name != name;
	}

	public override void ResetEffects()
	{
		currentlyForgedNameIndex = -1;
	}
}