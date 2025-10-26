using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.KeybindSystem;
[Autoload(Side = ModSide.Client)]
internal class KeybindIndicatorSystem : ModSystem
{
    private UserInterface? UIKeybindIndicatorUserInterface;
    private KeybindIndicator? UIKeybindIndicator;

    public override void Load()
    {
        UIKeybindIndicator = new();
        UIKeybindIndicatorUserInterface = new();
        UIKeybindIndicatorUserInterface.SetState(UIKeybindIndicator);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        UIKeybindIndicatorUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: UI Keybind Indicator",
            () =>
            {
                UIKeybindIndicatorUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.Game)
        );
    }
}
