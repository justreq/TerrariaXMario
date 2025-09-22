using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class CapPlayer : ModPlayer
{
    private GearSlotPlayer? GearSlotPlayer => Player.GetModPlayerOrNull<GearSlotPlayer>();

    private string? oldCap;
    internal string? currentCap;

    internal Powerup? currentPowerup;

    internal bool CanDoCapEffects => currentCap != null && (GearSlotPlayer?.ShowGearSlots ?? false);

    internal int forceDirection;
    internal int forceSwingDuration;
    internal int forceSwingTimer;

    internal void SetForceDirection(int duration, int direction)
    {
        if (duration <= 0 || Math.Abs(direction) != 1) return;

        forceSwingTimer = duration;
        forceDirection = direction;
        Player.direction = direction;
    }

    public override void ResetEffects()
    {
        currentCap = null;
    }

    public override void FrameEffects()
    {
        if (!CanDoCapEffects)
        {
            if (currentPowerup != null) currentPowerup = null;
            return;
        }

        Player.head = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}", EquipType.Legs);
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

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{currentCap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f });
    }

    public override void PostUpdateEquips()
    {
        if (!CanDoCapEffects || currentPowerup == null) return;

        currentPowerup.UpdateConsumed(Player);

        if (PlayerInput.Triggers.JustPressed.MouseLeft && !Player.mouseInterface && Main.cursorOverride != TerrariaXMario.Instance.CursorGrabIndex && Main.cursorOverride != TerrariaXMario.Instance.CursorThrowIndex && Player.HeldItem.IsAir && Main.mouseItem.IsAir)
        {
            SetForceDirection(10, Math.Sign(Main.MouseWorld.X - Player.position.X));
            currentPowerup.OnLeftClick(Player);
        }
    }

    public override void PostUpdate()
    {
        if ((GearSlotPlayer?.ShowGearSlots ?? false) && oldCap != currentCap)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{currentCap ?? oldCap}{(currentCap == null ? "Unequip" : "Equip")}") { Volume = 0.4f });
            oldCap = currentCap;
        }

        if (!CanDoCapEffects)
        {
            forceDirection = 0;
            forceSwingDuration = 0;
            forceSwingTimer = 0;
            return;
        }

        if (Player.wet && PlayerInput.Triggers.JustPressed.Jump || Player.justJumped)
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f });

        if (forceSwingTimer > 0)
        {
            if (forceSwingDuration == 0) forceSwingDuration = forceSwingTimer;

            Player.bodyFrame.Y = 56 * Math.Clamp(Math.Max(1, 4 - (int)Math.Floor((double)forceSwingTimer / forceSwingDuration * 4)), 1, 4);

            forceSwingTimer--;

            if (forceSwingTimer <= 0)
            {
                forceDirection = 0;
                forceSwingDuration = 0;
                forceSwingTimer = 0;
            }
        }
    }
}
