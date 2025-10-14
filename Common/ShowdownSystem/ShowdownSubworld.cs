using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace TerrariaXMario.Common.ShowdownSystem;

internal class ShowdownSubworld : Subworld
{
    public override int Width => 100;
    public override int Height => 100;

    public override bool ShouldSave => false;
    public override bool NoPlayerSaving => true;

    public override List<GenPass> Tasks => [new ShowdownSubworldPass()];
}

internal class ShowdownSubworldPass : GenPass
{
    public ShowdownSubworldPass() : base("Showdown", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Entering Showdown";
        Main.worldSurface = Main.maxTilesY;
        Main.rockLayer = Main.maxTilesY;
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY));
                Tile tile = Main.tile[i, j];
                tile.HasTile = true;
                tile.TileType = TileID.Dirt;
            }
        }
    }
}