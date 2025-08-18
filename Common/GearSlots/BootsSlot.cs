using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearTypeAttribute(GearType.Boots)]
internal class BootsSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 2);
}