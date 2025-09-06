using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal class CapPlayer : ModPlayer
{
    private ModAccessorySlot CapSlot => LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<GearSlot_Cap>().Type, Player);
    private GearSlotPlayer? GearSlotPlayer => Player.GetModPlayerOrNull<GearSlotPlayer>();

    private string? oldCap = null;
    internal string? CurrentCap => CapSlot?.FunctionalItem.IsAir ?? true ? null : CapSlot.FunctionalItem.ModItem.Name;

    internal Powerup? currentPowerup = null;

    internal bool CanDoCapEffects => CurrentCap != null && (GearSlotPlayer?.ShowGearSlots ?? false);

    public override void FrameEffects()
    {
        if (!CanDoCapEffects)
        {
            if (currentPowerup != null) currentPowerup = null;
            return;
        }

        Player.head = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{CurrentCap}", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{CurrentCap}", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{CurrentCap}", EquipType.Legs);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (!CanDoCapEffects) return;

        modifiers.DisableSound();
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!CanDoCapEffects) return;

        if (currentPowerup != null)
        {
            currentPowerup = null;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown") { Volume = 0.4f });
        }

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{CurrentCap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f });
    }

    public override void PostUpdateEquips()
    {
        if (!CanDoCapEffects || currentPowerup == null) return;

        currentPowerup.UpdateConsumed(Player);

        if (PlayerInput.Triggers.JustPressed.MouseLeft) currentPowerup.OnLeftClick(Player);
    }

    public override void PostUpdate()
    {
        if ((GearSlotPlayer?.ShowGearSlots ?? false) && oldCap != CurrentCap)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{CurrentCap ?? oldCap}{(CurrentCap == null ? "Unequip" : "Equip")}") { Volume = 0.4f });
            oldCap = CurrentCap;
        }

        if (!CanDoCapEffects) return;

        if ((Player.wet && PlayerInput.Triggers.JustPressed.Jump) || Player.justJumped)
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f });
    }
}
