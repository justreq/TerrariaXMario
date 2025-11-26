using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.StatueForm;
internal class StatueDrawLayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        Vector2 position = player.Center - new Vector2(20, 31) - Main.screenPosition;
        position = new((int)position.X, (int)position.Y);

        drawInfo.DrawDataCache.Add(new(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/Statue{modPlayer?.currentCap ?? "Mario"}").Value, position, null, Color.White));
    }
}