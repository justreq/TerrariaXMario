using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearTypeAttribute(GearType.Cap)]
internal class CapSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation();
}