using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.CapEffects;
internal class ImpactDust : ModDust
{
    int timeLeft = 0;
    public override void OnSpawn(Dust dust)
    {
        timeLeft = 45;
        dust.rotation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(3) * 16, 16, 16);
    }

    public override bool Update(Dust dust)
    {
        if (timeLeft > 0) timeLeft--;

        dust.scale -= 0.05f;
        dust.position += dust.velocity * (timeLeft * 0.075f);
        if (dust.scale < 0) dust.active = false;

        return false;
    }
}
