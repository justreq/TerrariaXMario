using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal class CapPlayer : ModPlayer
{
    private ModAccessorySlot CapSlot => LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<GearSlot_Cap>().Type, Player);
    private GearSlotPlayer? GearSlotPlayer => Player.GetModPlayerOrNull<GearSlotPlayer>();

    private string? oldCap = null;
    internal string? Cap => CapSlot.FunctionalItem.IsAir ? null : CapSlot.FunctionalItem.ModItem.Name;

    internal string? powerup = null;

    internal bool CanDoCapEffects => Cap != null && (GearSlotPlayer?.ShowGearSlots ?? false);

    public override void FrameEffects()
    {
        if (!CanDoCapEffects)
        {
            if (powerup != null) powerup = null;
            return;
        }

        Player.head = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, $"{powerup ?? ""}{Cap}", EquipType.Legs);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (!CanDoCapEffects) return;

        modifiers.DisableSound();
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        if (!CanDoCapEffects) return;

        if (powerup != null)
        {
            powerup = null;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown") { Volume = 0.4f });
        }

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{Cap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f });
    }

    public override void UpdateDead()
    {
        powerup = null;
    }

    public override void PostUpdate()
    {
        if ((GearSlotPlayer?.ShowGearSlots ?? false) && oldCap != Cap)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{Cap ?? oldCap}{(Cap == null ? "Unequip" : "Equip")}") { Volume = 0.4f });
            oldCap = Cap;
        }

        if (!CanDoCapEffects) return;

        if ((Player.wet && PlayerInput.Triggers.JustPressed.Jump) || Player.justJumped)
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f });
    }
}
