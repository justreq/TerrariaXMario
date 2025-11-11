using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class SuperLeafData : Powerup
{
    public override string Name => "SuperLeaf";

    internal override string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/TailPowerUp";

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.X = (float)(Math.Sin(MathHelper.Pi / 60 * updateCount % 60) * (updateCount <= 60 ? 1.5f : 3));
        projectile.velocity.Y = (60 - (updateCount % 60)) * 0.025f;

        projectile.frame = 1;
        projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
    }

    internal override void UpdateConsumed(Player player)
    {

    }

    internal override bool OnLeftClick(Player player)
    {
        return false;
    }
}

internal class SuperLeaf : PowerupProjectile
{
    internal override Powerup? PowerupData => ModContent.GetInstance<SuperLeafData>();
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override string[] Variations => ["Flying"];
    internal override float SpawnUpSpeed => -5f;
    internal override float SpawnDownSpeed => 2f;
    internal override int TimeBeforePickable => 20;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
    }
}