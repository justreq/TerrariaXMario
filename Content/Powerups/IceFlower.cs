using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class IceFlowerData : FireFlowerData
{
    public override string Name => "IceFlower";
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

internal class IceFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<IceFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneSnow;
}