using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.PowerupHUD;

[Autoload(Side = ModSide.Client)]
internal class PowerupHUDSystem : ModSystem
{
    private UserInterface? PowerupHUDUserInterface;
    private PowerupHUD? PowerupHUD;

    public override void Load()
    {
        PowerupHUD = new();
        PowerupHUDUserInterface = new();
        PowerupHUDUserInterface.SetState(PowerupHUD);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        PowerupHUDUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: Powerup HUD",
            () =>
            {
                PowerupHUDUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}
