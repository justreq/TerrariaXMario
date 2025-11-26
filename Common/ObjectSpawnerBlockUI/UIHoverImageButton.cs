using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class UIHoverImageButton : UIImageButton
{
    internal object hoverText;

    internal UIHoverImageButton(Asset<Texture2D> texture, string hoverText) : base(texture)
    {
        this.hoverText = hoverText;
    }

    internal UIHoverImageButton(Asset<Texture2D> texture, LocalizedText hoverText) : base(texture)
    {
        this.hoverText = hoverText;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (IsMouseHovering) Main.hoverItemName = hoverText is LocalizedText localized ? localized.Value : hoverText.ToString();
    }
}
