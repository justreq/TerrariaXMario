using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.MetaballSystem;
internal class MetaballProjectile : ModProjectile
{
    internal Color outlineColor = Color.Gray;
    internal Color fillColor = Color.White;

    public override string Texture => base.Texture.Replace("Projectile", "");

    public override bool PreDraw(ref Color lightColor) => false;
}
