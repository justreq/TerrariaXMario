using Microsoft.Xna.Framework;

namespace TerrariaXMario.Common.GearSlots;
[GearTypeAttribute(GearType.Socks)]
internal class SocksSlot : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 2);
}