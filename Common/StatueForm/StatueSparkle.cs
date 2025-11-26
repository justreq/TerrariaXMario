using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.StatueForm;
internal class StatueSparkle : ModProjectile // credits: Momlob
{
    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
        Projectile.timeLeft = 15;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

        // Smoke
        int _DustCount = 12;
        float _RandomAngleBase = Main.rand.NextFloat(MathHelper.PiOver4);
        Vector2 _DustPos = Projectile.Center + new Vector2(0, 5);
        for (int i = 0; i < _DustCount; i++)
        {
            Vector2 _Angle = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / _DustCount * i + _RandomAngleBase).RotatedByRandom(0.05f) * Main.rand.NextFloat(4.5f, 5f);
            Dust.NewDustPerfect(_DustPos, ModContent.DustType<StatueDust>(), _Angle);
            Vector2 _Angle2 = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / _DustCount * i + _RandomAngleBase + MathHelper.PiOver4 / 2).RotatedByRandom(0.05f) * Main.rand.NextFloat(6.5f, 7f);
            Dust.NewDustPerfect(_DustPos, ModContent.DustType<StatueDust>(), _Angle2);
        }
    }
    public override void AI()
    {
        Projectile.Center = Main.player[Projectile.owner].Center;
        Projectile.rotation += 0.05f;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 position = Projectile.Center - Main.screenPosition;
        Color color = Color.White;
        color.A = 100;

        // Blend
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        // Calc
        float _Scale = (float)Math.Sin((1 - (float)Projectile.timeLeft / 15) * MathHelper.Pi);

        // Spikes
        Texture2D _Texture = ModContent.Request<Texture2D>(Texture).Value;
        for (int i = -1; i <= 1; i += 2)
        {
            Main.EntitySpriteDraw(_Texture, position, _Texture.Frame(), color, Projectile.rotation * i, _Texture.Size() * 0.5f, _Scale * 1.5f, SpriteEffects.None);
        }

        // Blend Reset
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }
}
