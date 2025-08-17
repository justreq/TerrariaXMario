using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearContextAttribute(GearContext.Boots)]
internal class BootsSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 2);
}