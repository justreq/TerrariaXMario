using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.PowerupProjectiles;
internal class CapeSpin : InteractiveWithObjectSpawnerTileProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 10;
    }
    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 36;
        Projectile.friendly = true;
        Projectile.timeLeft = 19;
        Projectile.penetrate = -1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture + Main.player[Projectile.owner].GetModPlayerOrNull<CapEffectsPlayer>()?.currentCap);
        Rectangle destinationRect = Projectile.getRect();
        destinationRect.X -= (int)((int)Main.screenPosition.X - Projectile.width * 0.5f);
        destinationRect.Y -= (int)((int)Main.screenPosition.Y - Projectile.height * 0.5f) - 7;
        Main.EntitySpriteDraw(new(texture.Value, destinationRect, new(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size * 0.5f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.spriteDirection = Main.player[Projectile.owner].direction;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (++Projectile.frameCounter >= 2)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        Vector2 offset = new(0, 3);

        if (Projectile.spriteDirection == 1)
        {
            if (Projectile.frame < 5) Projectile.Right = player.Center + offset;
            else Projectile.Left = player.Center + offset;
        }
        else
        {
            if (Projectile.frame < 5) Projectile.Left = player.Center + offset;
            else Projectile.Right = player.Center + offset;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.HitDirectionOverride = -Math.Sign(Main.player[Projectile.owner].Center.X - target.Center.X);
        base.ModifyHitNPC(target, ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        IceBlockNPC? iceBlockNPC = target.GetGlobalNPCOrNull<IceBlockNPC>();

        if (iceBlockNPC?.frozen ?? false) iceBlockNPC.KillIceBlock(target);
    }
}