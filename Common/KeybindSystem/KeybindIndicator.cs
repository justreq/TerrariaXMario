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

    private int timeLeft = 300;

    public override void OnInitialize()
    {
        Indicator = this.AddElement(new UIKeypressIndicator());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<KeybindPlayer>()?.keyToShowInIndicator != null) base.Draw(spriteBatch);
    }

    private void UpdatePosition()
    {
        Player player = Main.LocalPlayer;
        Indicator?.Left = StyleDimension.FromPixels(player.Top.X - Main.screenPosition.X - Indicator.GetDimensions().Width * 0.5f);
        Indicator?.Top = StyleDimension.FromPixels(player.Top.Y - Main.screenPosition.Y - 128);
        Indicator?.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        KeybindPlayer? modPlayer = player.GetModPlayerOrNull<KeybindPlayer>();

        UpdatePosition();

        if (modPlayer?.keyToShowInIndicator == null)
        {
            Indicator?.frame = 0;
            timeLeft = 300;
            return;
        }
        else if (!Indicator?.canDraw ?? false)
        {
            Indicator?.SetKey(modPlayer?.keyToShowInIndicator!);
            UpdatePosition();
            Indicator?.canDraw = true;
        }

        timeLeft--;

        if (timeLeft <= 0)
        {
            player.GetModPlayerOrNull<ShowdownPlayer>()?.EndShowdownQuery();
            Indicator?.SetKey("");
            Indicator?.canDraw = false;
            return;
        }
    }
}
