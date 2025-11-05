using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Common.ShowdownActions;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownPlayer : ModPlayer
{
    CapEffectsPlayer? CapEffectsPlayer => Player.GetModPlayerOrNull<CapEffectsPlayer>();
    KeybindPlayer? KeybindPlayer => Player.GetModPlayerOrNull<KeybindPlayer>();

    internal int? showdownNPCIndex;
    internal int? showdownNPCPuppetIndex;
    internal int? showdownNPCNetType;
    internal int showdownNPCLife;

    internal int showdownQueryTimer = 300;

    internal bool isPlayerInShowdownSubworld;
    internal Vector2 lockCameraPosition;

    internal ShowdownAction? queriedAction;
    internal ShowdownAction? currentAction;

    private float defaultGameZoomTarget;
    private int defaultMapStyle;

    internal void BeginShowdownQuery(NPC npc)
    {
        if (isPlayerInShowdownSubworld || (!CapEffectsPlayer?.CanDoCapEffects ?? true)) return;

        if (showdownNPCIndex != null) Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = npc.whoAmI;
        showdownNPCNetType = npc.netID;
        showdownNPCLife = npc.life;
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

        defaultGameZoomTarget = Main.GameZoomTarget;
        defaultMapStyle = Main.mapStyle;
        showdownNPC.showdownState = NPCShowdownState.Active;
        isPlayerInShowdownSubworld = true;
        SubworldSystem.Enter<ShowdownSubworld>();
        Main.GameZoomTarget = 2.25f;
        Main.mapStyle = 0;
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

        Main.GameZoomTarget = defaultGameZoomTarget;
        Main.mapStyle = defaultMapStyle;
        isPlayerInShowdownSubworld = false;
        SubworldSystem.Exit();
        Main.npc[(int)showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        showdownNPCIndex = null;
        showdownNPCNetType = null;
        Outline.outlineNeeded = false;
        queriedAction = null;
        currentAction = null;
    }

    public override void OnEnterWorld()
    {
        if (isPlayerInShowdownSubworld)
        {
            foreach (Item item in Player.inventory)
            {
                item.TurnToAir();
            }

            lockCameraPosition = Main.screenPosition + new Vector2(Main.screenWidth / Main.GameZoomTarget * 0.25f, -Main.screenHeight / Main.GameZoomTarget * 0.15f);
            NPC npc = NPC.NewNPCDirect(Player.GetSource_Misc("Showdown"), Player.Bottom + new Vector2(Main.screenWidth / Main.GameZoomTarget * 0.5f, Main.npc[(int)showdownNPCIndex!].height * 0.5f), (int)showdownNPCNetType!);
            showdownNPCPuppetIndex = npc.whoAmI;

            ShowdownNPC? globalNPC = npc.GetGlobalNPCOrNull<ShowdownNPC>();
            globalNPC?.showdownState = NPCShowdownState.Active;
            globalNPC?.isCopyOfShowdownNPC = true;
            npc.life = showdownNPCLife;

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
        if (isPlayerInShowdownSubworld && currentAction != null) return false;
        return base.CanBeHitByNPC(npc, ref cooldownSlot);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (isPlayerInShowdownSubworld && currentAction != null) return false;
        return base.CanBeHitByProjectile(proj);
    }

    public override bool CanUseItem(Item item)
    {
        if (isPlayerInShowdownSubworld && currentAction != null) return false;
        return base.CanUseItem(item);
    }

    public override void PostUpdate()
    {
        if (showdownQueryTimer > 0) showdownQueryTimer--;
        else if (!isPlayerInShowdownSubworld) EndShowdownQuery();

        if (!isPlayerInShowdownSubworld) return;

        Main.playerInventory = false;

        if (currentAction == null) Player.direction = 1;
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

        if (queriedAction != null || (currentAction != null && KeybindPlayer?.keyToShowInIndicator != KeybindSystem.KeybindSystem.GetVanillaKeybindKey(TriggerNames.Jump))) Player.controlJump = false;

        currentAction?.Update(Player);
        currentAction?.updateCount += 1;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!isPlayerInShowdownSubworld) return;

        if (PlayerInput.Triggers.JustPressed.OpenCreativePowersMenu) EndShowdown();

        if (PlayerInput.Triggers.JustPressed.Inventory)
        {
            if (queriedAction == null) IngameOptions.Open();
            else if (currentAction == null)
            {
                KeybindPlayer?.keyToShowInIndicator = null;
                queriedAction = null;
            }
        }

        if (PlayerInput.Triggers.JustPressed.Jump && queriedAction != null && KeybindPlayer?.keyToShowInIndicator != null)
        {
            currentAction = queriedAction;

            queriedAction = null;
            KeybindPlayer?.keyToShowInIndicator = null;
        }

        if ((PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right) && queriedAction == null && currentAction == null && Player.IsOnGroundPrecise())
        {
            ActionRing? actionRing = (ActionRing?)Main.projectile.FirstOrDefault(e => e.type == ModContent.ProjectileType<ActionRing>())?.ModProjectile;

            if (actionRing != null)
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Showdown/ScrollActions") { Volume = 0.2f });

                if (PlayerInput.Triggers.JustPressed.Left) actionRing.currentAction = (actionRing.currentAction == 0 ? actionRing.ActionCount : actionRing.currentAction) - 1;
                if (PlayerInput.Triggers.JustPressed.Right) actionRing.currentAction = actionRing.currentAction == actionRing.ActionCount - 1 ? 0 : actionRing.currentAction + 1;
            }
        }
    }
}
