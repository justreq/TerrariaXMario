using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal sealed class Patch_DrawAccSlots : BasePatch
{
    private static bool ShowGearSlots => Main.LocalPlayer.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false;

    internal override void Patch(Mod mod)
    {
        // Prevents vanilla accessory slots from drawing when gear slots are enabled
        MonoModHooks.Modify(typeof(AccessorySlotLoader).GetMethod("DrawAccSlots"), DrawAccSlots);
    }

    private void DrawAccSlots(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(0))) ThrowError("LdcI4");

        c.Emit(OpCodes.Pop);
        c.EmitDelegate(() => ShowGearSlots ? 10 : 0);

        if (!c.TryGotoNext(i => i.MatchLdcI4(3), i => i.MatchStloc(5), i => i.MatchBr(out _))) ThrowError("LdcI4, Stloc, Br");

        c.EmitDelegate(() => ShowGearSlots);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brtrue, label);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchBlt(out _))) ThrowError("Blt");

        c.MarkLabel(label);
    }
}