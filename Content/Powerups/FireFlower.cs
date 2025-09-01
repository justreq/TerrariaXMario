using Terraria;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class FireFlower : PowerupProjectile
{
    internal override string[] Caps => [nameof(Mario)];

    public override void AI()
    {
        Projectile.velocity.Y += Player.defaultGravity;
    }
}