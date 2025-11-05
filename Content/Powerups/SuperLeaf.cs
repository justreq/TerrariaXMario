using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class SuperLeafData : Powerup
{
    protected override void Register()
    {
        SpawnUpSpeed = -5f;
        SpawnDownSpeed = 2f;
        TimeBeforePickable = 20;
    }

    public override string Name => "SuperLeaf";

    internal override string[] Caps => [nameof(Mario)];

    internal override string[] Variations => ["Flying"];

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
    internal override Powerup PowerupData { get; set; } = ModContent.GetInstance<SuperLeafData>();

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
    }
}