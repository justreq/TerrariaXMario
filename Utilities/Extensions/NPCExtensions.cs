using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;
internal static class NPCExtensions
{
    public static T? GetGlobalNPCOrNull<T>(this NPC npc) where T : GlobalNPC => npc.TryGetGlobalNPC(out T result) ? result : null;
}