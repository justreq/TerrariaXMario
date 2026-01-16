using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;

namespace TerrariaXMario.Common.BroInfoUI;

internal sealed class Patch_DrawPageIcons : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Main.DrawPageIcons += On_Main_DrawPageIcons;
    }

    private int On_Main_DrawPageIcons(On_Main.orig_DrawPageIcons orig, int yPos)
    {
        TerrariaXMario.BroInfoPageButtonPosition = new Vector2(Main.screenWidth - 44, yPos - 6);

        return orig(yPos);
    }
}
