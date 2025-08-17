using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class PlayerExtensions
{
    public static T? GetModPlayerOrNull<T>(this Player player) where T : ModPlayer => player.TryGetModPlayer(out T result) ? result : null;
}