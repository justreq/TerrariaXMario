using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.MetaballContent;

internal abstract class MetaballProjectile : ModProjectile
{
    public override bool PreDraw(ref Color lightColor) => false;

    internal virtual Color OutlineColor => Color.Gray;
    internal virtual Color FillColor => Color.White;
    internal virtual float Radius => 8;
    internal virtual int? PairedMetaballDust => null;

    public override void PostAI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }
}