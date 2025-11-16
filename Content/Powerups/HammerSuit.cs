using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Content.Powerups;
internal class HammerSuitData : Powerup
{
    public override string Name => "HammerSuit";

    internal override ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.Swing;

    internal override int RightClickActionCooldown => 8;

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnRightClick(Player player)
    {
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2((Main.MouseWorld.X - player.Center.X) / 50, -8f), ModContent.ProjectileType<HammerSuitHammer>(), 1, 0f, player.whoAmI);
    }
}

internal class HammerSuit : PowerupProjectile
{
    internal override int? PowerupType => PowerupID.Search.GetId(typeof(HammerSuitData).FullName);
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}