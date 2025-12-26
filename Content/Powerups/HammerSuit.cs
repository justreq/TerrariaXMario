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

    internal override void OnRightClick(Player player)
    {
        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2((Main.MouseWorld.X - player.Center.X) / 50, -8f), ModContent.ProjectileType<HammerSuitHammer>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPower ?? 1, 0f, player.whoAmI);
    }
}

internal class HammerSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<HammerSuitData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
}