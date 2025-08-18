using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal abstract class CapItem : ModItem
{
    private GearSlotGlobalItem? GlobalItem => Item.GetGlobalItemOrNull<GearSlotGlobalItem>();

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 20;
        Item.accessory = true;

        if (GlobalItem != null) GlobalItem.gearType = GearType.Cap;
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand) => !(pre == -1 || pre == -3);

    public override bool CanEquipAccessory(Player player, int slot, bool modded) => player.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false && modded;
}
