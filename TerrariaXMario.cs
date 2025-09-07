using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaXMario;
internal class TerrariaXMario : Mod
{
    public static TerrariaXMario Instance => ModContent.GetInstance<TerrariaXMario>();

    internal static string Textures => $"{nameof(TerrariaXMario)}/Assets/Textures";
    internal static string Sounds => $"{nameof(TerrariaXMario)}/Assets/Sounds";

    private Asset<Texture2D>[]? oldCursors;
    public int CursorGrabIndex = -1;
    public int CursorThrowIndex = -1;

    public override void Load()
    {
        oldCursors = [.. TextureAssets.Cursors];
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorGrab")];
        CursorGrabIndex = TextureAssets.Cursors.Length - 1;
        TextureAssets.Cursors = [.. TextureAssets.Cursors, ModContent.Request<Texture2D>($"{Textures}/CursorThrow")];
        CursorThrowIndex = TextureAssets.Cursors.Length - 1;
    }

    public override void Unload()
    {
        TextureAssets.Cursors = oldCursors;
        oldCursors = null;
        CursorGrabIndex = -1;
        CursorThrowIndex = -1;
    }
}