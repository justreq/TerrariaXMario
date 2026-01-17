using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class HammerSuitData : FireFlowerData
{
    public override string Name => "HammerSuit";

    internal override int RightClickActionCooldown => 8;
    internal override Color Color => new(76, 76, 109);
    internal override void OnRightClick(Player player)
    {
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, GetInitialProjectileVelocity(player, 0.3f), ModContent.ProjectileType<HammerSuitHammer>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatPower ?? 1, 0f, player.whoAmI);
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.PowerupCharge -= 20;
    }
}

internal class HammerSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<HammerSuitData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneDungeon || player.ZoneUndergroundDesert || player.ZoneUndergroundDesert;
}