using Microsoft.Xna.Framework;

namespace TerrariaXMario.Content.PowerupProjectiles;

internal class IceFlowerIceballDust : FireFlowerFireballDust
{
    internal override Color OutlineColor => new(0, 143, 254);
    internal override Color FillColor => new(180, 254, 254);
}
