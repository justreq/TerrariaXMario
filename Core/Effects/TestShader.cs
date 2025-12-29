using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Core.Effects;

internal class TestShader : ModSystem
{
    internal string Directory => GetType().Namespace!.Replace(".", "/");

    public override void PostDrawTiles()
    {
        return;
        Effect effect = ModContent.Request<Effect>($"{Directory}/TestShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        effect.Parameters["color"].SetValue(Color.White.ToVector4());

        Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, effect);
        Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{Directory}/TestShaderTexture").Value, Main.MouseScreen - new Vector2(436, 524) * 0.5f, Color.White);
        Main.spriteBatch.End();
    }
}