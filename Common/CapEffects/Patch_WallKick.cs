using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal sealed class Patch_WallKick : BasePatch
{
    internal override void Patch(Mod mod)
    {
        IL_Player.JumpMovement += IL_Player_JumpMovement;
    }

    private void IL_Player_JumpMovement(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchLdfld<Player>("slideDir"))) ThrowError("ldfld");

        c.Index -= 2;

        c.Remove();
        c.EmitLdarg(0);
        c.EmitDelegate((Player player) => (player.GetModPlayerOrNull<CapPlayer>()?.CanDoCapEffects ?? false) ? 5 : 3);
    }
}
