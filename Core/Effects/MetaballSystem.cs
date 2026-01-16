using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.MetaballContent;
using TerrariaXMario.Core.Effects;

namespace TerrariaXMario.Core.Effects;

static class MathPlusTM
{
    extension(Point)
    {
        public static Point operator /(Point left, int right) => new(left.X / right, left.Y / right);
    }
}
internal class MetaballSystem : ModSystem // credits Lolxd87
{
    public override void Load()
    {
        Main.QueueMainThreadAction(() =>
        {
            Main.OnPreDraw += Main_OnPreDraw;
        });
    }

    private void Main_OnPreDraw(GameTime obj)
    {
        PaintTargets(Main.ScreenSize);
    }

    public override void Unload()
    {
        Main.QueueMainThreadAction(() =>
        {
            Main.OnPreDraw -= Main_OnPreDraw;
            target.Dispose();
            pixellationTargetInner.Dispose();
        });
    }

    private RenderTarget2D target = null!;
    private readonly RenderTarget2D unused2 = null!;
    private readonly RenderTarget2D unused1 = null!;
    private RenderTarget2D pixellationTargetInner = null!;

    private static void Resize(ref RenderTarget2D target, Point screenSize)
    {
        if (target == null || target.Width != screenSize.X || target.Height != screenSize.Y)
        {
            target?.Dispose();
            target = new RenderTarget2D(Main.instance.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
        }
    }
    private void PaintTargets(Point screenSize)
    {
        previousScreenPosition = Main.screenPosition;
        var gd = Main.instance.GraphicsDevice;
        var sb = Main.spriteBatch;

        Resize(ref target, screenSize);
        Resize(ref pixellationTargetInner, screenSize / 2);

        var oldrt = gd.GetRenderTargets();

        Asset<Texture2D> inner = ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/Metaball", AssetRequestMode.ImmediateLoad);
        Asset<Texture2D> outer = inner;
        Draw(isOuter: false);
        void Draw(bool isOuter)
        {
            gd.SetRenderTarget(isOuter ? unused2 : target);
            gd.Clear(Color.Transparent);
            gd.BlendFactor = Color.Black;

            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer);

            for (int i = 0; i < Main.dust.Length; i++)
            {
                if (Main.dust[i] is { active: true, } dust && DustLoader.GetDust(dust.type) is MetaballDust metaballDust)
                {
                    Texture2D tex = isOuter ? outer.Value : inner.Value;
                    Color color = isOuter ? metaballDust.OutlineColor : metaballDust.FillColor;

                    float scaleFactor = isOuter ? 1.3f : 1f;
                    Vector2 center = dust.position;

                    Vector2 targetSize = new(metaballDust.Radius);
                    Vector2 scale = targetSize / tex.Size() * scaleFactor;

                    Vector2 velocityScaleFactor = dust.velocity.SafeNormalize(Vector2.One);
                    velocityScaleFactor.X += MathF.Sign(velocityScaleFactor.X);
                    velocityScaleFactor.Y += MathF.Sign(velocityScaleFactor.Y);
                    velocityScaleFactor.X = MathF.Abs(velocityScaleFactor.X);
                    velocityScaleFactor.Y = MathF.Abs(velocityScaleFactor.Y);
                    scale *= (velocityScaleFactor);
                    float rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;

                    sb.Draw(tex, center - Main.screenPosition, null, color, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }

            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is MetaballProjectile metaballProjectile)
                {
                    Texture2D tex = isOuter ? outer.Value : inner.Value;
                    Color color = isOuter ? metaballProjectile.OutlineColor : metaballProjectile.FillColor;

                    float scaleFactor = isOuter ? 1.3f : 1f;
                    Vector2 center = projectile.position;

                    Vector2 targetSize = new(metaballProjectile.Radius);
                    Vector2 scale = targetSize / tex.Size() * scaleFactor;

                    Vector2 velocityScaleFactor = projectile.velocity.SafeNormalize(Vector2.One);
                    velocityScaleFactor.X += MathF.Sign(velocityScaleFactor.X);
                    velocityScaleFactor.Y += MathF.Sign(velocityScaleFactor.Y);
                    velocityScaleFactor.X = MathF.Abs(velocityScaleFactor.X);
                    velocityScaleFactor.Y = MathF.Abs(velocityScaleFactor.Y);
                    scale *= (velocityScaleFactor);
                    float rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    sb.Draw(tex, center - Main.screenPosition, null, color, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }

            sb.End();
        }

        DrawPixellated(false);
        void DrawPixellated(bool isOuter)
        {
            RenderTarget2D current = isOuter ? unused1 : pixellationTargetInner;
            gd.SetRenderTarget(current);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer);
            sb.Draw(isOuter ? unused2 : target, current.Bounds, Color.White);
            sb.End();
        }

        gd.SetRenderTargets(oldrt);
    }

    Vector2 previousScreenPosition;
    public override void PostDrawTiles()
    {
        Asset<Effect> shader = ModContent.Request<Effect>($"{GetType().Namespace!.Replace(".", "/")}/MetaballShader", AssetRequestMode.ImmediateLoad);
        Effect effect = shader.Value;
        effect.Parameters["uInnerThreshold"].SetValue(0.3f);
        //effect.Parameters["uInnerColor"].SetValue(Color.Orange.ToVector4());
        effect.Parameters["uOuterThreshold"].SetValue(0.15f);
        //effect.Parameters["uOuterColor"].SetValue(Color.Red.ToVector4());

        var sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

        Vector2 position = Main.screenPosition - previousScreenPosition;
        Vector2 scale = Main.ScreenSize.ToVector2() / pixellationTargetInner.Size();
        sb.Draw(pixellationTargetInner, -position, null, Color.White, 0, default, scale, SpriteEffects.None, 0);
        //sb.Draw(pixellationTargetInner, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White);
        sb.End();
    }
}