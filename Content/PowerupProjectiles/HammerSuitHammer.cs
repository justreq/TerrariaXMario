using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.PowerupProjectiles;
public class HammerSuitHammer : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 22;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
    }

    public override void AI()
    {
        Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
        Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.35f;
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
