using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearContextAttribute(GearContext.Cap)]
internal class CapSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation();
}