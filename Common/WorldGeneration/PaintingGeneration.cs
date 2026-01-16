using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using TerrariaXMario.Content.Furniture;
using TerrariaXMario.Core;

namespace TerrariaXMario.Common.WorldGeneration;

internal class PaintingGeneration : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_WorldGen.RandPictureTile += On_WorldGen_RandPictureTile;
    }

    private PaintingEntry On_WorldGen_RandPictureTile(On_WorldGen.orig_RandPictureTile orig)
    {
        PaintingEntry obj = orig();

        if (Main.rand.NextBool(8))
        {
            obj.tileType = WorldGen.genRand.NextFromList(ModContent.TileType<FamilyPhotoTile>(), ModContent.TileType<GarlicPowerTile>(), ModContent.TileType<MadpaxTile>(), ModContent.TileType<MischievousActivitiesTile>(), ModContent.TileType<TalluigiTile>());
            obj.style = 0;
        }

        return obj;
    }
}
