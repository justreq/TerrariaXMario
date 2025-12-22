using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.SPBar;

internal class Patch_DrawMap : BasePatch
{
    // shift the minimap to give space for the sp bar
    internal override void Patch(Mod mod)
    {
        IL_Main.DrawMap += IL_Main_DrawMap;
    }

    private void IL_Main_DrawMap(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(e => e.MatchSub(), e => e.MatchStsfld<Main>("miniMapX"))) ThrowError("stsfld");

        c.EmitDelegate(() => Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.CanDoCapEffects ?? false ? -48 : 0);
        c.Emit(OpCodes.Sub);
    }
}
