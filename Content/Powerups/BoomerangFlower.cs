using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class BoomerangFlower : FireFlower
{
    internal override int? ProjectileType => ModContent.ProjectileType<BoomerangFlowerProjectile>();
    internal override int? ItemType => ModContent.ItemType<BoomerangFlowerItem>();

    internal override ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.Extend;
    internal override Color Color => new(24, 153, 230);
    internal override void OnRightClick(Player player)
    {
        if (Main.projectile.Any(e => e.type == ModContent.ProjectileType<Boomerang>() && e.active && e.owner == player.whoAmI)) return;
        Vector2 velocity = Main.MouseWorld - player.MountedCenter;
        velocity.Normalize();
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, velocity * 8, ModContent.ProjectileType<Boomerang>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1, 0f, player.whoAmI);
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.PowerupCharge -= 30;
    }
}

internal class BoomerangFlowerProjectile : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<BoomerangFlower>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneJungle;
}

internal class BoomerangFlowerItem : PowerupItem
{
    internal override int? PowerupType => ModContent.GetInstance<BoomerangFlower>().Type;
}