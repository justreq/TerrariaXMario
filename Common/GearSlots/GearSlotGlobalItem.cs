using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal class GearSlotGlobalItem : GlobalItem
{
    internal GearContext gearContext;

    public override bool InstancePerEntity => true;

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        if (player.GetModPlayerOrNull<GearSlotPlayer>()?.showGearSlots ?? false) return false;

        return base.CanEquipAccessory(item, player, slot, modded);
    }
}