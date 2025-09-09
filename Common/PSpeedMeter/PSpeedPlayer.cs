using System;
using Terraria;
using Terraria.Audio;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.PSpeedMeter;
internal class PSpeedPlayer : CapEffectsPlayer
{
    internal int runTime;
    internal int runTimeRequiredForPSpeed = 120;

    public override void PostUpdateRunSpeeds()
    {
        base.PostUpdateRunSpeeds();

        if (crouching || (!CapPlayer?.CanDoCapEffects ?? true)) return;

        if (runTime != 0 && !Player.controlLeft && !Player.controlRight) runTime = 0;
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f) runTime++;

        if (runTime >= runTimeRequiredForPSpeed)
        {
            if (!hasPSpeed)
            {
                hasPSpeed = true;
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/FastRunStart") { Volume = 0.4f });
            }

            Player.accRunSpeed *= CapPlayer!.CurrentCap == "Luigi" ? 1.5f : 1.25f;
        }
        else if (hasPSpeed) hasPSpeed = false;
    }
}
