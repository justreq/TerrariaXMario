using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.MetaballContent;
using TerrariaXMario.Core.Effects;

namespace TerrariaXMario.Content.PowerupProjectiles;

internal class FireFlowerFireball : MetaballProjectile, IDrawToDustMetaballsTarget
{
    internal override Color OutlineColor => new(249, 28, 26);
    internal override Color FillColor => new(246, 225, 21);
    internal override float Radius => 16;
    internal override int? PairedMetaballDust => ModContent.DustType<FireFlowerFireballDust>();

    internal float gravity = 0.4f;
    internal float bounceSpeed = -5;
    internal int dustType = DustID.Torch;
    internal int dustChance = 1;

    internal int tileCollideCount = 0;

    public void DrawToMetaballs(MetaballDust dustsThatWillBeDrawn, SpriteBatch sb, Texture2D metaballCircleTexture)
    {
        if (dustsThatWillBeDrawn.Type != PairedMetaballDust)
            return;
        Vector2 scale = new Vector2(20) / metaballCircleTexture.Size() * Projectile.scale * new Vector2(2, 1);
        sb.Draw(metaballCircleTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver2, metaballCircleTexture.Size() / 2, scale, SpriteEffects.None, 0);
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
        {
            Projectile.velocity.Y = bounceSpeed;
            tileCollideCount++;

            if (PairedMetaballDust != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)PairedMetaballDust);
                    dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
                }
            }
        }

        return false;
    }

    public override void AI()
    {
        Projectile.velocity.Y += gravity;

        if (Main.rand.NextBool(dustChance) && PairedMetaballDust != null)
        {
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, (int)PairedMetaballDust);
            dust2.velocity = Vector2.Zero;
        }

        if (Projectile.velocity.X == 0f) Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, dustType, Scale: 3f);
            dust.noGravity = true;
            dust.velocity *= 4f;
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Smoke, 0f, 0f);
            dust2.noGravity = true;
            dust2.velocity *= 4f;

            if (PairedMetaballDust == null) continue;

            Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)PairedMetaballDust);
            dust3.scale = Main.rand.NextFloat(0.7f, 0.8f);
        }
    }
}
