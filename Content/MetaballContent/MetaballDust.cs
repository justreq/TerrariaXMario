using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

    public virtual void DrawMetaball(Dust dust, SpriteBatch sb, Texture2D circleTexture)
    {
        Vector2 center = dust.position;

        Color color = Color.White;
        Vector2 targetSize = new(Radius);
        Vector2 scale = targetSize / circleTexture.Size() * dust.scale;

        Vector2 velocityScaleFactor = dust.velocity.SafeNormalize(Vector2.One);
        velocityScaleFactor.X += MathF.Sign(velocityScaleFactor.X);
        velocityScaleFactor.Y += MathF.Sign(velocityScaleFactor.Y);
        velocityScaleFactor.X = MathF.Abs(velocityScaleFactor.X);
        velocityScaleFactor.Y = MathF.Abs(velocityScaleFactor.Y);
        scale *= (velocityScaleFactor);
        float rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;

        //sb.Draw(tex, center - Main.screenPosition, Color.White);
        sb.Draw(circleTexture, center - Main.screenPosition, null, color, rotation, circleTexture.Size() / 2, scale, SpriteEffects.None, 0);
    }
}
