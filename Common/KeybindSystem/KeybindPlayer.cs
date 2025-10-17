using SubworldLibrary;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.KeybindSystem;
internal class KeybindPlayer : ModPlayer
{
    ShowdownPlayer? ShowdownPlayer => Player.GetModPlayerOrNull<ShowdownPlayer>();

    internal string? keybindToShowInIndicator;

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if ((KeybindSystem.EnterShowdownKeybind?.JustPressed ?? false) && ShowdownPlayer?.showdownNPCIndex != null && !ShowdownPlayer.IsPlayerInShowdownSubworld)
        {
            ShowdownNPC? showdownNPC = Main.npc[(int)ShowdownPlayer?.showdownNPCIndex!].GetGlobalNPCOrNull<ShowdownNPC>();

            if (showdownNPC?.showdownState == NPCShowdownState.Query)
            {
                showdownNPC.showdownState = NPCShowdownState.Active;
                SubworldSystem.Enter<ShowdownSubworld>();
                keybindToShowInIndicator = null;
            }
        }
    }
}
