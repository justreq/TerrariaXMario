using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace TerrariaXMario.Common.ShowdownSystem;

internal class ShowdownSubworld : Subworld
{
    internal int? currentPlayer;

    public override LocalizedText DisplayName => Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.Showdown.DisplayName");
    public override int Width => 256;
    public override int Height => 256;
    public override bool ShouldSave => false;
    public override bool NoPlayerSaving => true;
    public override List<GenPass> Tasks => [new ShowdownSubworldPass()];

    public override bool ChangeAudio()
    {
        // switch to showdown music
        return true;
    }

    public override void DrawMenu(GameTime gameTime)
    {
        // draw loading menu
    }

    public override void Update()
    {
        // basic update tasks (freeze time to midday, stop enemy spawns, etc)
    }
}

internal class ShowdownSubworldPass : GenPass
{
    public ShowdownSubworldPass() : base("Showdown", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "";
        Main.worldSurface = Main.maxTilesY;
        Main.rockLayer = Main.maxTilesY;
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                //progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY));
                //Tile tile = Main.tile[i, j];
                //tile.HasTile = true;
                //tile.TileType = TileID.Dirt;
            }
        }
    }
}