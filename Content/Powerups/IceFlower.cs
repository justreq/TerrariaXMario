using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Content.Powerups;

internal class IceFlowerData : FireFlowerData
{
    public override string Name => "IceFlower";

    internal override void OnRightClick(Player player)
    {
        if (Main.projectile.Any(e => e.type == ModContent.ProjectileType<IceFlowerIceball>() && e.active && e.owner == player.whoAmI)) return;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/IceFlowerShoot") { Volume = 0.4f }, player.Center);
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(player.direction * 2.5f, 0f), ModContent.ProjectileType<IceFlowerIceball>(), 1, 0f, player.whoAmI);
    }
}

internal class IceFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<IceFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}