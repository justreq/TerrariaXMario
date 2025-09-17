using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal enum Jump { None, Single, Double, Triple }
internal class CapEffectsPlayer : ModPlayer
{
    internal CapPlayer? CapPlayer => Player.GetModPlayerOrNull<CapPlayer>();

    internal bool crouching;
    internal bool groundPounding;
    internal Jump currentJump;
    internal bool hasPSpeed;

    internal Vector2 currentObjectSpawnerBlockToEdit;

    public override void PostUpdateRunSpeeds()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        if (crouching && Player.IsOnGroundPrecise())
        {
            Player.maxRunSpeed *= 0.6f;
            Player.accRunSpeed *= 0.3f;
            Player.runAcceleration *= 1.5f;
        }

        if (CapPlayer?.currentCap == "Luigi")
        {
            if (!crouching && Player.IsOnGroundPrecise()) Player.runSlowdown = 0.045f;

            Player.maxRunSpeed *= 1.05f;
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (Player.mount.Active || (!CapPlayer?.CanDoCapEffects ?? true))
        {
            player.headPosition = Vector2.Zero;
            player.headRotation = 0;
            return;
        }

        if (crouching)
        {
            player.headPosition.X = 4 * player.direction;
            player.headPosition.Y = 8;
            player.headRotation = MathHelper.PiOver4 * 0.5f * player.direction;

            if (!player.IsOnGroundPrecise()) player.legFrame.Y = 56 * 7;
        }
        else
        {
            if (!player.controlDown || player.IsOnGroundPrecise()) player.headPosition.X = 0;

            if (CapPlayer?.currentCap == "Luigi")
            {
                if (Player.sitting.isSitting) Player.headPosition.Y = 2;
                else Player.bodyPosition.Y = Player.IsOnGroundPrecise() ? -2 : 0;
            }
        }
    }

    public override void PreUpdate()
    {
        if (Player.mount.Active || (!CapPlayer?.CanDoCapEffects ?? true))
        {
            crouching = false;
            return;
        }

        if (Player.controlDown && Player.IsOnGroundPrecise()) crouching = true;
        if (crouching && !Player.controlDown) crouching = false;
    }

    public override void PostUpdate()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        if (crouching) Player.bodyFrame.Y = 56 * 2;
    }

    public override void PostUpdateEquips()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true)
        {
            if (currentObjectSpawnerBlockToEdit != Vector2.Zero) currentObjectSpawnerBlockToEdit = Vector2.Zero;
            return;
        }

        if (currentJump == Jump.Double) Player.jumpSpeedBoost += 1.25f;
        else if (currentJump == Jump.Triple) Player.jumpSpeedBoost += 2.5f;
    }
}
