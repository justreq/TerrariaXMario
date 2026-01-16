using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
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
    internal override Dictionary<PowerupAbility, string> Abilities => new() { { PowerupAbility.Ranged, "Right click to throw bouncing iceballs that freeze enemies (frozen enemies can be picked up and thrown for extra damage)" } };
    internal override void OnRightClick(Player player)
    {
        if (Main.projectile.Any(e => e.type == ModContent.ProjectileType<IceFlowerIceball>() && e.active && e.owner == player.whoAmI)) return;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/IceFlowerShoot") { Volume = 0.4f }, player.MountedCenter);
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, new Vector2(player.direction * 2.5f, 0f), ModContent.ProjectileType<IceFlowerIceball>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPower ?? 1, 0f, player.whoAmI);
    }
}

internal class IceFlower : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<IceFlowerData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneSnow;
}