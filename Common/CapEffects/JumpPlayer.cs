using Terraria;
using Terraria.Audio;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal enum Jump { None, Single, Double, Triple, Backflip, Sideflip, Long }

internal class JumpPlayer : CapEffectsPlayer
{
    internal Jump currentJump;
    private int jumpInputBuffer;

    public override void PostUpdate()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        Main.NewText($"{currentJump} {jumpInputBuffer}");

        if (jumpInputBuffer > 0 && !Player.IsOnGroundPrecise()) jumpInputBuffer = 0;

        if (crouching || jumpInputBuffer > 20)
        {
            currentJump = Jump.None;
            jumpInputBuffer = 0;
        }

        if (Player.justJumped)
        {
            currentJump = currentJump == Jump.Triple ? Jump.Single : currentJump + 1;
            if (currentJump == Jump.Double || currentJump == Jump.Triple) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{CapPlayer?.Cap}{currentJump}Jump") { Volume = 0.4f });
        }

        if ((currentJump == Jump.Single || currentJump == Jump.Double) && Player.IsOnGroundPrecise()) jumpInputBuffer++;

        if (currentJump == Jump.Triple && Player.IsOnGroundPrecise()) currentJump = Jump.None;
    }
}
