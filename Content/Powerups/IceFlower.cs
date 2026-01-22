using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class IceFlower : FireFlower
{
    internal override int? ProjectileType => ModContent.ProjectileType<IceFlowerProjectile>();
    internal override int? ItemType => ModContent.ItemType<IceFlowerItem>();
    internal override Color Color => new(55, 118, 242);
    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.fireFlowerFireballsCast > 1) return;

        modPlayer?.fireFlowerFireballsCast += 1;
        if (modPlayer?.fireFlowerCooldown == 0) modPlayer?.fireFlowerCooldown = 50;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/IceFlowerShoot") { Volume = 0.4f }, player.MountedCenter);
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, GetInitialProjectileVelocity(player, 0.2f), ModContent.ProjectileType<IceFlowerIceball>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1, 0f, player.whoAmI);
        modPlayer?.PowerupCharge -= 40;
    }
}

internal class IceFlowerProjectile : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<IceFlower>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneSnow;
}

internal class IceFlowerItem : PowerupItem
{
    internal override int? PowerupType => ModContent.GetInstance<IceFlower>().Type;
}