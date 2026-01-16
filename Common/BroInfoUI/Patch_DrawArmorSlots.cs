using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal sealed class Patch_DrawArmorSlots : BasePatch
{
    internal override void Patch(Mod mod)
    {
        // Prevents vanilla armor slots, the loadout buttons, and the defense counter from drawing when gear slots are enabled
        IL_Main.DrawInventory += IL_Main_DrawInventory;
    }

    private void IL_Main_DrawInventory(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchBrtrue(out label!))) ThrowError("Ldsfld, Brtrue");

        c.EmitDelegate(() => Main.LocalPlayer.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? false);
        c.EmitBrtrue(label);
    }
}
