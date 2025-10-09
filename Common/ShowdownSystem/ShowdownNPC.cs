using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownNPC : GlobalNPC
{
    internal bool inShowdown;

    public override bool InstancePerEntity => true;

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (inShowdown) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (inShowdown) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (inShowdown) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }
}
