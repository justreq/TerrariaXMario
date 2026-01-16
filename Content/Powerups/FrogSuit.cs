using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class FrogSuitData : Powerup
{
    public override string Name => "FrogSuit";
    internal override bool LookTowardRightClick => false;
    internal override Color Color => new(22, 176, 67);

    internal override Dictionary<PowerupAbility, string> Abilities => new() { { PowerupAbility.Dash, "Double tap left or right to dash into a run" }, { PowerupAbility.Swim, "Swim with ease in all 4 directions" } };
    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void UpdateConsumed(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        player.dashType = DashID.ShieldOfCthulhu;
        player.jumpSpeedBoost += 0.75f;
        player.iceSkate = true;
        player.accDivingHelm = true;
        player.ignoreWater = true;
        player.waterWalk = modPlayer?.CurrentVariants.Contains("Running") ?? false;

        if (modPlayer == null) return;

        if (modPlayer.frogSwimming)
        {
            modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = null;
        }
        else if (player.timeSinceLastDashStarted == 1)
        {
            modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Running";
        }

        if (player.mount.Active) modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Running";
        else modPlayer.frogSwimming = player.wet;

        if (modPlayer.frogSwimming)
        {
            Vector2 velocity = new Vector2((player.controlLeft ? -1 : 0) + (player.controlRight ? 1 : 0), ((player.controlUp || player.controlJump ? -1 : 0) + (player.controlDown ? 1 : 0)) * player.gravDir).SafeNormalize(Vector2.Zero) * 5f;

            player.velocity = velocity == Vector2.Zero ? Vector2.Lerp(player.velocity, velocity, 0.05f) : velocity;
            player.gravity = 0;
        }

        if (!player.wet)
        {
            modPlayer.frogSwimFrame = 0;
            player.gravity = Player.defaultGravity;
        }
    }
}

internal class FrogSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<FrogSuitData>().Type;
    internal override string[] Caps => [nameof(Mario)];
    internal override string[] Variations => ["Running"];
    internal override bool CanSpawn(Player player) => player.ZoneBeach;
    internal override bool GroundPound => false;
}