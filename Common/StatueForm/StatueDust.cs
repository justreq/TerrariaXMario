using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.StatueForm;
internal class StatueDust : ModDust // credits: Momlob
{
    public override void OnSpawn(Dust dust)
    {
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.fadeIn = Main.rand.NextFloat(0.02f, 0.04f);
        dust.scale = 1.1f;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.velocity *= 0.85f;
        dust.velocity.Y -= 0.2f;

        dust.frame = new Rectangle(0, (int)Math.Clamp(Math.Floor(2 - dust.scale * 2), 0, 2) * 22, 22, 22);

        dust.rotation += dust.fadeIn;
        dust.scale -= 0.05f;
        if (dust.scale <= 0) dust.active = false;

        dust.alpha = (int)Math.Clamp(200 - Math.Sin(dust.scale) * 200, 0, 200);

        return false;
    }
}
