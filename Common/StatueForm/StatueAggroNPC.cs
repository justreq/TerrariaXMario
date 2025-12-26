using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.StatueForm;

internal class StatueAggroNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        base.PostAI(npc);

        if (npc.HasValidTarget && (Main.player[npc.target].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false))
        {
            Main.player[npc.target].aggro = int.MinValue;
        }
    }
}
