using Terraria.ModLoader;
using Terraria;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal sealed class Patch_ForceDirection : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Player.HorizontalMovement += On_Player_HorizontalMovement;
        On_Player.ChangeDir += On_Player_ChangeDir;
    }

    private void On_Player_HorizontalMovement(On_Player.orig_HorizontalMovement orig, Player self)
    {
        orig(self);

        CapPlayer? capPlayer = self.GetModPlayerOrNull<CapPlayer>();

        if (capPlayer != null && capPlayer.forceDirection != 0) self.direction = capPlayer.forceDirection;

    }

    private void On_Player_ChangeDir(On_Player.orig_ChangeDir orig, Player self, int dir)
    {
        orig(self, dir);

        CapPlayer? capPlayer = self.GetModPlayerOrNull<CapPlayer>();

        if (capPlayer != null && capPlayer.forceDirection != 0) self.direction = capPlayer.forceDirection;
    }
}