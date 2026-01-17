using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FireFlowerData : Powerup
{
    public override string Name => "FireFlower";

    internal override ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.Swing;
    internal override Color Color => new(234, 51, 34);
    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal static Vector2 GetInitialProjectileVelocity(Player player, float gravity)
    {
        Vector2 start = player.MountedCenter;
        Vector2 apex = Main.MouseWorld;

        float dy = start.Y - apex.Y;
        if (dy <= 0)
        {
            Vector2 speed = (apex - start).SafeNormalize(Vector2.Zero) * (apex.Distance(start) * 0.05f);
            return new(MathHelper.Clamp(speed.X, -12, 12), MathHelper.Clamp(speed.Y, -12, 12));
        }

        float vy = (float)Math.Sqrt(2f * gravity * dy);
        float t = vy / gravity;
        float vx = (apex.X - start.X) / t;


        return new(MathHelper.Clamp(vx, -8, 8), -vy);
    }

    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.fireFlowerFireballsCast > 1) return;

        modPlayer?.fireFlowerFireballsCast += 1;
        if (modPlayer?.fireFlowerCooldown == 0) modPlayer?.fireFlowerCooldown = 30;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/FireFlowerShoot") { Volume = 0.4f }, player.MountedCenter);
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, GetInitialProjectileVelocity(player, 0.4f), ModContent.ProjectileType<FireFlowerFireball>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1, 0f, player.whoAmI);
        modPlayer?.PowerupCharge -= 30;
    }
}

internal class FireFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<FireFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneDesert || player.ZoneUndergroundDesert || player.ZoneUnderworldHeight;
}