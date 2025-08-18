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
        // Prevents vanilla armor slots, the loadout buttons, and the defense counter from drawing when gear slots are enabled
        IL_Main.DrawInventory += IL_Main_DrawInventory;
    }

    private void IL_Main_DrawInventory(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel originalLabel = c.DefineLabel();

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("EquipPage"), i => i.MatchBrtrue(out originalLabel!))) ThrowError("Ldsfld, Brtrue");

        c.Index++;

        c.EmitDelegate((int EquipPage) => EquipPage == 0 && ShowGearSlots);
    }
}
