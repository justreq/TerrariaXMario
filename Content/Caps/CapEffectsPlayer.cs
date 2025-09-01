using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;
internal class CapEffectsPlayer : ModPlayer
{
    private CapPlayer? CapPlayer => Player.GetModPlayerOrNull<CapPlayer>();

    private int stompCount = 0;

    private void Stomp(NPC target)
    {
        target.StrikeInstantKill();

        Player.velocity.Y = Player.controlJump ? -7.5f : -5f;
        if (!Player.immune)
        {
            Player.immuneTime = 15;
            Player.immune = true;
        }

        stompCount++;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(stompCount > 7 ? "Heal" : $"Stomp{stompCount}")}"));

        if (stompCount > 7) Player.Heal(1);
    }

    public override void PostUpdate()
    {
        if (stompCount > 0 && (Player.IsOnGroundPrecise() || Player.mount.Active)) stompCount = 0;

        if (!CapPlayer?.CanDoCapEffects ?? false) return;

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (Player.mount.Active || Player.velocity.Y <= 0 || Math.Abs(Player.Hitbox.Bottom - npc.Hitbox.Top) > 10 || !npc.Hitbox.Intersects(Player.Hitbox)) continue;

            Stomp(npc);
        }
    }
}
