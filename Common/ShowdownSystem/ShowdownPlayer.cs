using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownPlayer : ModPlayer
{
    CapEffectsPlayer? CapEffectsPlayer => Player.GetModPlayerOrNull<CapEffectsPlayer>();

    internal ShowdownState showdownState;

    internal int? showdownNPCIndex;

    internal bool DoShowdownEffects => showdownState == ShowdownState.Active && showdownNPCIndex != null && (CapEffectsPlayer?.CanDoCapEffects ?? false);

    internal void EndShowdown()
    {
        if (showdownNPCIndex == null || showdownState != ShowdownState.Active) return;

        showdownState = ShowdownState.None;
        IngameFancyUI.Close();
        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.inShowdown = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (showdownState == ShowdownState.Active || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        showdownState = ShowdownState.Queried;
        showdownNPCIndex = target.whoAmI;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (showdownState == ShowdownState.Active || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        showdownState = ShowdownState.Queried;
        showdownNPCIndex = npc.whoAmI;
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        if (showdownState == ShowdownState.Active) return false;
        return base.CanBeHitByNPC(npc, ref cooldownSlot);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (showdownState == ShowdownState.Active) return false;
        return base.CanBeHitByProjectile(proj);
    }

    public override bool CanUseItem(Item item)
    {
        if (showdownState == ShowdownState.Active) return false;
        return base.CanUseItem(item);
    }

    public override void PreUpdateMovement()
    {
        if (!DoShowdownEffects) return;

        Player.gravity = 0;
        Player.velocity = Vector2.Zero;
    }

    public override void PostUpdate()
    {
        if (!DoShowdownEffects) return;

        Player.immune = true;
        Main.mapStyle = 0;
        Main.MouseShowBuildingGrid = false;
        Main.GameZoomTarget = 2;

        if (PlayerInput.Triggers.JustPressed.OpenCreativePowersMenu) EndShowdown();
    }

    public override void SetControls()
    {
        if (!DoShowdownEffects) return;

        Player.controlCreativeMenu = false;
        Player.controlDown = false;
        Player.controlDownHold = false;
        Player.controlHook = false;
        Player.controlInv = false;
        Player.controlJump = false;
        Player.controlLeft = false;
        Player.controlMap = false;
        Player.controlMount = false;
        Player.controlQuickHeal = false;
        Player.controlQuickMana = false;
        Player.controlRight = false;
        Player.controlSmart = false;
        Player.controlThrow = false;
        Player.controlTorch = false;
        Player.controlUp = false;
        Player.controlUseItem = false;
        Player.controlUseTile = false;
        Main.playerInventory = false;
        Main.mapFullscreen = false;
    }
}
