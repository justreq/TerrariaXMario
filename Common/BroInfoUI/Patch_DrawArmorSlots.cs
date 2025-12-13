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

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchBrtrue(out _))) ThrowError("Ldsfld, Brtrue");

        c.Index++;

        c.EmitDelegate((int EquipPage) => EquipPage == 0 && (Main.LocalPlayer.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? false));
    }
}
