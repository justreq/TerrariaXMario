using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Tools;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Blocks;
internal class ObjectSpawnerBlockEntity : ModTileEntity
{
    internal int strikeAnimationTimeleft;
    internal bool justStruck;
    internal bool wasPreviouslyStruck;
    internal ISpawnableObject[] spawnContents = [];

    internal string? tileInternalName;
    internal bool ShouldShowEmpty => wasPreviouslyStruck && spawnContents.Length == 0;

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

    public override void OnKill()
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        for (int a = 0; a < 4; a++)
        {
            if (Mod.TryFind($"{(ShouldShowEmpty ? "EmptyBlockTile" : $"{tileInternalName}")}Gore_{a + 1}", out ModGore goreInstance))
            {
                int gore = goreInstance.Type;
                gore = Gore.NewGore(null, Position.ToWorldCoordinates() + new Vector2(8, 8), MathHelper.ToRadians(0 - a * 60).ToRotationVector2() + new Vector2(0, -2), gore);
                Main.gore[gore].timeLeft = 0;
            }
        }

        if (modPlayer?.currentObjectSpawnerBlockToEdit == Position.ToVector2()) modPlayer?.currentObjectSpawnerBlockToEdit = Vector2.Zero;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(wasPreviouslyStruck)] = wasPreviouslyStruck;
        if (spawnContents.Length > 0) tag[nameof(spawnContents)] = spawnContents.ToList();
        tag[nameof(tileInternalName)] = tileInternalName;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(wasPreviouslyStruck))) wasPreviouslyStruck = tag.GetBool(nameof(wasPreviouslyStruck));
        if (tag.ContainsKey(nameof(spawnContents))) spawnContents = [.. tag.GetList<ISpawnableObject>(nameof(spawnContents))];
        if (tag.ContainsKey(nameof(tileInternalName))) tileInternalName = tag.GetString(nameof(tileInternalName));
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

    public override void PlaceInWorld(int i, int j, Item item)
    {
        TerrariaXMario.GetTileEntityOrNull(i, j)?.tileInternalName = GetType().Name;
    }

    public override bool CanKillTile(int i, int j, ref bool blockDamaged) => TerrariaXMario.GetTileEntityOrNull(i, j)?.spawnContents.Length == 0;

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
        if (tile.TileFrameX != 0 || tile.TileFrameY != 0) return;

        for (int a = 0; a < (Main.FrameSkipMode == FrameSkipMode.Off ? 1 : 4); a++)
        {
            Dust dust = Dust.NewDustPerfect(new Vector2(i, j).ToWorldCoordinates() + new Vector2(8f) + Main.rand.NextVector2Circular(24, 24), DustID.SeaSnail, Vector2.Zero, newColor: Color.White);
            dust.noGravity = true;
        }
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