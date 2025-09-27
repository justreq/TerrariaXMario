using Terraria;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class TanookiSuitData : Powerup
{
    public override string Name => "TanookiSuit";

    internal override string[] Caps => [nameof(Mario)];

    internal override string[] Variations => ["Flying"];

    internal override string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/TailPowerUp";

    internal override void UpdateWorld(Projectile projectile)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void UpdateConsumed(Player player)
    {

    }

    internal override void OnLeftClick(Player player)
    {

    }
}

internal class TanookiSuit : PowerupProjectile
{
    internal override Powerup PowerupData { get; set; } = new TanookiSuitData();
}