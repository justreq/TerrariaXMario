using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal class GearSlotGlobalItem : GlobalItem
{
    internal GearType gearType;

    public override bool InstancePerEntity => true;

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        if (gearType == GearType.None && (player.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? false)) return false;

        return base.CanEquipAccessory(item, player, slot, modded);
    }
}