using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownUI : UIState
{
    private static ShowdownUI? instance;
    internal static ShowdownUI Instance => instance ??= new();

    private static float Scale => MathHelper.Max(Main.screenWidth / 1024f, Main.screenHeight / 512f);
    private Vector2 defaultScrollingBackgroundPositionViaScale;

    private UIPanel? ContentContainer { get; set; }
    private UIImage? ScrollingBackground { get; set; }
    private UIImage? Background { get; set; }
    private UIImage? Foreground { get; set; }

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

        defaultScrollingBackgroundPositionViaScale = new((1552 - 1552 * Scale) * -0.5f, (512 - 512 * Scale) * -0.5f);
        Vector2 defaultBackgroundPositionViaScale = new((1024 - 1024 * Scale) * -0.5f, (512 - 512 * Scale) * -0.5f);
        StyleDimension newWidth = StyleDimension.FromPixels(defaultBackgroundPositionViaScale.X);
        StyleDimension newHeight = StyleDimension.FromPixels(defaultBackgroundPositionViaScale.Y);

        Background?.Left = newWidth;
        Background?.Top = newHeight;
        Background?.ImageScale = Scale;
        ScrollingBackground?.Left = StyleDimension.FromPixels(defaultScrollingBackgroundPositionViaScale.X);
        ScrollingBackground?.Top = StyleDimension.FromPixels(defaultScrollingBackgroundPositionViaScale.Y);
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

        if (ScrollingBackground != null)
        {
            ScrollingBackground.Left.Pixels = ScrollingBackground.Left.Pixels <= defaultScrollingBackgroundPositionViaScale.X - 528 * Scale ? defaultScrollingBackgroundPositionViaScale.X : ScrollingBackground.Left.Pixels - 0.25f;
            ScrollingBackground.Recalculate();
        }
    }
}
