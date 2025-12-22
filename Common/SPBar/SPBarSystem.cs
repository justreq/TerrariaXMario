using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.SPBar;

[Autoload(Side = ModSide.Client)]
internal class SPBarSystem : ModSystem
{
    private UserInterface? SPBarUserInterface;
    private SPBar? SPBar;

    public override void Load()
    {
        SPBar = new();
        SPBarUserInterface = new();
        SPBarUserInterface.SetState(SPBar);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        SPBarUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: SP Bar",
            () =>
            {
                SPBarUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}
