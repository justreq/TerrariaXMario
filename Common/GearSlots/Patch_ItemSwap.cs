using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal sealed class Patch_ItemSwap : BasePatch
{
    private static bool ShowGearSlots => Main.LocalPlayer.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false;

    internal override void Patch(Mod mod)
    {
        // Prevents the player from quick-equipping items when gear slots are enabled
        On_ItemSlot.ArmorSwap += On_ItemSlot_ArmorSwap;
    }

    private Item On_ItemSlot_ArmorSwap(On_ItemSlot.orig_ArmorSwap orig, Item item, out bool success)
    {
        if (ShowGearSlots && item.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType == GearType.None)
        {
            success = false;
            return item;
        }

        return orig(item, out success);
    }
}