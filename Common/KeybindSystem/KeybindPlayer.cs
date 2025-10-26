using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.KeybindSystem;
internal class KeybindPlayer : ModPlayer
{
    internal string? keyToShowInIndicator;

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (KeybindSystem.EnterShowdownKeybind?.JustPressed ?? false) Player.GetModPlayerOrNull<ShowdownPlayer>()?.BeginShowdown();
    }
}
