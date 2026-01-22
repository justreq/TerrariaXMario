using StructureHelper.API;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TerrariaXMario.Content.Blocks;

namespace TerrariaXMario.Common.WorldGeneration;

internal class QuestionBlockGeneration : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        tasks.Add(new QuestionBlockPass("Sprinkling Question Blocks", 100f));
    }
}
internal class QuestionBlockPass(string name, float loadWeight) : GenPass(name, loadWeight)
{
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = Name;

        int maxCount = 100;
        if (Main.maxTilesX > 6000) maxCount += 100;
        if (Main.maxTilesX > 8000) maxCount += 200;

        int placed = 0;
        int attempts = 0;

        while (placed < maxCount && attempts < 5000)
        {
            attempts++;

            int x = WorldGen.genRand.Next(32, Main.maxTilesX - 32);
            int y = WorldGen.genRand.Next(32, Main.maxTilesY - 32);

            while (y < Main.maxTilesY - 32 && !WorldGen.SolidTile(x, y)) y++;

            if (y >= Main.maxTilesY - 32 || Framing.GetTileSafely(x, y).TileType == ModContent.TileType<QuestionBlockTile>()) continue;

            x -= 2;
            y -= 8;

            if (!AreaIsClear(x, y, 2, 8)) continue;

            Generator.GenerateStructure($"Assets/Structures/QuestionBlock", new Point16(x, y), TerrariaXMario.Instance);
            placed++;
            attempts = 0;
        }
    }

    private static bool AreaIsClear(int startX, int startY, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile tile = Framing.GetTileSafely(startX + i, startY + j);
                if (TerrariaXMario.SolidTile(tile) || tile.TileType == TileID.Trees) return false;
            }
        }
        return true;
    }
}