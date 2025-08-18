using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal class GearSlotGlobalItem : GlobalItem
{
    internal GearType gearType;

    public override bool InstancePerEntity => true;

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        if (gearType == GearType.None && (player.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false)) return false;

        return base.CanEquipAccessory(item, player, slot, modded);
    }
}