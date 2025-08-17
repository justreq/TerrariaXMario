using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearContextAttribute(GearContext.Gloves)]
internal class GlovesSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 1);
}