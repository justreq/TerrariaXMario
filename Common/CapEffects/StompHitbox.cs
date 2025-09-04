using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class StompHitbox : ModProjectile
{
    private int stompCount;
    private int stompCooldown;
    private int? targetIndex;

    private bool groundPound;

    private void Stomp(Player player)
    {
        JumpPlayer? jumpPlayer = player.GetModPlayerOrNull<JumpPlayer>();

        if (jumpPlayer != null) jumpPlayer.currentJump = Jump.None;

        player.velocity.Y = player.controlJump ? -(7.5f + player.jumpSpeedBoost) : -5;

        stompCooldown = 5;
        stompCount++;
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(stompCount > 7 ? "Heal" : $"Stomp{stompCount}")}") { Volume = 0.4f });

        if (stompCount > 7) player.Heal(1);
    }

    public override string Texture => "Terraria/Images/Extra_176";

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

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

        if (stompCooldown > 0) stompCooldown--;
        else if (targetIndex != null) targetIndex = null;


        Projectile.position = player.BottomLeft;

        if (!player.controlDown)
        {
            player.headPosition.X = 0;
            groundPound = false;
        }

        CapEffectsPlayer? capEffectsPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (player.controlDown && (!capEffectsPlayer?.crouching ?? false))
        {
            if (!groundPound)
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPoundStart") { Volume = 0.4f });
                player.fullRotation = 0;
                groundPound = true;
            }
            else
            {
                player.creativeGodMode = true;
                player.bodyFrame.Y = 0;
                player.legFrame.Y = 0;
                player.headPosition.X = 4 * player.direction;
                player.sitting.isSitting = true;

                if (player.holdDownCardinalTimer[0] < 15) player.velocity = Vector2.Zero;
                else player.velocity.Y = player.maxFallSpeed;
            }
        }
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
        }
    }

    public override bool? CanHitNPC(NPC target)
    {
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