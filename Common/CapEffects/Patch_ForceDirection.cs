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

        CapEffectsPlayer? modPlayer = self.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer != null && modPlayer.forceDirection != 0) self.direction = modPlayer.forceDirection;

    }

    private void On_Player_ChangeDir(On_Player.orig_ChangeDir orig, Player self, int dir)
    {
        orig(self, dir);

        CapEffectsPlayer? modPlayer = self.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer != null && modPlayer.forceDirection != 0) self.direction = modPlayer.forceDirection;
    }
}