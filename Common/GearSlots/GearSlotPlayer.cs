using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal class GearSlotPlayer : ModPlayer
{
    internal bool showGearSlots;
}

#if DEBUG
internal class GearSlotToggleCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        GearSlotPlayer? player = caller.Player.GetModPlayerOrNull<GearSlotPlayer>();
        if (player != null) player.showGearSlots ^= true;
    }

    public override string Command => "gay";

    public override CommandType Type => CommandType.Chat;
}
#endif