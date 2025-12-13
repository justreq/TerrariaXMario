using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.BroInfoUI;

[Autoload(Side = ModSide.Client)]
internal class BroInfoUISystem : ModSystem
{
    private UserInterface? BroInfoUserInterface;
    private BroInfoUI? BroInfoUI;

    public override void Load()
    {
        BroInfoUI = new();
        BroInfoUserInterface = new();
        BroInfoUserInterface.SetState(BroInfoUI);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        BroInfoUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: Bro Info UI",
            () =>
            {
                BroInfoUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}
