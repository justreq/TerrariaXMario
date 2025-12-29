using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.MetaballContent;

internal class MetaballDust : ModDust
{
    public override string Texture => null!;

    public override bool PreDraw(Dust dust) => false;

    internal virtual Color OutlineColor => Color.Gray;
    internal virtual Color FillColor => Color.White;
    internal virtual float Radius => 8;

    public override bool MidUpdate(Dust dust)
    {
        dust.rotation = dust.velocity.ToRotation();
        return true;
    }
}
