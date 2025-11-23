using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Content.Powerups;
internal class BoomerangFlowerData : Powerup
{
    public override string Name => "BoomerangFlower";

    internal override ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.Extend;

    internal override int RightClickActionCooldown => 8;

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnRightClick(Player player)
    {
        if (Main.projectile.Any(e => e.type == ModContent.ProjectileType<BoomerangFlowerBoomerang>() && e.active && e.owner == player.whoAmI)) return;
        Vector2 velocity = Main.MouseWorld - player.Center;
        velocity.Normalize();
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, velocity * 6, ModContent.ProjectileType<BoomerangFlowerBoomerang>(), 1, 0f, player.whoAmI);
    }
}

internal class BoomerangFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<BoomerangFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}