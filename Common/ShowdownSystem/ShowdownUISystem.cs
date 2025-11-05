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

    private readonly string[] enabledLayers = ["Vanilla: Cursor", "Vanilla: Mouse Over", "Vanilla: Resource Bars", "Vanilla: Entity Health Bars", "Vanilla: Achievement Complete Popups", "Vanilla: Fancy UI", "Vanilla: Ingame Options", "Vanilla: MP Player Names", "Vanilla: Mouse Text", "Vanilla: Interface Logic 1", "Vanilla: Interface Logic 2", "Vanilla: Interface Logic 3", "Vanilla: Interface Logic 4", "Vanilla: Player Chat"];

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

        foreach (GameInterfaceLayer layer in layers)
        {
            if (layer.Name.Contains("Vanilla") && !enabledLayers.Contains(layer.Name)) layer.Active = false;
        }
    }
}
