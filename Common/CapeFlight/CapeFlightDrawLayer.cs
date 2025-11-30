using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapeFlight;
internal class CapeFlightDrawLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.doCapeFlight ?? false;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        Vector2 position = player.Center - new Vector2(16, 12) - Main.screenPosition;
        position = new((int)position.X, (int)position.Y);

        drawInfo.DrawDataCache.Add(new(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/CapeFlight{modPlayer?.currentCap ?? "Mario"}").Value, position, new Rectangle(0, 56 * (modPlayer?.CapeFrame ?? 0), 40, 56), Color.White, player.fullRotation, player.Size * 0.5f, 1, player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
    }
}