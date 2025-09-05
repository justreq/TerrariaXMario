using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class PlayerExtensions
{
    public static T? GetModPlayerOrNull<T>(this Player player) where T : ModPlayer => player.TryGetModPlayer(out T result) ? result : null;

    // thank the lord for direworld420 vvv

    /// <summary>
    /// If any of the tiles right below this are standable, return true
    /// </summary>
    public static bool IsOnGroundPrecise(in float startX, float y, int width, bool onlySolid = false)
    {
        if (width <= 0)
        {
            throw new ArgumentException("width cannot be negative");
        }

        float fx = startX;

        //Needs atleast one iteration (in case width is 0)
        do
        {
            Point point = new Vector2(fx, y + 0.01f).ToTileCoordinates(); //0.01f is a magic number vanilla uses
            if (onlySolid && SolidTile(point.X, point.Y) || SolidOrSolidTopTile(point.X, point.Y))
            {
                return true;
            }
            fx += 16;
        }
        while (fx < startX + width);

        return false;
    }

    /// <inheritdoc cref="IsOnGroundPrecise(in float, float, int, bool)"/>
    public static bool IsOnGroundPrecise(this Entity entity, float yOffset = 0f, bool onlySolid = false)
    {
        return IsOnGroundPrecise(entity.BottomLeft.X, entity.BottomLeft.Y + yOffset, entity.width, onlySolid);
    }

    public static bool SolidTile(int i, int j)
    {
        return WorldGen.InWorld(i, j) && SolidTile(Main.tile[i, j]);
    }

    public static bool SolidTile(Tile t)
    {
        if (!t.HasTile || t.IsActuated)
        {
            return false;
        }

        return Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType];
    }

    public static bool SolidOrSolidTopTile(int i, int j)
    {
        return WorldGen.InWorld(i, j) && SolidOrSolidTopTile(Main.tile[i, j]);
    }

    public static bool SolidOrSolidTopTile(Tile t)
    {
        if (!t.HasTile || t.IsActuated)
        {
            return false;
        }

        return Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType];
    }
}