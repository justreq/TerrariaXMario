using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;

namespace TerrariaXMario.Common.MiscEffects;
internal sealed class Patch_PlayerFrame : BasePatch
{
    internal override void Patch(Mod mod)
    {
        IL_Player.PlayerFrame += IL_Player_PlayerFrame;
    }

    private void IL_Player_PlayerFrame(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("gamePaused"))) ThrowError("ldsfld");

        c.EmitPop();
        c.EmitDelegate(() => false);
    }
}
