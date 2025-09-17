using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class UIHoverImageButton(Asset<Texture2D> texture, string hoverText) : UIImageButton(texture)
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (IsMouseHovering) Main.hoverItemName = hoverText;
    }
}
