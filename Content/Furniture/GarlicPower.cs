using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TerrariaXMario.Content.Furniture;

internal class GarlicPowerTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Width = 6;
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
        TileObjectData.addTile(Type);

        AddMapEntry(Color.HotPink);
    }
}

internal class GarlicPower : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<GarlicPowerTile>());

        Item.width = 48;
        Item.height = 40;
    }
}