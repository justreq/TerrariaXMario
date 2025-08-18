using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal sealed class DrawInventoryPatch : BasePatch
{
    private static bool ShowGearSlots => Main.LocalPlayer.GetModPlayerOrNull<GearSlotPlayer>()?.ShowGearSlots ?? false;

    internal override void Patch(Mod mod)
    {
        // Prevents vanilla armor slots and the defense counter from drawing when gear slots are enabled
        IL_Main.DrawInventory += IL_Main_DrawInventory;
    }

    private void IL_Main_DrawInventory(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel originalLabel = c.DefineLabel();

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchBrtrue(out originalLabel!))) ThrowError("Ldsfld, Brtrue");
        if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("DrawLoadoutButtons"))) ThrowError("Call");

        c.EmitDelegate(() => ShowGearSlots);
        c.Emit(OpCodes.Brtrue, originalLabel);
    }
}
