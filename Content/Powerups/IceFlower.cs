using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Content.Powerups;
internal class IceFlowerData : Powerup
{
    public override string Name => "IceFlower";

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override bool OnLeftClick(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/IceFlowerShoot") { Volume = 0.4f });
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(player.direction * 2.5f, 0f), ModContent.ProjectileType<IceFlowerIceball>(), 1, 0f, player.whoAmI);

        return true;
    }
}

internal class IceFlower : PowerupProjectile
{
    internal override Powerup? PowerupData => ModContent.GetInstance<IceFlowerData>();
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}