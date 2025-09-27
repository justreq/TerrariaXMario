using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Content.Powerups;
internal class FireFlowerData : Powerup
{
    public override string Name => "FireFlower";

    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];

    internal override void UpdateWorld(Projectile projectile)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnLeftClick(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/FireFlowerShoot") { Volume = 0.4f });
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(player.direction * 5, 0f), ModContent.ProjectileType<FireFlowerFireball>(), 1, 0f, player.whoAmI);
    }
}

internal class FireFlower : PowerupProjectile
{
    internal override Powerup PowerupData { get; set; } = new FireFlowerData();
}