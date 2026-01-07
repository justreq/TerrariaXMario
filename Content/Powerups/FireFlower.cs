using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FireFlowerData : Powerup
{
    public override string Name => "FireFlower";

    internal override ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.Swing;
    internal override Color Color => new(234, 51, 34);
    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.fireFlowerFireballsCast > 1) return;

        modPlayer?.fireFlowerFireballsCast += 1;
        if (modPlayer?.fireFlowerCooldown == 0) modPlayer?.fireFlowerCooldown = 30;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/FireFlowerShoot") { Volume = 0.4f }, player.MountedCenter);
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, new Vector2(player.direction * 5, 0f), ModContent.ProjectileType<FireFlowerFireball>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPower ?? 1, 0f, player.whoAmI);
    }
}

internal class FireFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<FireFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}