using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownUI : UIState
{
    private static ShowdownUI? instance;
    internal static ShowdownUI Instance => instance ??= new();

    private static float Scale => MathHelper.Max(Main.screenWidth / 1024f, Main.screenHeight / 512f);

    private UIPanel? ContentContainer { get; set; }
    private UIImage? ScrollingBackground { get; set; }
    private UIImage? Background { get; set; }
    private UIImage? Foreground { get; set; }

    private UICharacter? PlayerPuppet { get; set; }

    public override void OnInitialize()
    {
        Main.OnResolutionChanged += Main_OnResolutionChanged;
        ContentContainer = this.AddElement(new UIPanel().With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
            e.SetPadding(0);
        }));

        Background = ContentContainer.AddElement(new UIImage(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/ShowdownBackground")));

        ScrollingBackground = ContentContainer.AddElement(new UIImage(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/ShowdownScrollingBackground")));

        Foreground = ContentContainer.AddElement(new UIImage(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/ShowdownForeground")));

        Main_OnResolutionChanged(new(Main.screenWidth, Main.screenHeight));
    }

    private void Main_OnResolutionChanged(Vector2 obj)
    {
        if (ContentContainer == null) return;

        ContentContainer.Width = StyleDimension.FromPixels(1024 * Scale);
        ContentContainer.Height = StyleDimension.FromPixels(512 * Scale);
        ContentContainer.Recalculate();

        Vector2 positionOffsetByScale = new((1024 - 1024 * Scale) * -0.5f, (512 - 512 * Scale) * -0.5f);
        StyleDimension newWidth = StyleDimension.FromPixels(positionOffsetByScale.X);
        StyleDimension newHeight = StyleDimension.FromPixels(positionOffsetByScale.Y);

        Background?.Left = newWidth;
        Background?.Top = newHeight;
        Background?.ImageScale = Scale;
        ScrollingBackground?.Left = newWidth;
        ScrollingBackground?.Top = newHeight;
        ScrollingBackground?.ImageScale = Scale;
        Foreground?.Left = newWidth;
        Foreground?.Top = newHeight;
        Foreground?.ImageScale = Scale;
        ContentContainer.RecalculateChildren();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Player player = Main.LocalPlayer;
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        if (ContentContainer?.IsMouseHovering ?? false) player.mouseInterface = true;
        if (!modPlayer?.DoShowdownEffects ?? true) return;

        PlayerPuppet ??= ContentContainer?.AddElement(new UICharacter(player, true, false, useAClone: true));
    }
}
