using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.PSpeedMeter;
[Autoload(Side = ModSide.Client)]
internal class PSpeedMeterSystem : ModSystem
{
    private UserInterface? PSpeedMeterUserInterface;
    private PSpeedMeter? PSpeedMeter;

    public override void Load()
    {
        PSpeedMeter = new();
        PSpeedMeterUserInterface = new();
        PSpeedMeterUserInterface.SetState(PSpeedMeter);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        PSpeedMeterUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: PSpeed Meter",
            () =>
            {
                PSpeedMeterUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.Game)
        );
    }
}
