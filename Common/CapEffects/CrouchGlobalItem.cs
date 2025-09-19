using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class CrouchGlobalItem : GlobalItem
{
    public override bool CanUseItem(Item item, Player player)
    {
        return !(player.GetModPlayerOrNull<CapEffectsPlayer>()?.crouching ?? false) && base.CanUseItem(item, player);
    }
}
