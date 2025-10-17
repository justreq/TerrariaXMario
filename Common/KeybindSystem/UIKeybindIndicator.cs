using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.KeybindSystem;
internal class UIKeybindIndicator : UIState
{
    private UIImageFramed? Image { get; set; }
    private UIText? Text { get; set; }

    private int frame;
    private int frameRate;
    private int timeLeft = 300;

    public override void OnInitialize()
    {
        Image = this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/UIKeybindIndicator"), new Rectangle(0, 0, 70, 70)).With(e =>
        {
            e.Width = StyleDimension.FromPixels(70);
            e.Height = StyleDimension.FromPixels(70);
        }));

        Text = Image.AddElement(new UIText("").With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<KeybindPlayer>()?.keybindToShowInIndicator != null) base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        KeybindPlayer? modPlayer = player.GetModPlayerOrNull<KeybindPlayer>();

        Image?.Left = StyleDimension.FromPixels(player.Top.X - Main.screenPosition.X - 35);
        Image?.Top = StyleDimension.FromPixels(player.Top.Y - Main.screenPosition.Y - 128);
        Image?.Recalculate();

        if (modPlayer?.keybindToShowInIndicator == null)
        {
            frame = 0;
            timeLeft = 300;
            return;
        }

        timeLeft--;

        if (timeLeft <= 0)
        {
            player.GetModPlayerOrNull<ShowdownPlayer>()?.EndShowdownQuery();
            return;
        }

        if (modPlayer.keybindToShowInIndicator != Text?.Text) Text?.SetText(modPlayer.keybindToShowInIndicator);

        frameRate++;

        if (frameRate >= 4)
        {
            frame = (frame + 1) % 15;
            Image?.SetFrame(new(0, 70 * frame, 70, 70));
            frameRate = 0;
        }
    }
}
