using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class ObjectSpawnerBlockUI : UIState
{
    private UIElement? Container { get; set; }
    private UIPanel? ObjectPanel { get; set; }
    private UIPanel? ListPanel { get; set; }
    private UIText? HowToUseText { get; set; }

    public override void OnInitialize()
    {
        Container = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(520);
            e.Height = StyleDimension.FromPixels(256);
            e.HAlign = 0.5f;
            e.VAlign = 0.8f;
        }));

        ObjectPanel = Container.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-4, 0.5f);
            e.Height = StyleDimension.Fill;
            e.BorderColor = new Color(63, 82, 151);
            e.BackgroundColor = new Color(43, 62, 131) * 0.7f;
        }));

        ListPanel = Container.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-4, 0.5f);
            e.Height = StyleDimension.Fill;
            e.HAlign = 1;
            e.BorderColor = new Color(63, 82, 151);
            e.BackgroundColor = new Color(43, 62, 131) * 0.85f;
        }));

        HowToUseText = ListPanel.AddElement(new UIText("Select an object from the left panel. Edit any properties or reorder the list from this panel.\n\nStrike the block to get its contents in listed order (from top to bottom).").With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-16, 1);
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
            e.IsWrapped = true;
            e.WrappedTextBottomPadding = 0;
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if ((modPlayer?.CapPlayer?.CanDoCapEffects ?? false) && modPlayer.currentObjectSpawnerBlockToEdit != Vector2.Zero) base.Draw(spriteBatch);
        else if (ObjectPanel?.Children.Any() ?? false) ObjectPanel.RemoveAllChildren();
    }

    public override void Update(GameTime gameTime)
    {
        if (Container?.IsMouseHovering ?? false) Main.LocalPlayer.mouseInterface = true;

        if ((!ObjectPanel?.Children.Any() ?? false) && TerrariaXMario.Instance.spawnableObjects != null)
        {
            UIGrid objectGrid = ObjectPanel!.AddElement(new UIGrid().With(e =>
            {
                e.Width = StyleDimension.FromPixelsAndPercent(-32, 1);
                e.Height = StyleDimension.Fill;
                e.ListPadding = 8;
            }));

            ObjectPanel!.AddElement(new UIScrollbar().With(e =>
            {
                e.HAlign = 1f;
                e.VAlign = 0.5f;
                e.Height = StyleDimension.FromPixelsAndPercent(-16f, 1f);
                objectGrid.SetScrollbar(e);
            }));

            for (int i = 0; i < TerrariaXMario.Instance.spawnableObjects.Length; i++)
            {
                if (i == 0)
                {
                    Main.instance.LoadItem(ItemID.GoldCoin);

                    objectGrid.AddElement(new UIHoverImageButton(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel"), "Gold Coin").With(e =>
                    {
                        e.SetVisibility(1, 1);
                        e.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
                        e.OnLeftClick += OnClickObjectButton;

                        e.AddElement(new UIImage(TextureAssets.Item[ItemID.GoldCoin])).With(e =>
                        {
                            e.HAlign = 0.5f;
                            e.VAlign = 0.5f;
                        });
                    }));
                }

                var obj = TerrariaXMario.Instance.spawnableObjects[i];

                UIHoverImageButton button = objectGrid.AddElement(new UIHoverImageButton(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel"), obj is ModItem or ModProjectile ? Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.{(obj is ModItem ? "Items" : "Projectiles")}.{obj.GetType().Name}.DisplayName") : "").With(e =>
                {
                    e.SetVisibility(1, 1);
                    e.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
                    e.OnLeftClick += OnClickObjectButton;
                }));

                UIImage image = button.AddElement(new UIImage(obj is ModItem modItem ? ModContent.Request<Texture2D>(modItem.Texture) : obj is ModProjectile modProjectile ? ModContent.Request<Texture2D>(modProjectile.Texture) : TextureAssets.Item[0]).With(e =>
                {
                    e.HAlign = 0.5f;
                    e.VAlign = 0.5f;
                }));
            }
        }
    }

    private void OnClickObjectButton(UIMouseEvent evt, UIElement listeningElement)
    {
        int index = listeningElement.Parent.Children.ToList().IndexOf(listeningElement);
    }
}
