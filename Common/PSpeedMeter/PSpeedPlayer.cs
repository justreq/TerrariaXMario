using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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

        if (crouching || (!CapPlayer?.CanDoCapEffects ?? true))
        {
            runTime = 0;
            hasPSpeed = false;
            return;
        }

        if (runTime != 0 && !Player.controlLeft && !Player.controlRight) runTime = 0;
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f) runTime++;

        if (runTime >= runTimeRequiredForPSpeed)
        {
            if (!hasPSpeed)
            {
                hasPSpeed = true;
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/FastRunStart") { Volume = 0.4f });

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Player.Center, DustID.Cloud, Scale: 1.5f);
                }
            }

            Player.accRunSpeed *= CapPlayer!.currentCap == "Luigi" ? 1.5f : 1.25f;
        }
        else if (hasPSpeed) hasPSpeed = false;
    }
}
