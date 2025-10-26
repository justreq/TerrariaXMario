using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal enum NPCShowdownState
{
    None,
    Query,
    Active
}
internal class ShowdownNPC : GlobalNPC
{
    internal NPCShowdownState showdownState;
    internal bool isCopyOfShowdownNPC;

    public override bool InstancePerEntity => true;

    public override bool NeedSaving(NPC npc)
    {
        if (showdownState == NPCShowdownState.Active && !isCopyOfShowdownNPC) return true;

        return base.NeedSaving(npc);
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (showdownState == NPCShowdownState.Active && !isCopyOfShowdownNPC) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (showdownState == NPCShowdownState.Active) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (showdownState == NPCShowdownState.Active && !isCopyOfShowdownNPC) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool PreAI(NPC npc)
    {
        if (showdownState != NPCShowdownState.Active) return base.PreAI(npc);
        return false;
    }

    public override void AI(NPC npc)
    {
        if (showdownState != NPCShowdownState.Active)
        {
            base.AI(npc);
            return;
        }
    }

    public override void PostAI(NPC npc)
    {
        if (showdownState != NPCShowdownState.Active)
        {
            base.PostAI(npc);
            return;
        }

        npc.velocity = Vector2.Zero;

        if (isCopyOfShowdownNPC)
        {
            // turn based state machine
        }
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (isCopyOfShowdownNPC) return true;
        if (showdownState != NPCShowdownState.None) return Main.graphics.GraphicsDevice.GetRenderTargets().Any(rtb => rtb.RenderTarget == Outline.NPCTarget);

        return true;
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (player.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false) maxSpawns = 0;
        base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
    }
}
