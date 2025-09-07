using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.PowerupProjectiles;

namespace TerrariaXMario.Common.CapEffects;
internal class GrabPlayer : CapEffectsPlayer
{
    public GrabbableProjectile? QueriedGrabProjectile;
    public GrabbableProjectile? GrabProjectile;

    public void InitiateGrabProjectile()
    {
        if (Main.cursorOverride == TerrariaXMario.Instance.CursorGrabIndex && QueriedGrabProjectile != null)
        {
            GrabProjectile = QueriedGrabProjectile;
            GrabProjectile.grabOwner = Player.whoAmI;
            QueriedGrabProjectile = null;
        }
    }

    public void InitiateThrowProjectile()
    {
        if (Main.cursorOverride == TerrariaXMario.Instance.CursorThrowIndex && GrabProjectile != null)
        {
            GrabProjectile.Throw();
            GrabProjectile = null;
        }
    }

    public override bool CanUseItem(Item item)
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return base.CanUseItem(item);

        if (Main.cursorOverride == TerrariaXMario.Instance.CursorGrabIndex || Main.cursorOverride == TerrariaXMario.Instance.CursorThrowIndex) return false;
        return base.CanUseItem(item);
    }

    public override void PreUpdate()
    {
        if (GrabProjectile != null) Main.cursorOverride = TerrariaXMario.Instance.CursorThrowIndex;
        else if (Main.projectile.SkipLast(1).Where(e => e.ModProjectile is GrabbableProjectile projectile).Any(e =>
        {
            if (e.ModProjectile is IceBlock iceBlock && iceBlock.npc?.type == NPCID.TargetDummy)
            {
                QueriedGrabProjectile = null;
                return false;
            }

            if (e.getRect().Contains(Main.MouseWorld.ToPoint()) && e.position.Distance(Player.position) < 64)
            {
                QueriedGrabProjectile = e.ModProjectile as GrabbableProjectile;
                return true;
            }

            QueriedGrabProjectile = null;
            return false;
        })) Main.cursorOverride = TerrariaXMario.Instance.CursorGrabIndex;

        if (PlayerInput.Triggers.JustPressed.MouseLeft && !Player.mouseInterface)
        {
            InitiateGrabProjectile();
            InitiateThrowProjectile();
        }
    }
}
