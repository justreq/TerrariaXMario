using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class PlayerExtensions
{
    public static T? GetModPlayerOrNull<T>(this Player player) where T : ModPlayer => player.TryGetModPlayer(out T result) ? result : null;

    public static bool IsOnGroundPrecise(this Player player) // I didn't make this, but it works, so I will not touch it
    {
        for (int i = 0; i < 3; i++)
        {
            var tileX = Main.tile[(int)((player.position.X + (player.width * i / 2f)) / 16f), (int)((player.position.Y + (player.gravDir == 1 ? player.height + 1 : -1)) / 16f)];

            if (tileX.HasTile && (Main.tileSolid[tileX.TileType] || Main.tileSolidTop[tileX.TileType]) && player.velocity.Y == 0f)
            {
                return true;
            }
        }
        return false;
    }
}