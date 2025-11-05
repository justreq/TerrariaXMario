using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class TanookiSuitData : Powerup
{
    public override string Name => "TanookiSuit";

    internal override string[] Caps => [nameof(Mario)];

    internal override string[] Variations => ["Flying"];

    internal override string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/TailPowerUp";

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void UpdateConsumed(Player player)
    {

    }

    internal override bool OnLeftClick(Player player)
    {
        return false;
    }
}

internal class TanookiSuit : PowerupProjectile
{
    internal override Powerup PowerupData { get; set; } = ModContent.GetInstance<TanookiSuitData>();
}