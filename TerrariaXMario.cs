using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using TerrariaXMario.Common.ObjectSpawnerBlockUI;
using TerrariaXMario.Content.Blocks;

namespace TerrariaXMario;
internal class TerrariaXMario : Mod
{
    public static TerrariaXMario Instance => ModContent.GetInstance<TerrariaXMario>();

    internal static string Textures => $"{nameof(TerrariaXMario)}/Assets/Textures";
    internal static string Sounds => $"{nameof(TerrariaXMario)}/Assets/Sounds";
    internal static string BrickBlockTile => $"{nameof(TerrariaXMario)}/Content/Blocks/BrickBlockTile";
    internal static string QuestionBlockTile => $"{nameof(TerrariaXMario)}/Content/Blocks/QuestionBlockTile";

    private Asset<Texture2D>[]? oldCursors;
    internal int CursorGrabIndex = -1;
    internal int CursorThrowIndex = -1;
    internal int CursorEditIndex = -1;

    internal ISpawnableObject[]? spawnableObjects;

    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(int i, int j) => TileEntity.TryGet(new(i, j), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Vector2 coords) => TileEntity.TryGet(new((int)coords.X, (int)coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;
    internal static ObjectSpawnerBlockEntity? GetTileEntityOrNull(Point coords) => TileEntity.TryGet(new(coords.X, coords.Y), out ModTileEntity entity) ? entity as ObjectSpawnerBlockEntity : null;

    public override void Load()
    {
        oldCursors = [.. TextureAssets.Cursors];
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorGrab")];
        CursorGrabIndex = TextureAssets.Cursors.Length - 1;
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorThrow")];
        CursorThrowIndex = TextureAssets.Cursors.Length - 1;
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorEdit")];
        CursorEditIndex = TextureAssets.Cursors.Length - 1;

        spawnableObjects = [.. ModContent.GetContent<ISpawnableObject>().OrderBy(x => x.GetType().Name)];

    }

    public override void Unload()
    {
        TextureAssets.Cursors = oldCursors;
        oldCursors = null;
        CursorGrabIndex = -1;
        CursorThrowIndex = -1;
        CursorEditIndex = -1;
        spawnableObjects = null;
    }
}