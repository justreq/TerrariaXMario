using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class ItemExtensions
{
    public static T? GetGlobalItemOrNull<T>(this Item item) where T : GlobalItem => item.TryGetGlobalItem(out T result) ? result : null;
}