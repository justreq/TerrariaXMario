using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.PowerupProjectiles;
public class FireFlowerFireball : ModProjectile
{
    internal float gravity = 0.4f;
    internal float bounceSpeed = -5;
    internal int dustType = DustID.Torch;
    internal int dustChance = 3;

    internal int tileCollideCount = 0;

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
        Projectile.scale = 0.75f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
        {
            Projectile.velocity.Y = bounceSpeed;
            tileCollideCount++;
        }

        return false;
    }

    public override void AI()
    {
        Projectile.velocity.Y += gravity;
        Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.35f;

        if (Main.rand.NextBool(dustChance))
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, dustType, Scale: 2f);
            dust.noGravity = true;
            dust.velocity *= 4f;
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
        }
    }
}
