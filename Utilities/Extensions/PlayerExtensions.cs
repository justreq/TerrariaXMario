using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class PlayerExtensions
{
    public static T? GetModPlayerOrNull<T>(this Player player) where T : ModPlayer => player.TryGetModPlayer(out T result) ? result : null;

    // thanks direwolf

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
            if (onlySolid && TerrariaXMario.SolidTile(point.X, point.Y) || TerrariaXMario.SolidOrSolidTopTile(point.X, point.Y))
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
        return entity.velocity.Y == 0 && IsOnGroundPrecise(entity.BottomLeft.X, entity.BottomLeft.Y + yOffset, entity.width, onlySolid);
    }
}