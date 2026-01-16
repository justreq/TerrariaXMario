using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Common.StatueForm;

namespace TerrariaXMario.Content.Blocks;

internal class ObjectSpawnerBlockEntity : ModTileEntity
{
    internal int strikeAnimationTimeleft;
    internal bool justStruck;
    internal bool WasPreviouslyStruck
    {
        get => field;
        set
        {
            field = value;
            if (value && spawnContents.Length == 0)
            {
                SpawnToadDust();
            }
        }
    }
    internal int ticksSinceEmptied;
    internal ISpawnableObject[] spawnContents = [];

    internal string? tileInternalName;
    internal bool ShouldShowEmpty => WasPreviouslyStruck && spawnContents.Length == 0;

    internal void SpawnToadDust()
    {
        for (int i = 0; i < 20; i++)
        {
            Dust.NewDust(Position.ToWorldCoordinates() - new Vector2(0, 56), 32, 32, ModContent.DustType<StatueDust>());
        }
    }

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

        if (ShouldShowEmpty)
        {
            if (ticksSinceEmptied >= Main.nightLength * 0.5f)
            {
                WasPreviouslyStruck = false;
                ticksSinceEmptied = 0;
                SpawnToadDust();
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Toad/ToadSuccess") { Volume = 0.4f }, Position.ToWorldCoordinates());
                return;
            }

            ticksSinceEmptied += (int)Main.dayRate;

            if ((int)(Main.GameUpdateCount * 0.25f) % 15 == 8) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Toad/ToadAmbient") { Volume = 0.1f }, Position.ToWorldCoordinates());
        }
    }

    public override void OnKill()
    {
        for (int a = 0; a < 4; a++)
        {
            if (Mod.TryFind($"{(ShouldShowEmpty ? "EmptyBlockTile" : $"{tileInternalName}")}Gore_{a + 1}", out ModGore goreInstance))
            {
                int gore = goreInstance.Type;
                gore = Gore.NewGore(null, Position.ToWorldCoordinates() + new Vector2(8, 8), MathHelper.ToRadians(0 - a * 60).ToRotationVector2() + new Vector2(0, -2), gore);
                Main.gore[gore].timeLeft = 0;
            }
        }

        if (ShouldShowEmpty)
        {
            SpawnToadDust();
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Toad/ToadKill{Main.rand.Next(1, 4)}") { Volume = 0.4f }, Position.ToWorldCoordinates());
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(WasPreviouslyStruck)] = WasPreviouslyStruck;
        if (spawnContents.Length > 0) tag[nameof(spawnContents)] = spawnContents.ToList();
        tag[nameof(tileInternalName)] = tileInternalName;
        tag[nameof(ticksSinceEmptied)] = ticksSinceEmptied;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(WasPreviouslyStruck))) WasPreviouslyStruck = tag.GetBool(nameof(WasPreviouslyStruck));
        if (tag.ContainsKey(nameof(spawnContents))) spawnContents = [.. tag.GetList<ISpawnableObject>(nameof(spawnContents))];
        if (tag.ContainsKey(nameof(tileInternalName))) tileInternalName = tag.GetString(nameof(tileInternalName));
        if (tag.ContainsKey(nameof(ticksSinceEmptied))) ticksSinceEmptied = tag.GetInt(nameof(ticksSinceEmptied));
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
        TileID.Sets.DrawsWalls[Type] = true;
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

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(i, j);

        if (entity == null || !entity.ShouldShowEmpty || i != entity.Position.X || j != entity.Position.Y) return;

        Vector2 worldPosition = (entity.Position.ToVector2() + new Vector2(11, 8)).ToWorldCoordinates() + new Vector2(-6, 6);

        spriteBatch.Draw(ModContent.Request<Texture2D>($"{TerrariaXMario.Textures}/Toad").Value, worldPosition - Main.screenPosition, new Rectangle(160, 56 * ((int)(Main.GameUpdateCount * 0.25f) % 15), 80, 56), Color.White);
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(i, j);
        if (entity == null) return;

        float offset = (float)(entity.strikeAnimationTimeleft > 0 ? -8 * ((float)entity.strikeAnimationTimeleft / 15) : 0);
        offsetY = (int)offset;
    }

    public override bool CreateDust(int i, int j, ref int type) => false;
}

internal abstract class ObjectSpawnerBlockItem<T> : ModItem where T : ObjectSpawnerBlockTile
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<T>());
    }
}