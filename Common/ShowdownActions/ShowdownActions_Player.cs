using Microsoft.Xna.Framework;
using System;
using Terraria;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownActions;
internal class ActionJump : ShowdownAction
{
    Vector2 P0 = Vector2.Zero;
    Vector2 P3 = Vector2.Zero;
    Vector2 P2 = Vector2.Zero;
    Vector2 P1 = Vector2.Zero;

    static Vector2 EvaluateBezier(float t, Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
    {
        float u = 1 - t;
        return u * u * u * P0 +
               3 * u * u * t * P1 +
               3 * u * t * t * P2 +
               t * t * t * P3;
    }

    internal override void Update(Entity owner)
    {
        Player player = (Player)owner;
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        if (modPlayer == null) return;

        NPC npc = Main.npc[(int)modPlayer?.showdownNPCPuppetIndex!];
        P3 = npc.Top;

        if (updateCount < 30) player.controlRight = true;
        else
        {
            if (updateCount == 30)
            {
                player.controlJump = true;
                P0 = player.Bottom;
                Vector2 direction = P3 - P0;
                Vector2 normal = Vector2.Normalize(new Vector2(-direction.Y, direction.X));
                float arcHeight = MathHelper.Clamp(Math.Abs(P3.Y - P0.Y) * 1.5f, 100f, 300f);

                P1 = P0 + 0.3f * direction - arcHeight * normal;
                P2 = P3 - 0.3f * direction - arcHeight * normal;
            }
            else if (player.Bottom.X < npc.Top.X)
            {
                player.Bottom = EvaluateBezier(updateCount - 30, P0, P1, P2, P3);
            }
        }
    }
}

internal class ActionSpecial : ShowdownAction
{
    internal override void Update(Entity owner)
    {
        base.Update(owner);
    }
}

internal class ActionItem : ShowdownAction
{
    internal override void Update(Entity owner)
    {
        base.Update(owner);
    }
}

internal class ActionFlee : ShowdownAction
{
    internal override void Update(Entity owner)
    {
        Player player = (Player)owner;

        owner.direction = -1;
        player.controlLeft = true;
        if (updateCount > 100) player.GetModPlayerOrNull<ShowdownPlayer>()?.EndShowdown();
    }
}