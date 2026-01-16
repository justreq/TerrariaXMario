using Microsoft.Xna.Framework;
using System;
using Terraria;
using TerrariaXMario.Content.MetaballContent;

namespace TerrariaXMario.Content.PowerupProjectiles;

internal class FireFlowerFireballDust : MetaballDust
{
    internal override Color OutlineColor => new(249, 28, 26);
    internal override Color FillColor => new(246, 225, 21);
    internal override float Radius => 10;

    public override void OnSpawn(Dust dust)
    {
        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        dust.fadeIn = Main.rand.NextFloat(0.02f, 0.04f);
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.velocity *= 0.95f;

        dust.frame = new Rectangle(0, (int)Math.Clamp(Math.Floor(2 - dust.scale * 2), 0, 2) * 22, 22, 22);

        dust.rotation += dust.fadeIn;
        dust.scale -= 0.025f;
        if (dust.scale <= 0) dust.active = false;

        dust.alpha = (int)Math.Clamp(200 - Math.Sin(dust.scale) * 200, 0, 200);

        return false;
    }
}
