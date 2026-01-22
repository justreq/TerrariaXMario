using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class HammerSuit : FireFlower
{
    internal override int? ProjectileType => ModContent.ProjectileType<HammerSuitProjectile>();
    internal override int? ItemType => ModContent.ItemType<HammerSuitItem>();

    internal override int RightClickActionCooldown => 8;
    internal override Color Color => new(76, 76, 109);
    internal override void OnRightClick(Player player)
    {
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, GetInitialProjectileVelocity(player, 0.3f), ModContent.ProjectileType<HammerSuitHammer>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1, 0f, player.whoAmI);
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.PowerupCharge -= 20;
    }
}

internal class HammerSuitProjectile : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<HammerSuit>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneDungeon || player.ZoneUndergroundDesert || player.ZoneUndergroundDesert;
}

internal class HammerSuitItem : PowerupItem
{
    internal override int? PowerupType => ModContent.GetInstance<HammerSuit>().Type;
}