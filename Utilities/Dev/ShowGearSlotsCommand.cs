#if DEBUG
using Terraria.ModLoader;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Utilities.Dev;
internal class GearSlotToggleCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        GearSlotPlayer? player = caller.Player.GetModPlayerOrNull<GearSlotPlayer>();
        if (player != null) player.ShowGearSlots ^= true;
    }

    public override string Command => "sg";

    public override CommandType Type => CommandType.Chat;
}
#endif