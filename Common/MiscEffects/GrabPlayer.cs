using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameInput;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.MiscEffects;
internal class GrabPlayer : CapEffectsPlayer
{
    internal NPC? hoverNPC;
    internal NPC? grabbedNPC;

    public override void PreUpdate()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        if (grabbedNPC != null)
        {
            IceBlockNPC? globalNPC = grabbedNPC.GetGlobalNPCOrNull<IceBlockNPC>();

            if (!globalNPC?.frozen ?? true)
            {
                grabbedNPC = null;
                return;
            }

            Main.cursorOverride = TerrariaXMario.Instance.CursorThrowIndex;

            grabbedNPC.Bottom = Player.Top;

            if (PlayerInput.Triggers.JustPressed.MouseLeft)
            {
                CapPlayer?.SetForceDirection(10, Math.Sign(Main.MouseWorld.X - Player.position.X));
                grabbedNPC.velocity.X = 7.5f * CapPlayer!.forceDirection;
                globalNPC?.thrown = true;
                grabbedNPC = null;
            }
        }
        else if (hoverNPC != null)
        {
            IceBlockNPC? globalNPC = hoverNPC.GetGlobalNPCOrNull<IceBlockNPC>();
            if (globalNPC == null) return;

            if (!globalNPC.frozen || globalNPC.thrown || !globalNPC.iceBlockRect.Intersects(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 1, 1)))
            {
                hoverNPC = null;
                return;
            }

            Main.cursorOverride = TerrariaXMario.Instance.CursorGrabIndex;

            if (PlayerInput.Triggers.JustPressed.MouseLeft) grabbedNPC = hoverNPC;
        }
    }

    public override void PostUpdate()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        if (grabbedNPC != null)
        {
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
        }
    }
}
