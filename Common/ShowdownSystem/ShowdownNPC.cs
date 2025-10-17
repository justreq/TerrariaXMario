using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core.Effects;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownNPC : GlobalNPC
{
    internal NPCShowdownState showdownState;

    public override bool InstancePerEntity => true;

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (showdownState == NPCShowdownState.Active) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (showdownState == NPCShowdownState.Active) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (showdownState == NPCShowdownState.Active) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool PreAI(NPC npc)
    {
        if (showdownState != NPCShowdownState.Active) return base.PreAI(npc);
        return false;
    }
    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (showdownState != NPCShowdownState.None) return Main.graphics.GraphicsDevice.GetRenderTargets().Any(rtb => rtb.RenderTarget == Outline.NPCTarget);

        return true;
    }
}
