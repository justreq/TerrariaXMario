using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class ObjectSpawnerBlockUI : UIState
{
    private UIElement? Container { get; set; }
    private UIPanel? Panel { get; set; }
    private UIElement? CarouselContainer { get; set; }

    public override void OnInitialize()
    {
        Container = new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(384);
            e.Height = StyleDimension.FromPixels(128);
            e.HAlign = 0.5f;
            e.VAlign = 0.75f;
        });

        Append(Container);

        Panel = new UIPanel(ModContent.Request<Texture2D>($"{TerrariaXMario.Textures}/AlternativePanelBackground"), ModContent.Request<Texture2D>($"{TerrariaXMario.Textures}/AlternativePanelBorder")).With(e =>
        {
            e.Width = StyleDimension.FromPixels(128);
            e.Height = StyleDimension.FromPixels(128);
            e.HAlign = 0.5f;
            e.BackgroundColor = Color.Black * 0.75f;
            e.BorderColor = Color.Black * 0.9f;
        });

        Container.Append(Panel);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer != null && (modPlayer.CapPlayer?.CanDoCapEffects ?? false) && modPlayer.currentObjectSpawnerBlockToEdit != Vector2.Zero) base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;

        if (Container?.IsMouseHovering ?? false) Main.LocalPlayer.mouseInterface = true;

        if (CarouselContainer == null && TerrariaXMario.Instance.spawnableObjects != null)
        {
            CarouselContainer = new UIElement().With(e =>
            {
                e.Width = StyleDimension.FromPixels(128 * TerrariaXMario.Instance.spawnableObjects.Length);
                e.Height = StyleDimension.FromPixels(128);
            });

            Container?.Append(CarouselContainer);

            for (int i = 0; i < TerrariaXMario.Instance.spawnableObjects.Length * 2; i++)
            {
                var obj = TerrariaXMario.Instance.spawnableObjects[i % TerrariaXMario.Instance.spawnableObjects.Length];

                UIElement container = new UIElement().With(e =>
                {
                    e.Width = StyleDimension.FromPixels(128);
                    e.Height = StyleDimension.FromPixels(128);
                    e.Left = StyleDimension.FromPixels(128 * i);
                });

                CarouselContainer.Append(container);

                UIImage image = new UIImage(obj is ModItem modItem ? ModContent.Request<Texture2D>(modItem.Texture) : obj is ModProjectile modProjectile ? ModContent.Request<Texture2D>(modProjectile.Texture) : obj is Item item ? TextureAssets.Item[item.type] : TextureAssets.Item[0]).With(e =>
                {
                    e.HAlign = 0.5f;
                    e.VAlign = 0.5f;
                    e.ImageScale = 2;
                });

                container.Append(image);
            }
        }
    }
}
