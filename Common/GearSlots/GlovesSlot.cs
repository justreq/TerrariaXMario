using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearTypeAttribute(GearType.Gloves)]
internal class GlovesSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 1);
}