using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class Patch_PlayerFrame : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Player.PlayerFrame += On_Player_PlayerFrame;
    }

    private void On_Player_PlayerFrame(On_Player.orig_PlayerFrame orig, Player self)
    {
        orig(self);

        CapEffectsPlayer? modPlayer = self.GetModPlayerOrNull<CapEffectsPlayer>();
        if (modPlayer?.forceLegFrameY != null) self.legFrame.Y = (int)modPlayer.forceLegFrameY * 56;
    }
}
