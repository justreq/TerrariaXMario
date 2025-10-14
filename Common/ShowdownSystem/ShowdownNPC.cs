using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownNPC : GlobalNPC
{
    internal bool queryShowdown;
    internal bool inShowdown;

    public override bool InstancePerEntity => true;

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (inShowdown) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (inShowdown) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (inShowdown) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (queryShowdown || inShowdown)
        {
            spriteBatch.End();

            Effect outline = ModContent.Request<Effect>($"{nameof(TerrariaXMario)}/Core/Effects/Outline", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            outline.Parameters["uSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            outline.Parameters["uColor"].SetValue(Main.OurFavoriteColor.ToVector4());
            outline.Parameters["uThreshold"].SetValue(0.001f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, outline, Main.GameViewMatrix.TransformationMatrix);
            outline.CurrentTechnique.Passes[0].Apply();
        }

        return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
    }
}
