using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.CapEffects;
internal class ImpactDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(3) * 16, 16, 16);
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.velocity -= new Vector2(0.01f);
        dust.scale -= 0.01f;
        dust.color.A -= (byte)0.01f;

        if (dust.scale < 0.5f) dust.active = false;

        return false;
    }
}
