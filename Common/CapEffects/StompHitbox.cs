using Microsoft.Xna.Framework;
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
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (stompCooldown > 0) stompCooldown--;
        else if (targetIndex != null) targetIndex = null;

        Projectile.position = player.BottomLeft;

        if (player.controlDown && (!player.GetModPlayerOrNull<CapEffectsPlayer>()?.crouching ?? false))
        {
            if (!groundPound)
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPoundStart") { Volume = 0.4f });
                groundPound = true;
            }
            else
            {
                player.creativeGodMode = true;

                if (player.holdDownCardinalTimer[0] < 15) player.velocity = Vector2.Zero;
                else player.velocity.Y = player.maxFallSpeed;
            }
        }

        if (!player.controlDown) groundPound = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetInstantKill();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        targetIndex ??= target.whoAmI;
        if (!groundPound) Stomp(Main.player[Projectile.owner]);
    }

    public override bool? CanHitNPC(NPC target)
    {
        return groundPound || (targetIndex == null && stompCooldown == 0 && Projectile.Hitbox.Intersects(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, 4)));
    }

    public override void OnKill(int timeLeft)
    {
        if (groundPound) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPound") { Volume = 0.4f });
    }
}