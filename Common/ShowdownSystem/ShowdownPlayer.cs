using SubworldLibrary;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownPlayer : ModPlayer
{
    CapEffectsPlayer? CapEffectsPlayer => Player.GetModPlayerOrNull<CapEffectsPlayer>();
    KeybindPlayer? KeybindPlayer => Player.GetModPlayerOrNull<KeybindPlayer>();

    internal int? showdownNPCIndex;

    internal bool IsPlayerInShowdownSubworld => ModContent.GetInstance<ShowdownSubworld>().currentPlayer == Player.whoAmI;

    internal void EndShowdownQuery()
    {
        if (showdownNPCIndex == null) return;

        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = null;
        KeybindPlayer?.keybindToShowInIndicator = null;
        Outline.outlineNeeded = false;
    }

    internal void EndShowdown()
    {
        if (showdownNPCIndex == null || !IsPlayerInShowdownSubworld) return;

        SubworldSystem.Exit();
        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = null;
        Outline.outlineNeeded = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (IsPlayerInShowdownSubworld || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        if (showdownNPCIndex != null) Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = target.whoAmI;
        target.GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.Query;
        KeybindPlayer?.keybindToShowInIndicator = KeybindSystem.KeybindSystem.EnterShowdownKeybind?.GetAssignedKeys().FirstOrDefault();
        Outline.outlineNeeded = true;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (IsPlayerInShowdownSubworld || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        if (showdownNPCIndex != null) Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = npc.whoAmI;
        npc.GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.Query;
        KeybindPlayer?.keybindToShowInIndicator = KeybindSystem.KeybindSystem.EnterShowdownKeybind?.GetAssignedKeys().FirstOrDefault();
        Outline.outlineNeeded = true;
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        if (IsPlayerInShowdownSubworld) return false;
        return base.CanBeHitByNPC(npc, ref cooldownSlot);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (IsPlayerInShowdownSubworld) return false;
        return base.CanBeHitByProjectile(proj);
    }

    public override bool CanUseItem(Item item)
    {
        if (IsPlayerInShowdownSubworld) return false;
        return base.CanUseItem(item);
    }

    public override void PostUpdate()
    {
        if (!IsPlayerInShowdownSubworld) return;

        Player.immune = true;
        Main.mapStyle = 0;
        Main.MouseShowBuildingGrid = false;
        Main.GameZoomTarget = 2;

        if (PlayerInput.Triggers.JustPressed.OpenCreativePowersMenu) EndShowdown();
    }

    public override void PostUpdateEquips()
    {
        if (!IsPlayerInShowdownSubworld) return;

        Player.noKnockback = true;
    }

    public override void SetControls()
    {
        if (!IsPlayerInShowdownSubworld) return;

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
