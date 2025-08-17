using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearContextAttribute(GearContext.Overalls)]
internal class OverallsSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 1);
}