using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FrogSuitData : Powerup
{
    public override string Name => "FrogSuit";

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }
}

internal class FrogSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<FrogSuitData>().Type;
    internal override string[] Caps => [nameof(Mario)];
}