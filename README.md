# Rainbow Weapon Forgery

![Rainbow Weapon Forgery mod icon](icon.png)

**Rainbow Weapon Forgery** adds a single accessory, the Forged Developer Signature. With it, the player is allowed to use special developer-only colors for the Last Prism and the Empress of Light's weapons without having to change their character's name.

[Download the mod on Steam!](https://steamcommunity.com/sharedfiles/filedetails/?id=2882074287)

## Mod Calls

| Call | Description | Example
| --- | --- | --- |
| `"IsNameForged", Player player : Tuple<bool, string>` | Determines if a player is currently using a forged name. The `bool` value is `true` if the player is currently using a forged name. If the `bool` value is `true`, then the  `string` value will be the forged name. If the `bool` value is `false`, the `string` value will be `player.name`. | `Tuple<bool, string> results = mod.Call("IsNameForged", Main.LocalPlayer) as Tuple<bool, string>;` |
| `"AddLegalDocument", int itemId : bool` | Tries to register an item ID (`Item.type`) as a legal document for the purposes of crafting the Forged Developer Signature. Returns `HashSet<int>.Add(itemId)`. | `mod.Call("AddLegalDocument", ModContent.ItemType<MyLegalDocumentItem>());` |