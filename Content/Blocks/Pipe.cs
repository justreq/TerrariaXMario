using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TerrariaXMario.Content.Blocks;

internal class PipeEntity : ModTileEntity
{
    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Framing.GetTileSafely(x, y);
        return tile.HasTile && TileLoader.GetTile(tile.TileType) is PipeTile;
    }

    public override void OnKill()
    {
        for (int a = 0; a < 4; a++)
        {
            if (Mod.TryFind($"PipeTileGore_{a + 1}", out ModGore goreInstance))
            {
                int gore = goreInstance.Type;
                gore = Gore.NewGore(null, Position.ToWorldCoordinates() + new Vector2(8, 8), MathHelper.ToRadians(0 - a * 60).ToRotationVector2() + new Vector2(0, -2), gore);
                Main.gore[gore].timeLeft = 0;
            }
        }
    }
}

internal class PipeTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileNoAttach[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<PipeEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Height, 0);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.addAlternate(1);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.addAlternate(2);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Height, 0);
        TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
        TileObjectData.addAlternate(3);
        TileObjectData.addTile(Type);
        TileID.Sets.DrawsWalls[Type] = true;
        AddMapEntry(Color.HotPink);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<PipeEntity>().Kill(i, j);
    }

    public override bool CreateDust(int i, int j, ref int type) => false;
}

internal class Pipe : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PipeTile>());
    }
}