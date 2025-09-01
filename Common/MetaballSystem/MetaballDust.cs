using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.MetaballSystem;
internal class MetaballDust : ModDust
{
    internal Color outlineColor = Color.Gray;
    internal Color fillColor = Color.White;

    public override string Texture => base.Texture.Replace("Dust", "");

    public override bool PreDraw(Dust dust) => false;
}
