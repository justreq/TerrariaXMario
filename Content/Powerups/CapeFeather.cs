using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;
internal class CapeFeatherData : SuperLeafData
{
    public override string Name => "CapeFeather";

    internal override string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/CapePowerUp";

    internal override bool ShowTail => false;
    internal override bool ShowCape => true;

    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if ((!modPlayer?.CanShowCape ?? true) || player.fullRotation != 0) return;
        modPlayer?.forceSwitchDirectionCount = 2;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/CapeSpin") { Volume = 0.4f });
        modPlayer?.SetForceDirection(10, ForceArmMovementType.None, -player.direction);
        Projectile.NewProjectile(player.GetSource_Misc("CapeSpin"), player.Center, Vector2.Zero, ModContent.ProjectileType<CapeSpin>(), 1, 7.5f, player.whoAmI);
    }

    private readonly float[] fallSpeedMultipliers = [1, 3, 3, 3.5f, 3.5f, 4];
    private readonly float[] gravityMultipliers = [1, 1, 3, 4, 5, 6];

    internal override void OnJumpHeldDown(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        if (player.IsOnGroundPrecise() || modPlayer == null) return;

        switch (modPlayer.flightState)
        {
            case FlightState.None:
                if (modPlayer.hasPSpeed)
                {
                    modPlayer.flightState = FlightState.Flying;
                    modPlayer.capeRiseToFlightTimer = 80;
                }
                else if (player.velocity.Y > 0) modPlayer.flightState = FlightState.Gliding;

                break;
            case FlightState.Gliding:
                modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = "Flying";
                if (player.velocity.Y > 0) player.velocity.Y = 1;
                break;
            case FlightState.Flying:
                modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Flying";

                if (modPlayer.capeRiseToFlightTimer > 0)
                {
                    modPlayer.capeRiseToFlightTimer--;
                    player.velocity.Y = -8;
                }
                else if (!modPlayer.doCapeFlight && player.velocity.Y > 0) modPlayer.doCapeFlight = true;
                else if (modPlayer.doCapeFlight)
                {
                    player.velocity.X = 4 * player.direction * fallSpeedMultipliers[modPlayer.CapeFrame] + 1;
                    player.maxFallSpeed = 2 * fallSpeedMultipliers[modPlayer.CapeFrame] + 1;
                    player.gravity = modPlayer.CapeFrame == 0 && modPlayer.capeBoostTimer > 0 ? -0.1f : 0.05f * gravityMultipliers[modPlayer.CapeFrame] + 1;
                }

                if (modPlayer.runTime <= 0)
                {
                    modPlayer.hasPSpeed = false;
                    modPlayer.flightState = FlightState.None;
                    modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = null;
                }
                break;
        }
    }
}

internal class CapeFeather : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<CapeFeatherData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override float SpawnUpSpeed => -5f;
    internal override float SpawnDownSpeed => 2f;
    internal override int TimeBeforePickable => 20;
    internal override bool Legs => false;
    internal override bool Body => false;
    internal override bool Head => false;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
    }
}