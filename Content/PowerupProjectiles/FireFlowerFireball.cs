using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.MetaballContent;

namespace TerrariaXMario.Content.PowerupProjectiles;

internal class FireFlowerFireball : MetaballProjectile
{
    internal override Color OutlineColor => new(249, 28, 26);
    internal override Color FillColor => new(246, 225, 21);
    internal override float Radius => 10;

    internal float gravity = 0.4f;
    internal float bounceSpeed = -5;
    internal int dustType = DustID.Torch;
    internal int dustChance = 3;

    internal int tileCollideCount = 0;

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

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FireFlowerFireballDust>());
                dust.velocity = Vector2.Zero;
                dust.scale = Main.rand.NextFloat(0.7f, 0.8f);
            }
        }

        return false;
    }

    public override void AI()
    {
        Projectile.velocity.Y += gravity;

        if (Main.rand.NextBool(dustChance))
        {
            //Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, dustType, Scale: 2f);
            //dust.noGravity = true;
            //dust.velocity *= 4f;

            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<FireFlowerFireballDust>());
            dust2.velocity = Vector2.Zero;
            dust2.scale = Main.rand.NextFloat(0.5f, 0.6f);
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

            Dust dust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FireFlowerFireballDust>());
            dust3.velocity = Vector2.Zero;
            dust3.scale = Main.rand.NextFloat(0.7f, 0.8f);
        }
    }
}
