using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal class SuperLeafData : Powerup
{
    public override string Name => "SuperLeaf";

    internal override string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/TailPowerUp";

    internal override bool LookTowardRightClick => false;
    internal override Color Color => new(176, 86, 40);
    internal override Dictionary<PowerupAbility, string> Abilities => new() { { PowerupAbility.Melee, "Right click to swipe your tail at enemies" }, { PowerupAbility.Glide, "Hold jump midair to glide down slowly" }, { PowerupAbility.Flight, "Hold jump with full P-Speed to start flying upwards" } };
    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.X = (float)(Math.Sin(MathHelper.Pi / 60 * updateCount % 60) * (updateCount <= 60 ? 2f : 3f));
        projectile.velocity.Y = (60 - (updateCount % 60)) * 0.025f;

        projectile.frame = 1;
        projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
    }

    internal override void UpdateConsumed(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.flightState == FlightState.None && !modPlayer.GroundPounding) modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = modPlayer.hasPSpeed ? "Flying" : null;
    }

    internal override void OnRightClick(Player player)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if ((!modPlayer?.CanShowTail ?? true) || player.fullRotation != 0) return;
        modPlayer?.forceSwitchDirectionCount = 2;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/TailSwipe") { Volume = 0.4f }, player.MountedCenter);
        modPlayer?.SetForceDirection(10, ForceArmMovementType.None, -player.direction);
        Projectile.NewProjectile(player.GetSource_Misc("TailSwipe"), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<TailSwipe>(), player.GetModPlayerOrNull<CapEffectsPlayer>()?.statPower ?? 1, 7.5f, player.whoAmI);
    }

    internal override void OnJumpHeldDown(Player player)
    {
        DoJumpHold(player, 2);
    }

    protected static void DoJumpHold(Player player, int runtimeDecayFactor)
    {
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        if (player.IsOnGroundPrecise() || player.wet || modPlayer == null) return;

        switch (modPlayer.flightState)
        {
            case FlightState.None:
                if (modPlayer.hasPSpeed) modPlayer.flightState = FlightState.Flying;
                else if (player.velocity.Y > 0) modPlayer.flightState = FlightState.Gliding;
                break;
            case FlightState.Gliding:
                modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Flying";
                if (player.velocity.Y > 0) player.velocity.Y = 1;

                if (!SoundEngine.TryGetActiveSound(modPlayer.glideFlySoundSlot, out var glideSound))
                {
                    modPlayer.glideFlySoundSlot = SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/TailGlide") { Volume = 0.4f }, player.MountedCenter);
                }
                break;
            case FlightState.Flying:
                modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = "Flying";
                player.velocity.Y = -2;

                if (Main.GameUpdateCount % runtimeDecayFactor == 0) modPlayer.runTime--;
                if (modPlayer.runTime <= 0)
                {
                    modPlayer.hasPSpeed = false;
                    modPlayer.flightState = FlightState.None;
                    modPlayer.currentHeadVariant = modPlayer.currentBodyVariant = modPlayer.currentLegsVariant = null;
                }

                if (!SoundEngine.TryGetActiveSound(modPlayer.glideFlySoundSlot, out var flySound))
                {
                    modPlayer.glideFlySoundSlot = SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/TailFly") { Volume = 0.4f }, player.MountedCenter);
                }
                break;
        }
    }
}

internal class SuperLeaf : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<SuperLeafData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override bool CanSpawn(Player player) => (player.ZoneForest || player.ZoneJungle) && Main.dayTime;
    internal override float SpawnUpSpeed => -5f;
    internal override float SpawnDownSpeed => 2f;
    internal override int TimeBeforePickable => 20;
    internal override bool Legs => false;
    internal override bool Body => false;
    internal override string[] Variations => ["Flying"];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
    }
}