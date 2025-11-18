using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.PowerupProjectiles;
public class BoomerangFlowerBoomerang : ModProjectile
{
    private enum State { Travelling, Waiting, Returning }

    private State state = State.Travelling;

    private int stateTimer = 30;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 8;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
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
        Rectangle destinationRect = Projectile.getRect();
        destinationRect.X -= (int)Main.screenPosition.X;
        destinationRect.Y -= (int)Main.screenPosition.Y;
        Rectangle sourceRect = new(0, Projectile.frame * Projectile.height + Projectile.frame * 2, Projectile.width, Projectile.height);
        Main.EntitySpriteDraw(new(texture.Value, destinationRect, sourceRect, modPlayer == null ? lightColor : lightColor.MultiplyRGB(TerrariaXMario.capColors[modPlayer.currentCap!])));
        sourceRect.X += Projectile.width + 2;
        Main.EntitySpriteDraw(new(texture.Value, destinationRect, sourceRect, lightColor));
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
