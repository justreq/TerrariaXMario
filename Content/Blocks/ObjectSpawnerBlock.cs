using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.ObjectSpawnerBlockUI;
using TerrariaXMario.Content.Tools;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Blocks;
internal class ObjectSpawnerBlockEntity : ModTileEntity
{
    internal (SpawnableObjectGroup group, int type)[] spawnContents = [(SpawnableObjectGroup.Item, ItemID.GoldCoin)];

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Framing.GetTileSafely(x, y);
        return tile.HasTile && TileLoader.GetTile(tile.TileType) is ObjectSpawnerBlockTile;
    }
}

internal class ObjectSpawnerBlockTile : ModTile
{
    internal virtual Color? MapColor => null;

    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(int i, int j) => TileEntity.TryGet(new(i, j), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Vector2 coords) => TileEntity.TryGet(new((int)coords.X, (int)coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Point coords) => TileEntity.TryGet(new(coords.X, coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = false;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 16];
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<ObjectSpawnerBlockEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);
        if (MapColor != null) AddMapEntry((Color)MapColor);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<ObjectSpawnerBlockEntity>().Kill(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (!modPlayer?.CapPlayer?.CanDoCapEffects ?? true) return;

        if ((player.HeldItem.type == ModContent.ItemType<Hammer>() || Main.mouseItem.type == ModContent.ItemType<Hammer>()) && modPlayer != null && GetTileEntityOrNull(modPlayer.currentObjectSpawnerBlockToEdit.ToPoint())?.Position != GetTileEntityOrNull(i, j)?.Position)
        {
            Main.cursorOverride = TerrariaXMario.Instance.CursorEditIndex;

            if (PlayerInput.Triggers.JustPressed.MouseLeft)
            {
                Tile tile = Framing.GetTileSafely(i, j);

                modPlayer.currentObjectSpawnerBlockToEdit = new(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
            }
        }
    }
}

internal abstract class ObjectSpawnerBlockItem<T> : ModItem where T : ObjectSpawnerBlockTile
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<T>());
    }
}