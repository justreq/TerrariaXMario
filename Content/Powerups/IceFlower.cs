using Terraria;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class IceFlowerData : Powerup
{
    public override string Name => "IceFlower";

    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];

    internal override void UpdateWorld(Projectile projectile)
    {
        projectile.velocity.Y += 0.4f;
    }
}

internal class IceFlower : PowerupProjectile<IceFlowerData>;