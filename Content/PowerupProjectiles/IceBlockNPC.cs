using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.PowerupProjectiles;
public class IceBlockNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public IceBlock? iceBlock;
    public bool IsFrozen => iceBlock != null;

    public override bool PreAI(NPC npc)
    {
        if (IsFrozen) return false;
        return base.PreAI(npc);
    }

    public override void OnKill(NPC npc)
    {
        if (IsFrozen) iceBlock?.Projectile.Kill();
        base.OnKill(npc);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (IsFrozen && projectile.ModProjectile is not IceBlock) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (IsFrozen) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (IsFrozen) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool CanHitNPC(NPC npc, NPC target)
    {
        if (IsFrozen) return false;
        return base.CanHitNPC(npc, target);
    }

    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        if (IsFrozen) return false;
        return base.CanHitPlayer(npc, target, ref cooldownSlot);
    }

    public override void FindFrame(NPC npc, int frameHeight)
    {
        if (IsFrozen)
        {
            npc.frame.X = npc.frame.Y = 0;
            return;
        }

        base.FindFrame(npc, frameHeight);
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);

        if (IsFrozen)
        {
            Vector2 size = npc.frame.Size();
            int width = (int)size.X;
            int height = (int)size.Y;

            Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<IceBlock>()].Value;
            Color color = Color.White * 0.75f;

            Rectangle worldRect = new((int)(npc.Center.X - width * 0.5f), (int)(npc.Center.Y - height * 0.5f), width, height);
            Vector2 topLeft = worldRect.TopLeft();
            Vector2 topRight = worldRect.TopRight();
            Vector2 bottomLeft = worldRect.BottomLeft();
            Vector2 bottomRight = worldRect.BottomRight();


            Main.EntitySpriteDraw(texture, topLeft - Main.screenPosition + size * 0.5f, new Rectangle(0, 0, 4, 4), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(texture, topRight - Main.screenPosition + size * 0.5f - new Vector2(4f, 0f), new Rectangle(18, 0, 4, 4), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(texture, bottomLeft - Main.screenPosition + size * 0.5f - new Vector2(0f, 4f), new Rectangle(0, 18, 4, 4), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(texture, bottomRight - Main.screenPosition + size * 0.5f - new Vector2(4f), new Rectangle(18, 18, 4, 4), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Vector2 sourcePosition = topLeft - Main.screenPosition + new Vector2(0f, 4f);
            Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 4, height - 8), new Rectangle(0, 4, 4, 14), color));

            sourcePosition = topLeft - Main.screenPosition + new Vector2(4f, 0f);
            Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, width - 8, height - 4), new Rectangle(4, 0, 14, 18), color));

            sourcePosition = topRight - Main.screenPosition - new Vector2(4f, -4f);
            Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 4, height - 8), new Rectangle(18, 4, 4, 14), color));

            sourcePosition = bottomLeft - Main.screenPosition + new Vector2(4f, -4f);
            Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, width - 8, 4), new Rectangle(4, 18, 14, 4), color));

            Main.EntitySpriteDraw(texture, topLeft - Main.screenPosition + size * 0.5f, new Rectangle(22, 0, 14, 12), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(texture, bottomRight - Main.screenPosition + size * 0.5f - new Vector2(8f), new Rectangle(36, 14, 8, 8), color, 0, size * 0.5f, npc.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)(topLeft.X - Main.screenPosition.X) + 4, (int)(topLeft.Y - Main.screenPosition.Y) + 4, width - 8, (int)(height * 0.67f)), new Rectangle(44, 0, 22, 22), color * 0.35f));
        }
    }
}
