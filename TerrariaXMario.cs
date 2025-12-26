using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Core;

namespace TerrariaXMario;

internal class TerrariaXMario : Mod
{
    internal static TerrariaXMario Instance => ModContent.GetInstance<TerrariaXMario>();

    internal static string Textures => $"{nameof(TerrariaXMario)}/Assets/Textures";
    internal static string Sounds => $"{nameof(TerrariaXMario)}/Assets/Sounds";
    internal static string BrickBlockTile => $"{nameof(TerrariaXMario)}/Content/Blocks/BrickBlockTile";
    internal static string QuestionBlockTile => $"{nameof(TerrariaXMario)}/Content/Blocks/QuestionBlockTile";

    private Asset<Texture2D>? oldMushroomTexture;
    private Asset<Texture2D>[]? oldCursors;
    internal int CursorGrabIndex = -1;
    internal int CursorThrowIndex = -1;
    internal int CursorEditIndex = -1;

    internal ISpawnableObject[]? spawnableObjects;

    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(int i, int j) => TileEntity.TryGet(new(i, j), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Vector2 coords) => TileEntity.TryGet(new((int)coords.X, (int)coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Point coords) => TileEntity.TryGet(new(coords.X, coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;

    internal static Dictionary<string, Color> capColors = new() {
        { "Mario", new Color(217, 22, 22) },
        { "Luigi", new Color(27, 149, 4) }
    };

    internal static Vector2 BroInfoPageButtonPosition;

    internal static string ResourceBarStyle => Main.ResourceSetsManager.ActiveSet.DisplayedName;

    public override void Load()
    {
        oldMushroomTexture = TextureAssets.Item[ItemID.Mushroom];
        TextureAssets.Item[ItemID.Mushroom] = ModContent.Request<Texture2D>($"{nameof(TerrariaXMario)}/Content/Consumables/EdibleMushroom1");

        oldCursors = [.. TextureAssets.Cursors];
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorGrab")];
        CursorGrabIndex = TextureAssets.Cursors.Length - 1;
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorThrow")];
        CursorThrowIndex = TextureAssets.Cursors.Length - 1;
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorEdit")];
        CursorEditIndex = TextureAssets.Cursors.Length - 1;

        spawnableObjects = [.. ModContent.GetContent<ISpawnableObject>().Where(e => e is not DefaultSpawnableObject).OrderBy(x => x.GetType().BaseType?.Name)];
    }

    public override void Unload()
    {
        TextureAssets.Item[ItemID.Mushroom] = oldMushroomTexture;
        oldMushroomTexture = null;
        TextureAssets.Cursors = oldCursors;
        oldCursors = null;
        CursorGrabIndex = -1;
        CursorThrowIndex = -1;
        CursorEditIndex = -1;
        spawnableObjects = null;
    }

    internal static bool SolidTile(int i, int j)
    {
        return WorldGen.InWorld(i, j) && SolidTile(Main.tile[i, j]);
    }

    internal static bool SolidTile(Tile t)
    {
        if (!t.HasTile || t.IsActuated)
        {
            return false;
        }

        return Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType];
    }

    internal static bool SolidOrSolidTopTile(int i, int j)
    {
        return WorldGen.InWorld(i, j) && SolidOrSolidTopTile(Main.tile[i, j]);
    }

    internal static bool SolidOrSolidTopTile(Tile t)
    {
        if (!t.HasTile || t.IsActuated)
        {
            return false;
        }

        return Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType];
    }

    internal static Vector2 CenterOfMultitileInWorld(Point point)
    {
        Tile tile = Framing.GetTileSafely(point.X, point.Y);
        TileObjectData data = TileObjectData.GetTileData(tile);
        return TileObjectData.TopLeft(point.X, point.Y).ToWorldCoordinates() + new Vector2(data.Width, data.Height) * 4;
    }
}