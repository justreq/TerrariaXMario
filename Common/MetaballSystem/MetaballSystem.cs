using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.MetaballSystem;
internal class MetaballSystem : ModSystem
{
    private static MetaballRenderTarget metaballRenderTarget = null!;
    private static MetaballRenderTarget metaballOutlineRenderTarget = null!;

    public override void Load()
    {
        Main.QueueMainThreadAction(() =>
        {
            Main.ContentThatNeedsRenderTargets.Add(metaballRenderTarget = new(false));
            Main.ContentThatNeedsRenderTargets.Add(metaballOutlineRenderTarget = new(true));
        });
    }

    public override void PostDrawTiles()
    {
        metaballRenderTarget.Request();
        metaballOutlineRenderTarget.Request();

        if (!metaballRenderTarget.IsReady || !metaballOutlineRenderTarget.IsReady) return;

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(metaballOutlineRenderTarget.GetTarget(), metaballRenderTarget.screenPosition - Main.screenPosition, null, Color.White);
        Main.spriteBatch.Draw(metaballRenderTarget.GetTarget(), metaballRenderTarget.screenPosition - Main.screenPosition, null, Color.White);
        Main.spriteBatch.End();
    }

    internal class MetaballRenderTarget(bool outline) : ARenderTargetContentByRequest
    {
        internal Vector2 screenPosition;

        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);

            var oldTargets = device.GetRenderTargets();

            device.SetRenderTargets(_target);
            device.Clear(Color.Transparent);
            screenPosition = Main.screenPosition;
            spriteBatch.Begin();

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile.Type != ModContent.ProjectileType<MetaballProjectile>()) continue;

                MetaballProjectile metaball = (MetaballProjectile)projectile.ModProjectile;

                spriteBatch.Draw(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/Metaball").Value, projectile.position - Main.screenPosition, new Rectangle(outline ? 0 : 20, 0, 18, 18), outline ? metaball.outlineColor : metaball.fillColor);
            }

            foreach (Dust dust in Main.dust)
            {
                if (!dust.active || dust.type != ModContent.DustType<MetaballDust>()) continue;

                MetaballDust metaball = (MetaballDust)ModContent.GetModDust(dust.type);

                spriteBatch.Draw(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/Metaball").Value, dust.position - Main.screenPosition, new Rectangle(outline ? 0 : 20, 0, 18, 18), outline ? metaball.outlineColor : metaball.fillColor);
            }

            spriteBatch.End();
            device.SetRenderTargets(oldTargets);
            _wasPrepared = true;
        }
    }
}
