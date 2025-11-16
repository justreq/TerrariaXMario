using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.PowerupProjectiles;
internal class TailSwipe : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = (int)(Main.player[Projectile.owner].width * 1.5f);
        Projectile.height = (int)(Main.player[Projectile.owner].height * 0.75f);
        Projectile.friendly = true;
        Projectile.timeLeft = 16;
        Projectile.penetrate = -1;
        Projectile.hide = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        Projectile.Center = player.Center + new Vector2(player.width * 1.25f * -player.direction, player.height * 0.15f);
        Projectile.spriteDirection = player.direction;
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

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }
}