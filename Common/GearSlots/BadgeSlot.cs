using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearTypeAttribute(GearType.Badge)]
internal class BadgeSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 3);
}