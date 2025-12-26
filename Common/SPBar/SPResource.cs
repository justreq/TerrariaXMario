using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.SPBar;

internal class SPResource : UIElement
{
    private readonly string texture;
    private int minSP;
    private int maxSP;

    private float Percent => Math.Clamp(((float)(Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.StatSP ?? 0) - minSP) / (maxSP - minSP), 0f, 1f);

    internal SPResource(string texture, int minSP, int maxSP)
    {
        this.texture = texture;
        this.minSP = minSP;
        this.maxSP = maxSP;

        Width = StyleDimension.FromPixels(texture == "MiddleModern" ? 24 : 40);

        Height = StyleDimension.FromPixels(texture switch
        {
            "Single" => 38,
            "Top" => 34,
            "Middle" => 32,
            "Bottom" => 36,
            "MiddleModern" => 12,
            _ => 38
        });
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        string path = GetType().Namespace!.Replace(".", "/");
        CalculatedStyle dimensions = GetDimensions();
        Rectangle rectangle = dimensions.ToRectangle();
        Vector2 position = dimensions.Position();

        if (!texture.Contains("Modern") && !TerrariaXMario.ResourceBarStyle.Contains("Bars"))
        {
            if (TerrariaXMario.ResourceBarStyle.Contains("Fancy")) spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SP{texture}", AssetRequestMode.ImmediateLoad).Value, rectangle, Color.White);

            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPFill", AssetRequestMode.ImmediateLoad).Value, position + new Vector2(20, 15 + texture switch
            {
                "Single" => 4,
                "Top" => 4,
                "Middle" => 2,
                "Bottom" => 2,
                _ => 4
            }), null, Color.White * (TerrariaXMario.ResourceBarStyle == "Classic" ? Percent * 0.5f + 0.5f : 1), 0, new Vector2(16, 15), TerrariaXMario.ResourceBarStyle == "Classic" ? Percent * 0.5f + 0.5f : Percent, SpriteEffects.None, 0);

            return;
        }

        if (texture.Contains("Modern") && TerrariaXMario.ResourceBarStyle.Contains("Bars"))
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SP{texture}", AssetRequestMode.ImmediateLoad).Value, GetDimensions().ToRectangle(), Color.White);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/SPFillModern", AssetRequestMode.ImmediateLoad).Value, position + new Vector2(12, 0), new Rectangle(0, 0, 12, (int)(Percent * 12)), Color.White, 0, new Vector2(6, 0), 1, SpriteEffects.None, 0);
        }
    }

    internal void SetRange(int min, int max)
    {
        minSP = min;
        maxSP = max;
    }
}
