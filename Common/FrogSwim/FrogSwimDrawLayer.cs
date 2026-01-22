using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.FrogSwim;

internal class FrogSwimDrawLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.frogSwimming ?? false;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        int frame = modPlayer?.frogSwimFrame ?? 0;
        int xOffset;

        xOffset = frame == 2 ? 8 : 6;

        Vector2 position = player.MountedCenter - Main.screenPosition - new Vector2(xOffset * player.direction, 3);
        position = new((int)position.X, (int)position.Y);

        drawInfo.DrawDataCache.Add(new(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/FrogSwim{modPlayer?.currentCap ?? "Mario"}").Value, position, new Rectangle(0, 40 * (frame), 56, 40), Color.White, player.fullRotation, new Vector2(28, 20), 1, player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
    }
}