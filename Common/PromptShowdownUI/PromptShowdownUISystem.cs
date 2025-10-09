using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.PromptShowdownUI;
[Autoload(Side = ModSide.Client)]
internal class PromptShowdownUISystem : ModSystem
{
    private UserInterface? PromptShowdownUserInterface;
    private PromptShowdownUI? PromptShowdownUI;

    public override void Load()
    {
        PromptShowdownUI = new();
        PromptShowdownUserInterface = new();
        PromptShowdownUserInterface.SetState(PromptShowdownUI);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        PromptShowdownUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: Prompt Showdown UI",
            () =>
            {
                PromptShowdownUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}
