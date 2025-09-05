using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal abstract class CapItem : ModItem
{
    private GearSlotGlobalItem? GlobalItem => Item.GetGlobalItemOrNull<GearSlotGlobalItem>();

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
        EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
        EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server) return;

        int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
        int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
        int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

        if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        if (equipSlotBody != -1)
        {
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
        }
        if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 20;
        Item.accessory = true;

        if (GlobalItem != null) GlobalItem.gearType = GearType.Cap;
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand) => !(pre == -1 || pre == -3);

    public override bool CanEquipAccessory(Player player, int slot, bool modded) => player.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false && modded;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.lifeRegen = 0;
        player.lifeRegenTime = 0;
        player.accFlipper = true;
        player.noKnockback = true;

        if (Name == "Luigi") player.jumpSpeedBoost = 0.5f;

        CapEffectsPlayer? capEffectsPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (capEffectsPlayer == null) return;

        if (!capEffectsPlayer.crouching && !capEffectsPlayer.groundPounding) player.spikedBoots = 1;
    }
}

internal class Mario : CapItem { }
internal class Luigi : CapItem { }