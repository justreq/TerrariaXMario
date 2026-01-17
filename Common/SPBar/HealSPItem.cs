using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.SPBar;

internal class HealSPItem : GlobalItem
{
    internal int healSP;

    public override bool InstancePerEntity => true;

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        base.ModifyTooltips(item, tooltips);

        if (healSP > 0)
        {
            int index = tooltips.FindIndex(e => e.Name == "Consumable");

            tooltips.Insert(index == -1 ? tooltips.Count - 1 : index, new(Mod, "HealSP", Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.HealSPTooltip").Format(healSP)));
        }
    }
}
