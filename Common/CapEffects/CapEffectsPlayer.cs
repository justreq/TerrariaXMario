using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class CapEffectsPlayer : ModPlayer
{
    internal CapPlayer? CapPlayer => Player.GetModPlayerOrNull<CapPlayer>();

    internal bool crouching;

    public override void PostUpdateRunSpeeds()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        if (crouching && Player.IsOnGroundPrecise())
        {
            Player.maxRunSpeed *= 0.6f;
            Player.accRunSpeed *= 0.3f;
            Player.runAcceleration *= 1.5f;
        }

        if (CapPlayer?.Cap == "Luigi" && !crouching && Player.IsOnGroundPrecise()) Player.runSlowdown = 0.045f;
    }

    public override void PreUpdate()
    {
        if (Player.controlDown && Player.IsOnGroundPrecise()) crouching = true;
        if (crouching && !Player.controlDown) crouching = false;
    }
}
