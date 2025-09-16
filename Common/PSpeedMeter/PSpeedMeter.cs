﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.PSpeedMeter;
internal class PSpeedMeter : UIState
{
    private UIElement? Container { get; set; }
    private UIImageFramed? ImageFrame { get; set; }
    private UIImageFramed? ImageResource { get; set; }

    private Asset<Texture2D>? Texture;

    private int resourceWidth;
    private bool frameFlag;
    private int frameRate;

    public override void OnInitialize()
    {
        Texture = ModContent.Request<Texture2D>($"{GetType().FullName!.Replace(".", "/")}");

        Container = new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(56);
            e.Height = StyleDimension.FromPixels(16);
        });
        Append(Container);

        ImageFrame = new(Texture, new(0, 0, 56, 16));
        Container.Append(ImageFrame);

        ImageResource = new UIImageFramed(Texture, new(0, 54, 0, 6)).With(e =>
        {
            e.Left = StyleDimension.FromPixels(2);
            e.Top = StyleDimension.FromPixels(8);
        });
        Container.Append(ImageResource);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<CapPlayer>()?.CanDoCapEffects ?? false) base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        PSpeedPlayer? pSpeedPlayer = player.GetModPlayerOrNull<PSpeedPlayer>();

        if (pSpeedPlayer == null || (!pSpeedPlayer.CapPlayer?.CanDoCapEffects ?? true)) return;

        if (Container != null)
        {
            Container.Left = StyleDimension.FromPixels(player.Bottom.X - Main.screenPosition.X - 28);
            Container.Top = StyleDimension.FromPixels(player.Bottom.Y - Main.screenPosition.Y);
            Container.Recalculate();
        }

        resourceWidth = pSpeedPlayer.runTime == 0 ? resourceWidth - 1 : (int)((float)pSpeedPlayer.runTime / pSpeedPlayer.runTimeRequiredForPSpeed * 36);
        ImageResource?.SetFrame(new(0, 54, resourceWidth, 6));

        if (ImageFrame != null) ImageFrame.Color = Color.Lerp(ImageFrame.Color, resourceWidth <= 18 ? Color.Transparent : Color.White, 0.075f);
        if (ImageResource != null) ImageResource.Color = ImageFrame?.Color ?? Color.Transparent;

        if (pSpeedPlayer.hasPSpeed)
        {
            ImageFrame?.SetFrame(new(0, 18 * (frameFlag.ToInt() + 1), 56, 16));
            frameRate++;

            if (frameRate >= 10)
            {
                frameFlag ^= true;
                frameRate = 0;
            }
        }
        else
        {
            frameFlag = false;
            frameRate = 0;
            ImageFrame?.SetFrame(new(0, 0, 56, 16));
        }
    }
}
