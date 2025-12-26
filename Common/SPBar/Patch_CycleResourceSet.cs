using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;
using TerrariaXMario.Core;

namespace TerrariaXMario.Common.SPBar;

internal class Patch_CycleResourceSet : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_PlayerResourceSetsManager.CycleResourceSet += On_PlayerResourceSetsManager_CycleResourceSet;
    }

    private void On_PlayerResourceSetsManager_CycleResourceSet(On_PlayerResourceSetsManager.orig_CycleResourceSet orig, PlayerResourceSetsManager self)
    {
        orig(self);
        ModContent.GetInstance<SPBar>()?.ChangeResourceSet(self.ActiveSet.DisplayedName);
    }
}
