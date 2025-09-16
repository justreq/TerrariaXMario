using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
[Autoload(Side = ModSide.Client)]
internal class ObjectSpawnerBlockUISystem : ModSystem
{
    private UserInterface? ObjectSpawnerBlockUserInterface;
    private ObjectSpawnerBlockUI? ObjectSpawnerBlockUI;

    public override void Load()
    {
        ObjectSpawnerBlockUI = new();
        ObjectSpawnerBlockUserInterface = new();
        ObjectSpawnerBlockUserInterface.SetState(ObjectSpawnerBlockUI);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        ObjectSpawnerBlockUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            $"{nameof(TerrariaXMario)}: Object Spawner Block UI",
            () =>
            {
                ObjectSpawnerBlockUserInterface?.Draw(Main.spriteBatch, new());
                return true;
            },
            InterfaceScaleType.UI)
        );
    }
}
