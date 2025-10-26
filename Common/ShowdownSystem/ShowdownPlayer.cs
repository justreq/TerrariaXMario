using Microsoft.Xna.Framework;
using SteelSeries.GameSense.DeviceZone;
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
internal enum ShowdownAction
{
    None,
    Jump,
    Special,
    Item,
    Flee
}
internal class ShowdownPlayer : ModPlayer
{
    CapEffectsPlayer? CapEffectsPlayer => Player.GetModPlayerOrNull<CapEffectsPlayer>();
    KeybindPlayer? KeybindPlayer => Player.GetModPlayerOrNull<KeybindPlayer>();

    internal int? showdownNPCIndex;
    internal int? showdownNPCPuppetIndex;
    internal int? showdownNPCNetType;

    internal int showdownQueryTimer = 300;

    internal bool isPlayerInShowdownSubworld;
    internal Vector2 lockCameraPosition;

    internal ShowdownAction queriedAction;
    internal ShowdownAction currentAction;

    internal int actionTime;
    private float defaultZoom;

    internal void BeginShowdownQuery(NPC npc)
    {
        if (isPlayerInShowdownSubworld || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        if (showdownNPCIndex != null) Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = npc.whoAmI;
        showdownNPCNetType = npc.netID;
        npc.GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.Query;
        KeybindPlayer?.keyToShowInIndicator = KeybindSystem.KeybindSystem.EnterShowdownKeybind?.GetAssignedKeys().FirstOrDefault();
        Outline.outlineNeeded = true;
        showdownQueryTimer = 300;
    }

    internal void BeginShowdown()
    {
        if (showdownNPCIndex == null || showdownNPCNetType == null || isPlayerInShowdownSubworld || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        ShowdownNPC? showdownNPC = Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>();
        if (showdownNPC?.showdownState != NPCShowdownState.Query) return;

        defaultZoom = Main.GameZoomTarget;
        showdownNPC.showdownState = NPCShowdownState.Active;
        isPlayerInShowdownSubworld = true;
        SubworldSystem.Enter<ShowdownSubworld>();
        Main.GameZoomTarget = 2.5f;
        KeybindPlayer?.keyToShowInIndicator = null;
    }

    internal void EndShowdownQuery()
    {
        if (showdownNPCIndex == null || showdownNPCNetType == null) return;

        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = null;
        showdownNPCNetType = null;
        KeybindPlayer?.keyToShowInIndicator = null;
        Outline.outlineNeeded = false;
    }

    internal void EndShowdown()
    {
        if (showdownNPCIndex == null || showdownNPCNetType == null || !isPlayerInShowdownSubworld) return;
        SubworldSystem.Exit();
        Main.GameZoomTarget = defaultZoom;
        isPlayerInShowdownSubworld = false;
        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = null;
        showdownNPCNetType = null;
        Outline.outlineNeeded = false;
        queriedAction = ShowdownAction.None;
        currentAction = ShowdownAction.None;
    }

    public override void OnEnterWorld()
    {
        if (isPlayerInShowdownSubworld)
        {
            foreach (Item item in Player.inventory)
            {
                item.TurnToAir();
            }

            lockCameraPosition = Main.screenPosition + new Vector2(Main.screenWidth / Main.GameZoomTarget * 0.25f, 0);
            NPC npc = NPC.NewNPCDirect(Player.GetSource_Misc("Showdown"), Player.Bottom + new Vector2(Main.screenWidth / Main.GameZoomTarget * 0.5f, Main.npc[(int)showdownNPCIndex!].height * 0.5f), (int)showdownNPCNetType!);
            showdownNPCPuppetIndex = npc.whoAmI;

            ShowdownNPC? globalNPC = npc.GetGlobalNPCOrNull<ShowdownNPC>();
            globalNPC?.showdownState = NPCShowdownState.Active;
            globalNPC?.isCopyOfShowdownNPC = true;

            CapEffectsPlayer?.KillStompHitbox();
            Projectile.NewProjectile(Player.GetSource_Misc("Showdown"), Player.Top - new Vector2(0, 64), Vector2.Zero, ModContent.ProjectileType<ActionRing>(), 0, 0, Player.whoAmI);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        BeginShowdownQuery(target);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        BeginShowdownQuery(npc);
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        if (isPlayerInShowdownSubworld) return false;
        return base.CanBeHitByNPC(npc, ref cooldownSlot);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (isPlayerInShowdownSubworld) return false;
        return base.CanBeHitByProjectile(proj);
    }

    public override bool CanUseItem(Item item)
    {
        if (isPlayerInShowdownSubworld) return false;
        return base.CanUseItem(item);
    }

    public override void PostUpdate()
    {
        if (showdownQueryTimer > 0) showdownQueryTimer--;
        else if (!isPlayerInShowdownSubworld) EndShowdownQuery();

        if (!isPlayerInShowdownSubworld) return;

        if (PlayerInput.Triggers.JustPressed.Inventory && queriedAction != ShowdownAction.None && currentAction == ShowdownAction.None)
        {
            KeybindPlayer?.keyToShowInIndicator = null;
            queriedAction = ShowdownAction.None;
        }

        if (PlayerInput.Triggers.JustPressed.Jump && queriedAction == ShowdownAction.Flee)
        {
            KeybindPlayer?.keyToShowInIndicator = null;
            queriedAction = ShowdownAction.None;
            currentAction = ShowdownAction.Flee;
        }

        if (currentAction != ShowdownAction.None)
        {
            actionTime++;

            if (currentAction == ShowdownAction.Flee) Player.direction = -1;
        }
        else
        {
            Player.direction = 1;
            actionTime = 0;
        }
    }

    public override void PostUpdateEquips()
    {
        if (!isPlayerInShowdownSubworld) return;

        Player.noKnockback = true;
    }

    public override void SetControls()
    {
        if (!isPlayerInShowdownSubworld) return;

        Player.controlCreativeMenu = false;
        Player.controlDown = false;
        Player.controlDownHold = false;
        Player.controlHook = false;
        Player.controlInv = false;
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

        if (queriedAction != ShowdownAction.None || currentAction != ShowdownAction.None) Player.controlJump = false;

        switch (currentAction)
        {
            case ShowdownAction.None:
                break;
            case ShowdownAction.Jump:
                break;
            case ShowdownAction.Special:
                break;
            case ShowdownAction.Item:
                break;
            case ShowdownAction.Flee:
                Player.controlLeft = true;
                if (actionTime > 100) EndShowdown();
                break;
            default:
                break;
        }
    }
}
