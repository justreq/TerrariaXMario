using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class JumpPlayer : CapEffectsPlayer
{
    private int jumpInputBuffer;
    private int jumpFlipTimer;
    private int jumpFlipDuration;
    private bool backflip;

    public override void PostUpdate()
    {
        if (Player.mount.Active || (!CapPlayer?.CanDoCapEffects ?? true)) return;

        if (jumpInputBuffer > 0 && !Player.IsOnGroundPrecise()) jumpInputBuffer = 0;

        if ((crouching && currentJump is Jump.Single or Jump.Double or Jump.Triple) || jumpInputBuffer > 10 || Player.sliding)
        {
            currentJump = Jump.None;
            jumpInputBuffer = 0;
        }

        if ((currentJump is Jump.Single or Jump.Double) && Player.IsOnGroundPrecise())
        {
            jumpInputBuffer++;

            if (!Player.controlLeft && !Player.controlRight)
            {
                currentJump = Jump.None;
                jumpInputBuffer = 0;
            }
        }

        if (currentJump == Jump.Triple && Player.IsOnGroundPrecise()) currentJump = Jump.None;

        if (jumpFlipTimer > 0)
        {
            if (Player.IsOnGroundPrecise() || currentJump == Jump.None)
            {
                jumpFlipTimer = 0;
                jumpFlipDuration = 0;
                backflip = false;
                return;
            }

            if (jumpFlipDuration == 0) jumpFlipDuration = jumpFlipTimer;
            jumpFlipTimer--;

            Player.fullRotationOrigin = Player.Size * 0.5f;
            Player.fullRotation = (jumpFlipTimer / (float)jumpFlipDuration) * MathHelper.TwoPi * -Player.direction * (backflip ? -1 : 1);
        }
        else
        {
            if (jumpFlipDuration != 0) jumpFlipDuration = 0;
            if (Player.fullRotation != 0) Player.fullRotation = 0;
        }

        if (Player.justJumped && !crouching)
        {
            currentJump = currentJump == Jump.Triple ? Jump.Single : currentJump + 1;

            if (currentJump == Jump.Triple)
            {
                jumpFlipTimer = 45;
                backflip = false;
            }

            if (currentJump is not (Jump.None or Jump.Single) && (int)currentJump < Enum.GetNames(typeof(Jump)).Length) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{CapPlayer?.currentCap}{currentJump}Jump") { Volume = 0.4f });
        }
    }
}
