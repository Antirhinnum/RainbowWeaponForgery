using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;

namespace RainbowWeaponForgery.Common.Players;

public sealed class ShiftClickPlayer : ModPlayer
{
	// Required to sync equips in modded accessory slots.
	private static MethodInfo _ModAccessorySlotPlayer_NetHandler_SendSlot;

	public override void Load()
	{
		_ModAccessorySlotPlayer_NetHandler_SendSlot = typeof(ModAccessorySlotPlayer).GetNestedType("NetHandler", BindingFlags.NonPublic | BindingFlags.Static).GetMethod("SendSlot", BindingFlags.Public | BindingFlags.Static);
	}

	public override void Unload()
	{
		_ModAccessorySlotPlayer_NetHandler_SendSlot = null;
	}

	public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
	{
		Item item = inventory[slot];

		if (item.ModItem is IHaveSpecialShiftClickBehavior behavior && (behavior.AllowedContexts.Length == 0 || behavior.AllowedContexts.Contains(context)))
		{
			behavior.OnShiftClick(context, out bool shouldSync);
			if (Main.netMode == NetmodeID.MultiplayerClient && shouldSync)
			{
				if (context == ItemSlot.Context.InventoryItem) // Player inventory
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, Main.myPlayer, null, Main.myPlayer, slot);
				}
				else if (context < 0) // Modded equip slots
				{
					_ModAccessorySlotPlayer_NetHandler_SendSlot.Invoke(null, new object[] { -1, Main.myPlayer, slot, inventory[slot] });
				}
				else // Vanilla equip slots
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, Main.myPlayer, null, Main.myPlayer, PlayerItemSlotID.Armor0 + slot);
				}
			}
			return true;
		}

		return base.ShiftClickSlot(inventory, context, slot);
	}
}

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will have special logic when shift-clicked.
/// </summary>
public interface IHaveSpecialShiftClickBehavior
{
	/// <summary>
	/// The contexts from <see cref="ItemSlot.Context"/> that <see cref="OnShiftClick(int)"/> can be run from.<br/>
	/// If empty, then all contexts are allowed.
	/// </summary>
	int[] AllowedContexts => Array.Empty<int>();

	/// <summary>
	/// Used to perform the special behavior.
	/// </summary>
	/// <param name="context">The context from <see cref="ItemSlot.Context"/> that this method is being run from.</param>
	/// <param name="shouldSync">If <see langword="true"/>, then this change will be synced to other clients.</param>
	void OnShiftClick(int context, out bool shouldSync);
}