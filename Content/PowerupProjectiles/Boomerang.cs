using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.PowerupProjectiles;

internal class Boomerang : InteractiveWithObjectSpawnerTileProjectile
{
    private enum State { Travelling, Waiting, Returning }

    private State state = State.Travelling;
    private int stateTimer = 20;

    private SlotId spinSoundSlot;
    private SpriteEffects effect;

    internal override float TileCheckExtent => 0.1f;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 8;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        if (++Projectile.frameCounter >= (state == State.Waiting ? 2 : 3))
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        if (Projectile.velocity != Vector2.Zero)
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            effect = Projectile.velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        }

        if (!SoundEngine.TryGetActiveSound(spinSoundSlot, out var spinSound))
        {
            spinSoundSlot = SoundEngine.PlaySound(new($"{GetType().FullName!.Replace(".", "/")}Spin") { Volume = 0.4f }, Main.player[Projectile.owner].Center);
        }

        switch (state)
        {
            case State.Travelling:
                stateTimer--;
                if (stateTimer <= 0)
                {
                    stateTimer = 60;
                    Projectile.velocity = Vector2.Zero;
                    state = State.Waiting;
                }
                break;
            case State.Waiting:
                stateTimer--;
                if (stateTimer <= 0) state = State.Returning;
                break;
            case State.Returning:
                Player player = Main.player[Projectile.owner];
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 8;
                if (Projectile.getRect().Intersects(player.getRect())) Projectile.Kill();
                break;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        CapEffectsPlayer? modPlayer = Main.player[Projectile.owner].GetModPlayerOrNull<CapEffectsPlayer>();
        Vector2 position = Projectile.Center - Main.screenPosition;
        Rectangle sourceRect = new(0, Projectile.frame * 20, 34, 20);
        Vector2 origin = new(17, 10);

        Main.EntitySpriteDraw(new(texture.Value, position, sourceRect, modPlayer == null ? lightColor : lightColor.MultiplyRGB(TerrariaXMario.capColors[modPlayer.currentCap!]), Projectile.rotation, origin, Projectile.scale, effect));
        sourceRect.X += 34;
        Main.EntitySpriteDraw(new(texture.Value, position, sourceRect, lightColor, Projectile.rotation, origin, Projectile.scale, effect));
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (state == State.Returning) return true;

        state = State.Returning;
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Smoke, 0f, 0f);
            dust.noGravity = true;
            dust.velocity *= 4f;
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.TreasureSparkle, 0f, 0f);
            dust2.noGravity = true;
            dust2.velocity *= 4f;
        }
    }
}
