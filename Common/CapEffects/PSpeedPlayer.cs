using System;
using Terraria;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class PSpeedPlayer : CapEffectsPlayer
{
    private int runTime;

    public override void PostUpdateRunSpeeds()
    {
        base.PostUpdateRunSpeeds();

        if (crouching || (!CapPlayer?.CanDoCapEffects ?? true)) return;

        if (runTime != 0 && !Player.controlLeft && !Player.controlRight) runTime = 0;
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f) runTime++;

        if (runTime > 120)
        {
            hasPSpeed = true;
            Player.accRunSpeed *= CapPlayer!.CurrentCap == "Luigi" ? 1.5f : 1.25f;
        }
        else hasPSpeed = false;
    }
}
