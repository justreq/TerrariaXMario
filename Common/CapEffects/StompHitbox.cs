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

    private bool groundPound;
    private int groundPoundCooldown;

    private void Stomp(Player player)
    {
        player.GetModPlayerOrNull<JumpPlayer>()?.currentJump = Jump.None;
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
        Projectile.timeLeft = 1;
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
        else player.headPosition.X = 0;

        CapEffectsPlayer? capEffectsPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (player.controlDown && (!capEffectsPlayer?.crouching ?? false) && !groundPound)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPoundStart") { Volume = 0.4f });
            player.fullRotation = 0;
            groundPound = true;
        }

        if (groundPound)
        {
            player.creativeGodMode = true;
            player.bodyFrame.Y = 0;
            player.legFrame.Y = 0;
            player.headPosition.X = 4 * player.direction;
            player.sitting.isSitting = true;

            if (groundPoundCooldown <= 15) player.velocity = new(0, 0.1f);
            else if (player.controlDown) player.velocity.Y = player.maxFallSpeed;
        }

        capEffectsPlayer?.groundPounding = groundPound;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetInstantKill();
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
        if (target.GetGlobalNPCOrNull<IceBlockNPC>()?.frozen ?? false) return groundPound;

        return groundPound || (targetIndex == null && stompCooldown == 0 && Projectile.Hitbox.Intersects(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, 4)));
    }

    public override void OnKill(int timeLeft)
    {
        if (!groundPound) return;

        Player player = Main.player[Projectile.owner];

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPound") { Volume = 0.4f });
        for (int i = -2; i < 3; i++)
        {
            if (i == 0) continue;
            Dust.NewDustPerfect(player.Bottom, ModContent.DustType<ImpactDust>(), new Vector2(1.5f * Math.Sign(i), Main.rand.NextFloat(-1f, -0.5f)));
        }

    }
}