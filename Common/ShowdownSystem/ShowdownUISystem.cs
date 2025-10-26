using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
[Autoload(Side = ModSide.Client)]
internal class ShowdownUISystem : ModSystem
{
    private UserInterface? ShowdownUIUserInterface;
    private ShowdownUI? ShowdownUI;

    private readonly string[] disabledLayers = ["Vanilla: Map / Minimap", "Vanilla: Info Accessories Bar", "Vanilla: Hotbar"];

    public override void Load()
    {
        ShowdownUI = new();
        ShowdownUIUserInterface = new();
        ShowdownUIUserInterface.SetState(ShowdownUI);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        ShowdownUIUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: Showdown UI",
            () =>
            {
                ShowdownUIUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );

        if (!Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? true) return;

        foreach (string layer in disabledLayers)
        {
            int i = layers.FindIndex(e => e.Name.Equals(layer));

            if (i != -1) layers[i].Active = false;
        }
    }
}
