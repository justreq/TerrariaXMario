using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.KeybindIndicator;

internal class UIKeypressIndicator : UIElement
{
    private UIImageFramed? KeyboardButton { get; set; }
    private UIImageFramed? KeypressIndicatorLeft { get; set; }
    private UIImageFramed? KeypressIndicatorRight { get; set; }
    private UIText? Text { get; set; }

    internal int frame;
    private int frameRate;
    internal bool canDraw = true;
    internal bool Animated
    {
        get;
        set
        {
            field = value;

            if (KeypressIndicatorLeft == null || KeypressIndicatorRight == null) return;

            if (value)
            {
                if (!HasChild(KeypressIndicatorLeft)) this.AddElement(KeypressIndicatorLeft);
                if (!HasChild(KeypressIndicatorRight)) this.AddElement(KeypressIndicatorRight);
            }
            else
            {
                if (HasChild(KeypressIndicatorLeft)) KeypressIndicatorLeft.Remove();
                if (HasChild(KeypressIndicatorRight)) KeypressIndicatorRight.Remove();
            }
        }
    }

    internal UIKeypressIndicator(bool animated = false) : base()
    {
        Animated = animated;
        string textureFolder = GetType().Namespace!.Replace(".", "/");

        Width = StyleDimension.FromPixels(70);
        Height = StyleDimension.FromPixels(70);

        KeypressIndicatorLeft = new UIImageFramed(ModContent.Request<Texture2D>($"{textureFolder}/KeypressIndicator"), new Rectangle(0, 0, 34, 70)).With(e =>
        {
            e.VAlign = 0.5f;
            e.Width = StyleDimension.FromPixels(34);
            e.Height = StyleDimension.FromPixels(70);
        });

        KeypressIndicatorRight = new UIImageFramed(ModContent.Request<Texture2D>($"{textureFolder}/KeypressIndicator"), new Rectangle(36, 0, 34, 70)).With(e =>
        {
            e.HAlign = 1;
            e.VAlign = 0.5f;
            e.Width = StyleDimension.FromPixels(34);
            e.Height = StyleDimension.FromPixels(70);
        });

        KeyboardButton = this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{textureFolder}/KeyboardButton"), new Rectangle(0, 0, 22, 30)).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.Fill;
        }));

        Text = this.AddElement(new UIText("", 0.4f, true).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
        }));

        SetKey("");

        Animated = animated;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (canDraw) base.Draw(spriteBatch);

        if (!Animated)
        {
            frameRate = frame = 0;
            return;
        }

        frameRate++;

        if (frameRate >= 4)
        {
            frame = (frame + 1) % 15;
            KeypressIndicatorLeft?.SetFrame(new(0, 70 * frame + 2 * frame, 34, 70));
            KeypressIndicatorRight?.SetFrame(new(36, 70 * frame + 2 * frame, 34, 70));
            frameRate = 0;
        }
    }

    internal void SetKey(string key)
    {
        canDraw = false;
        Text?.SetText(key == "Space" ? "" : key);
        Text?.Recalculate();

        int width = key == "Space" ? 148 : (int)MathHelper.Max(22, Text?.GetDimensions().Width ?? 0 + 16);
        KeyboardButton?.SetFrame(new(0, key == "Space" ? 32 : 0, width, 30));
        KeyboardButton?.Recalculate();
        Width = StyleDimension.FromPixels(48 + width);
        Recalculate();
        RecalculateChildren();
        canDraw = true;
    }
}
