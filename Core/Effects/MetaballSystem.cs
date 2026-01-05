using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.MetaballContent;

namespace TerrariaXMario.Core.Effects;

internal class MetaballSystem : ModSystem
{
    internal static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

    RenderTargetBinding[] oldRenderTargets = [];
    internal static RenderTarget2D? outlineRenderTarget;
    internal static RenderTarget2D? fillRenderTarget;
    internal static RenderTarget2D? pixellationRenderTarget;
    internal static Asset<Effect> MetaballShader => ModContent.Request<Effect>($"{nameof(TerrariaXMario)}/Core/Effects/MetaballShader", AssetRequestMode.ImmediateLoad);
    internal static Asset<Texture2D> MetaballTexture => ModContent.Request<Texture2D>($"{nameof(TerrariaXMario)}/Core/Effects/Metaball", AssetRequestMode.ImmediateLoad);

    internal static float PixellationFactor => 2;
    internal static Vector2 AdjustedScreenSize => Main.ScreenSize.ToVector2() / PixellationFactor;

    public override void Load()
    {
        Main.QueueMainThreadAction(() =>
        {
            outlineRenderTarget = new(GraphicsDevice, Main.screenWidth, Main.screenHeight);
            fillRenderTarget = new(GraphicsDevice, Main.screenWidth, Main.screenHeight);
            pixellationRenderTarget = new(GraphicsDevice, (int)AdjustedScreenSize.X, (int)AdjustedScreenSize.Y);
        });

        Main.OnPreDraw += Main_OnPreDraw;
    }

    private void Main_OnPreDraw(GameTime obj)
    {
        oldRenderTargets = [.. GraphicsDevice.GetRenderTargets()];

        for (int i = 0; i < 2; i++)
        {
            GraphicsDevice.SetRenderTarget(i == 0 ? outlineRenderTarget : fillRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is not MetaballProjectile metaballProjectile) continue;

                DrawMetaballProjectile(metaballProjectile, i != 0);
            }

            foreach (Dust dust in Main.dust)
            {
                DrawMetaballDust(dust, i != 0);
            }
        }

        GraphicsDevice.SetRenderTarget(pixellationRenderTarget);
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, RasterizerState.CullNone);
        Main.spriteBatch.Draw(outlineRenderTarget, new Rectangle(0, 0, (int)AdjustedScreenSize.X, (int)AdjustedScreenSize.Y), Color.White);
        Main.spriteBatch.Draw(fillRenderTarget, new Rectangle(0, 0, (int)AdjustedScreenSize.X, (int)AdjustedScreenSize.Y), Color.White);
        Main.spriteBatch.End();
        GraphicsDevice.SetRenderTargets(oldRenderTargets);
        oldRenderTargets = [];
    }

    public override void PostDrawTiles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(pixellationRenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        Main.spriteBatch.End();
    }

    private static void DrawMetaballProjectile(MetaballProjectile metaball, bool fill)
    {
        Projectile projectile = metaball.Projectile;
        Color color = new Color[] { metaball.OutlineColor, metaball.FillColor }[fill.ToInt()];
        float scale = !fill ? 1 : 0.75f;
        Vector2 position = projectile.position.ToScreenPosition();
        Vector2 scaleFactor = projectile.velocity == Vector2.Zero ? Vector2.One : new((float)Math.Cbrt(projectile.velocity.X), (float)Math.Cbrt(projectile.velocity.Y));

        //VertexPositionColorTexture[] vertices = [
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(-metaball.Radius), 0), color, Vector2.Zero), // tl
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(metaball.Radius, -metaball.Radius), 0), color, Vector2.UnitX), // tr
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(metaball.Radius), 0), color, Vector2.One), // br
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(-metaball.Radius, metaball.Radius), 0), color, Vector2.UnitY), // bl
        //];

        MetaballShader.Value.Parameters["worldViewProjection"].SetValue(Matrix.CreateTranslation(
            new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * // world
            Main.GameViewMatrix.TransformationMatrix *
            Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1) // projection
            );
        MetaballShader.Value.Parameters["color"].SetValue(color.ToVector4());
        MetaballShader.Value.Parameters["radius"].SetValue(metaball.Radius);
        MetaballShader.Value.Parameters["scale"].SetValue(projectile.scale * scale);
        MetaballShader.Value.CurrentTechnique.Passes[0].Apply();

        Main.spriteBatch.Begin();
        Main.spriteBatch.Draw(MetaballTexture.Value, new Rectangle((int)position.X, (int)position.Y, (int)(metaball.Radius * 2 * projectile.scale * scaleFactor.X), (int)(metaball.Radius * 2 * projectile.scale * scaleFactor.Y)), null, color, projectile.rotation, new Vector2(metaball.Radius) * 0.5f, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        //GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, [0, 1, 2, 2, 3, 0], 0, 2);
    }

    private static void DrawMetaballDust(Dust dust, bool fill)
    {
        if (ModContent.GetModDust(dust.type) is not MetaballDust metaball || !dust.active) return;

        Color color = new Color[] { metaball.OutlineColor, metaball.FillColor }[fill.ToInt()];
        float scale = !fill ? 1 : 0.25f;
        Vector2 position = dust.position.ToScreenPosition() - new Vector2(metaball.Radius);
        Vector2 scaleFactor = dust.velocity == Vector2.Zero ? Vector2.One : new((float)Math.Cbrt(dust.velocity.X), (float)Math.Cbrt(dust.velocity.Y));

        //VertexPositionColorTexture[] vertices = [
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(-metaball.Radius), 0), color, Vector2.Zero), // tl
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(metaball.Radius, -metaball.Radius), 0), color, Vector2.UnitX), // tr
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(metaball.Radius), 0), color, Vector2.One), // br
        //    new VertexPositionColorTexture(new Vector3(position + new Vector2(-metaball.Radius, metaball.Radius), 0), color, Vector2.UnitY), // bl
        //];

        MetaballShader.Value.Parameters["worldViewProjection"].SetValue(Matrix.CreateTranslation(
            new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * // world
            Main.GameViewMatrix.TransformationMatrix *
            Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1) // projection
            );
        MetaballShader.Value.Parameters["color"].SetValue(color.ToVector4());
        MetaballShader.Value.Parameters["radius"].SetValue(metaball.Radius);
        MetaballShader.Value.Parameters["scale"].SetValue(dust.scale * scale);
        MetaballShader.Value.CurrentTechnique.Passes[0].Apply();

        Main.spriteBatch.Begin();
        Main.spriteBatch.Draw(MetaballTexture.Value, new Rectangle((int)position.X, (int)position.Y, (int)(metaball.Radius * 2 * dust.scale * scaleFactor.X), (int)(metaball.Radius * 2 * dust.scale * scaleFactor.Y)), null, color, dust.rotation, new Vector2(metaball.Radius) * 0.5f, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        //GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, [0, 1, 2, 2, 3, 0], 0, 2);
    }
}
