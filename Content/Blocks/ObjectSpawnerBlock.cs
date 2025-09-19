using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    internal int strikeAnimationTimeleft;
    internal bool justStruck;
    internal bool wasPreviouslyStruck;
    internal ISpawnableObject?[] spawnContents = [];

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Framing.GetTileSafely(x, y);
        return tile.HasTile && TileLoader.GetTile(tile.TileType) is ObjectSpawnerBlockTile;
    }

    public override void Update()
    {
        if (justStruck)
        {
            strikeAnimationTimeleft = 15;
            justStruck = false;
        }

        if (strikeAnimationTimeleft > 0) strikeAnimationTimeleft--;
    }
}

internal class ObjectSpawnerBlockTile : ModTile
{
    internal virtual Color MapColor => Color.HotPink;

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
        AddMapEntry(MapColor);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<ObjectSpawnerBlockEntity>().Kill(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || modPlayer.crouching || modPlayer.currentObjectSpawnerBlockToEdit != Vector2.Zero || (!modPlayer.CapPlayer?.CanDoCapEffects ?? true)) return;

        if ((player.HeldItem.type == ModContent.ItemType<Hammer>() || Main.mouseItem.type == ModContent.ItemType<Hammer>()) && modPlayer != null && TerrariaXMario.GetTileEntityOrNull(modPlayer.currentObjectSpawnerBlockToEdit.ToPoint())?.Position != TerrariaXMario.GetTileEntityOrNull(i, j)?.Position)
        {
            Main.cursorOverride = TerrariaXMario.Instance.CursorEditIndex;

            if (PlayerInput.Triggers.JustPressed.MouseLeft)
            {
                Tile tile = Framing.GetTileSafely(i, j);
                modPlayer.currentObjectSpawnerBlockToEdit = new(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
            }
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(i, j);

        if (entity == null || entity != TerrariaXMario.GetTileEntityOrNull(Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.currentObjectSpawnerBlockToEdit ?? Vector2.Zero)) return;

        Tile tile = Framing.GetTileSafely(i, j);

        Dust dust = Dust.NewDustPerfect(new Vector2(i, j).ToWorldCoordinates() + new Vector2(0.5f * (tile.TileFrameX / 18 == 0 ? 1 : -1), 0.5f * (tile.TileFrameY / 18 == 0 ? 1 : -1)) + Main.rand.NextVector2CircularEdge(16, 16), DustID.SeaSnail, Vector2.Zero, newColor: Color.White);
        dust.noGravity = true;
    }


    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(i, j);
        if (entity == null) return;

        float offset = (float)(entity.strikeAnimationTimeleft > 0 ? -8 * ((float)entity.strikeAnimationTimeleft / 15) : 0);
        offsetY = (int)offset;
    }
}

internal abstract class ObjectSpawnerBlockItem<T> : ModItem where T : ObjectSpawnerBlockTile
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<T>());
    }
}