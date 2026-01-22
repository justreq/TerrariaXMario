using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;

internal class TanookiSuit : SuperLeaf
{
    internal override int? ProjectileType => ModContent.ProjectileType<TanookiSuitProjectile>();
    internal override int? ItemType => ModContent.ItemType<TanookiSuitItem>();
    internal override Color Color => new(255, 90, 9);
    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnJumpHeldDown(Player player)
    {
        DoJumpHold(player, 3);
    }
}

internal class TanookiSuitProjectile : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<TanookiSuit>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override string[] Variations => ["Flying"];
    internal override bool CanSpawn(Player player) => (player.ZoneForest || player.ZoneJungle) && !Main.dayTime;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 28;
    }
}

internal class TanookiSuitItem : PowerupItem
{
    internal override int? PowerupType => ModContent.GetInstance<TanookiSuit>().Type;
}