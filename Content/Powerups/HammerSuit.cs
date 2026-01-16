using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
    internal override Dictionary<PowerupAbility, string> Abilities => new() { { PowerupAbility.Ranged, "Right click to throw an arching hammer" } };
    internal override void OnRightClick(Player player)
    {
        Vector2 magnitude = Main.MouseWorld - player.MountedCenter;
        Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, magnitude.SafeNormalize(Vector2.Zero) * new Vector2(15, 20), ModContent.ProjectileType<HammerSuitHammer>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPower ?? 1, 0f, player.whoAmI);
    }
}

internal class HammerSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<HammerSuitData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => player.ZoneDungeon || player.ZoneUndergroundDesert || player.ZoneUndergroundDesert;
}