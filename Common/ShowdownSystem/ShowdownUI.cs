using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownUI : UIState
{
    private UIKeypressIndicator? CancelIndicator { get; set; }
    private UIKeypressIndicator? ConfirmIndicator { get; set; }

    public override void OnInitialize()
    {
        CancelIndicator = this.AddElement(new UIKeypressIndicator().With(e =>
        {
            e.Left = StyleDimension.FromPixels(16);
            e.Top = StyleDimension.FromPixels(16);
        }));

        ConfirmIndicator = this.AddElement(new UIKeypressIndicator());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false) base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        CancelIndicator?.SetKey($"{KeybindSystem.KeybindSystem.GetVanillaKeybindKey(TriggerNames.Inventory)} ({Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.Showdown.UI.Cancel")})");
        ConfirmIndicator?.SetKey($"{KeybindSystem.KeybindSystem.GetVanillaKeybindKey(TriggerNames.Jump)}");

        if (!modPlayer?.isPlayerInShowdownSubworld ?? true) return;

        CancelIndicator?.canDraw = modPlayer?.queriedAction != ShowdownAction.None;
        ConfirmIndicator?.canDraw = false;
    }
}
