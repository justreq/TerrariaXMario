using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

interface IDrawToDustMetaballsTarget
{
    void DrawToMetaballs(MetaballDust dustsThatWillBeDrawn, SpriteBatch sb, Texture2D metaballCircleTexture);
}
internal class MetaballSystem2 : ModSystem
{
    private Asset<Texture2D>? circleAsset;
    private Asset<Effect>? metaballShader;
    public override void Load()
    {
        circleAsset = ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/Metaball", AssetRequestMode.AsyncLoad);
        metaballShader = ModContent.Request<Effect>($"{GetType().Namespace!.Replace(".", "/")}/MetaballShader", AssetRequestMode.AsyncLoad);
        Main.QueueMainThreadAction(() =>
        {
            Main.OnPreDraw += Main_OnPreDraw_PaintTargets;
        });
    }
    public override void Unload()
    {
        Main.QueueMainThreadAction(() =>
        {
            Main.OnPreDraw -= Main_OnPreDraw_PaintTargets;
        });
    }

    private static void Resize([NotNull] ref RenderTarget2D? target, Point screenSize)
    {
        if (target == null || target.Width != screenSize.X || target.Height != screenSize.Y)
        {
            target?.Dispose();
            target = new RenderTarget2D(Main.instance.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
        }
    }

    // mainPixellationTarget as the name says, is pixellated
    private RenderTarget2D? mainPixellationTarget;
    // this one is used for drawing one dust type
    // when all dusts of that type have been drawn, this target is drawn to 'mainPixellationTarget'
    // And then next dust draws 
    private RenderTarget2D? targetPerDustType;
    private List<Dust> metaballDusts = new(128);
    private List<IDrawToDustMetaballsTarget> projectilesWithMetaballDraw = new(128);
    private Vector2 previousScreenPosition;
    private RenderTargetBinding[] previousTargets = [];
    private void Main_OnPreDraw_PaintTargets(GameTime obj)
    {
        if (circleAsset?.IsLoaded != true || metaballShader?.IsLoaded != true || Main.gameMenu)
            return;
        previousScreenPosition = Main.screenPosition;
        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice gd = Main.instance.GraphicsDevice;

        int targetCount = gd.GetRenderTargetsNoAllocEXT(null);
        if (previousTargets.Length != targetCount)
            Array.Resize(ref previousTargets, targetCount);
        gd.GetRenderTargetsNoAllocEXT(previousTargets);
        try
        {
            Effect shader = metaballShader.Value;
            Resize(ref mainPixellationTarget, Main.ScreenSize / 2);
            Resize(ref targetPerDustType, Main.ScreenSize);
            foreach (var proj in Main.projectile)
            {
                if (proj is { active: true, ModProjectile: IDrawToDustMetaballsTarget target })
                    projectilesWithMetaballDraw.Add(target);
            }
            foreach (var dust in Main.dust)
            {
                if (dust is { active: true } && DustLoader.GetDust(dust.type) is MetaballDust)
                {
                    metaballDusts.Add(dust);
                }
            }
            metaballDusts.Sort((a, b) => a.type.CompareTo(b.type));

            gd.SetRenderTarget(mainPixellationTarget);
            gd.Clear(Color.Transparent);

            BeginSBPerDustType();
            int previousType = -1;
            MetaballDust? previousMetaballDust = null;
            foreach (var dust in metaballDusts)
                Draw(dust);
            void Draw(Dust? dust)
            {
                if (dust == null)
                    return;
                bool newBatch = dust.type != previousType;
                MetaballDust metaballDust = (MetaballDust)DustLoader.GetDust(dust.type);
                Texture2D tex = circleAsset.Value;
                if (newBatch)
                {
                    FlushToMainTarget(true);
                    foreach (var proj in this.projectilesWithMetaballDraw)
                        proj.DrawToMetaballs(metaballDust, sb, tex);
                }

                metaballDust.DrawMetaball(dust, sb, tex);

                previousMetaballDust = metaballDust;
                previousType = dust.type;
            }
            FlushToMainTarget(false);
            void BeginSBPerDustType() => sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer);
            void BeginToMainTarget() => sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, shader);
            void FlushToMainTarget(bool continuing)
            {
                sb.End();
                if (previousMetaballDust != null)
                {
                    gd.SetRenderTarget(mainPixellationTarget);
                    // TODO: properties in MetaballDust to calculate this
                    shader.Parameters["uInnerThreshold"].SetValue(0.3f);
                    shader.Parameters["uOuterThreshold"].SetValue(0.15f);
                    shader.Parameters["uInnerColor"].SetValue(previousMetaballDust.FillColor.ToVector4());
                    shader.Parameters["uOuterColor"].SetValue(previousMetaballDust.OutlineColor.ToVector4());
                    BeginToMainTarget();
                    sb.Draw(targetPerDustType, mainPixellationTarget.Bounds, Color.White);
                    sb.End();
                }

                if (continuing)
                {
                    gd.SetRenderTarget(targetPerDustType);
                    gd.Clear(Color.Transparent);
                    BeginSBPerDustType();
                }
            }

        }
        finally
        {
            metaballDusts.Clear();
            projectilesWithMetaballDraw.Clear();
            gd.SetRenderTargets(previousTargets);
        }

    }

    public override void PostDrawTiles()
    {
        if (mainPixellationTarget == null) return;

        var sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        Vector2 position = Main.screenPosition - previousScreenPosition;
        Vector2 scale = Main.ScreenSize.ToVector2() / mainPixellationTarget.Size();
        sb.Draw(mainPixellationTarget, -position, null, Color.White, 0, default, scale, SpriteEffects.None, 0);
        sb.End();
    }

    // just to test the dust
    public override void PostUpdatePlayers()
    {
        //var dust = Main.dust[Dust.NewDust(Main.MouseWorld, 10, 10, ModContent.DustType<FireFlowerFireballDust>(), 0, 0, 0, default, 1)];
        //var dust2 = Main.dust[Dust.NewDust(Main.MouseWorld, 10, 10, ModContent.DustType<IceFlowerIceballDust>(), 0, 0, 0, default, 1)];
        //dust.velocity *= 2;
        //dust.noGravity = true;
        //dust.fadeIn = 4;
    }
}