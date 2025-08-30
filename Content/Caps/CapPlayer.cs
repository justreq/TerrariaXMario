using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal class CapPlayer : ModPlayer
{
    private ModAccessorySlot CapSlot => LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<GearSlot_Cap>().Type, Player);

    internal string? Cap => CapSlot.FunctionalItem.IsAir ? null : CapSlot.FunctionalItem.ModItem.Name;
    internal string? powerup = null;

    public override void FrameEffects()
    {
        if (Cap == null || (!Player.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false))
        {
            powerup = null;
            return;
        }

        Player.head = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Legs);
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        powerup = null;
    }
}
