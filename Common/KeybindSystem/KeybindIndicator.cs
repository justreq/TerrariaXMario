using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.KeybindSystem;
internal class KeybindIndicator : UIState
{
    private UIKeypressIndicator? Indicator { get; set; }

    public override void OnInitialize()
    {
        Indicator = this.AddElement(new UIKeypressIndicator());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        KeybindPlayer? keybindPlayer = Main.LocalPlayer.GetModPlayerOrNull<KeybindPlayer>();

        if (keybindPlayer?.keyToShowInIndicator != null)
        {
            Indicator?.SetKey(keybindPlayer?.keyToShowInIndicator!);
            base.Draw(spriteBatch);
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;

        Indicator?.Left = StyleDimension.FromPixels(player.Top.X - Main.screenPosition.X - Indicator.GetDimensions().Width * 0.5f);
        Indicator?.Top = StyleDimension.FromPixels(player.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false ? Main.screenHeight / Main.GameZoomTarget + 160 : player.Top.Y - Main.screenPosition.Y - 128);
        Indicator?.Recalculate();
    }
}
