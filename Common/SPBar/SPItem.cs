using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.SPBar;

internal class SPItem : GlobalItem
{
    internal int healSP;
    internal int useSP;

    public override bool InstancePerEntity => true;

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        base.ModifyTooltips(item, tooltips);

        int index = tooltips.FindIndex(e => e.Name == "Consumable");
        if (healSP > 0) tooltips.Insert(index == -1 ? tooltips.Count - 1 : index, new(Mod, "HealSP", Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.HealSPTooltip").Format(healSP)));
        if (useSP > 0) tooltips.Insert(index == -1 ? tooltips.Count - 1 : index, new(Mod, "UseSP", Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.UseSPTooltip").Format(useSP)));
    }
}
