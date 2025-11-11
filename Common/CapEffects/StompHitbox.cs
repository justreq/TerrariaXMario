using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class StompHitbox : ModProjectile
{
    private int stompCount;
    private int stompCooldown;
    private int? targetIndex;

    internal bool groundPound;
    private int groundPoundCooldown;

    private void Stomp(Player player)
    {
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentJump = Jump.None;
        player.velocity.Y = player.controlJump ? -(7.5f + player.jumpSpeedBoost) : -5;

        stompCooldown = 5;
        stompCount++;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(stompCount > 7 ? "Heal" : $"Stomp{stompCount}")}") { Volume = 0.4f });

        if (stompCount > 7) player.Heal(1);
    }

    public override bool PreDraw(ref Color lightColor) => false;

    public override void SetDefaults()
    {
        Projectile.width = Main.player[Projectile.owner].width;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        Projectile.timeLeft++;
        Player player = Main.player[Projectile.owner];

        if (player.IsOnGroundPrecise()) Projectile.Kill();

        if (stompCooldown > 0) stompCooldown--;
        else if (targetIndex != null) targetIndex = null;

        Projectile.position = player.BottomLeft;

        if (groundPound && groundPoundCooldown > 15 && !player.controlDown)
        {
            groundPound = false;
            groundPoundCooldown = 0;
        }

        if (groundPound) groundPoundCooldown++;

        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (groundPound)
        {
            player.creativeGodMode = true;
            player.bodyFrame.Y = 0;
            player.legFrame.Y = Math.Clamp(groundPoundCooldown / 8, 0, 3) * 56;
            player.itemTime = 0;
            player.itemAnimation = 0;
            modPlayer?.currentJump = Jump.None;

            if (groundPoundCooldown <= 15)
            {
                player.velocity = new(0, 0.1f);
                player.fullRotationOrigin = new Vector2(player.Size.X * 0.5f, player.Size.Y * 0.75f);

                if (groundPoundCooldown <= 5) player.fullRotation = player.direction * (groundPoundCooldown * MathHelper.Pi * 0.2f);
                else player.fullRotation = player.direction * (MathHelper.Pi + (groundPoundCooldown - 5) * MathHelper.Pi * 0.1f);
            }
            else if (player.controlDown)
            {
                player.fullRotation = 0;
                player.velocity.Y = player.maxFallSpeed;

                foreach (Point point in modPlayer?.HitObjectSpawnerBlocks(1, true) ?? [])
                {
                    modPlayer?.SpawnObjectFromBlock(point, true);
                }
            }
        }

        modPlayer?.GroundPounding = groundPound;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!groundPound) modifiers.DisableKnockback();
        else modifiers.HitDirectionOverride = -Math.Sign(Main.player[Projectile.owner].Center.X - target.Center.X);
        base.ModifyHitNPC(target, ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        targetIndex ??= target.whoAmI;

        if (!groundPound)
        {
            Stomp(Main.player[Projectile.owner]);

            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                Dust.NewDustPerfect(target.Top, ModContent.DustType<ImpactDust>(), new Vector2(Math.Sign(i), Main.rand.NextFloat(-0.5f, 0.5f)));
            }

            return;
        }

        IceBlockNPC? iceBlockNPC = target.GetGlobalNPCOrNull<IceBlockNPC>();

        if (iceBlockNPC?.frozen ?? false) iceBlockNPC.KillIceBlock(target);
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (target.friendly) return false;

        if (target.GetGlobalNPCOrNull<IceBlockNPC>()?.frozen ?? false) return groundPound;

        return groundPound || (targetIndex == null && stompCooldown == 0 && Projectile.Hitbox.Intersects(target.getRect()));
    }

    public override void OnKill(int timeLeft)
    {
        Player player = Main.player[Projectile.owner];
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.GroundPounding = false;

        if (!groundPound) return;

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPound") { Volume = 0.4f });
        for (int i = -2; i < 3; i++)
        {
            if (i == 0) continue;
            Dust.NewDustPerfect(player.Bottom, ModContent.DustType<ImpactDust>(), new Vector2(1.5f * Math.Sign(i), Main.rand.NextFloat(-1f, -0.5f)));
        }

    }
}