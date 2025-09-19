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
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class ObjectSpawnerBlockUI : UIState
{
    private bool isLoaded;

    private UIElement? Container { get; set; }
    private UIGrid? ObjectGrid { get; set; }
    private UIScrollbar? ObjectGridScrollbar { get; set; }
    private UIPanel? FinalListContainer { get; set; }
    private UIList? FinalList { get; set; }
    private UIScrollbar? FinalListScrollbar { get; set; }
    private UIText? HowToUseText { get; set; }

    public override void OnInitialize()
    {
        Container = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(520);
            e.Height = StyleDimension.FromPixels(312);
            e.HAlign = 0.5f;
            e.VAlign = 0.8f;
        }));

        UIPanel objectGridContainer = Container.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-4, 0.5f);
            e.Height = StyleDimension.FromPixels(256);
            e.BorderColor = new Color(63, 82, 151);
            e.BackgroundColor = new Color(43, 62, 131) * 0.7f;
        }));

        ObjectGrid = objectGridContainer.AddElement(new UIGrid().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-32, 1);
            e.Height = StyleDimension.Fill;
            e.ListPadding = 8;
        }));

        ObjectGridScrollbar = objectGridContainer.AddElement(new UIScrollbar().With(e =>
        {
            e.HAlign = 1f;
            e.VAlign = 0.5f;
            e.Height = StyleDimension.FromPixelsAndPercent(-16f, 1f);
            ObjectGrid.SetScrollbar(e);
        }));

        FinalListContainer = Container.AddElement(new UIPanel().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-4, 0.5f);
            e.Height = StyleDimension.FromPixels(256);
            e.HAlign = 1;
            e.BorderColor = new Color(63, 82, 151);
            e.BackgroundColor = new Color(43, 62, 131) * 0.85f;
        }));

        HowToUseText = FinalListContainer.AddElement(new UIText("Select an object from the left panel. Edit any properties or reorder the list from this panel.\n\nStrike the block to get its contents in listed order (from top to bottom).").With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-16, 1);
            e.HAlign = 0.5f;
            e.VAlign = 0.5f;
            e.IsWrapped = true;
            e.WrappedTextBottomPadding = 0;
        }));

        FinalList = new UIList().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-32, 1);
            e.Height = StyleDimension.Fill;
            e.ListPadding = 8;
        });

        FinalListScrollbar = new UIScrollbar().With(e =>
        {
            e.HAlign = 1f;
            e.VAlign = 0.5f;
            e.Height = StyleDimension.FromPixelsAndPercent(-16f, 1f);
            FinalList.SetScrollbar(e);
        });

        Container.AddElement(new UIHoverImageButton(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/ButtonExit", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Exit (WILL NOT SAVE!)").With(e =>
        {
            e.VAlign = 1;
            e.OnLeftClick += Exit;
        }));

        Container.AddElement(new UIHoverImageButton(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/ButtonSubmit", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Save").With(e =>
        {
            e.HAlign = 1;
            e.VAlign = 1;
            e.OnLeftClick += Save;
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if ((modPlayer?.CapPlayer?.CanDoCapEffects ?? false) && modPlayer.currentObjectSpawnerBlockToEdit != Vector2.Zero) base.Draw(spriteBatch);
        else if (ObjectGrid?.Count > 0) ObjectGrid.Clear();
    }

    public override void Update(GameTime gameTime)
    {
        if (Container?.IsMouseHovering ?? false) Main.LocalPlayer.mouseInterface = true;

        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.currentObjectSpawnerBlockToEdit == Vector2.Zero)
        {
            if (isLoaded)
            {
                FinalList?.Clear();
                isLoaded = false;
            }
        }
        else if (!isLoaded)
        {
            FinalList?.Clear();

            if (modPlayer != null)
            {
                var contents = TerrariaXMario.GetTileEntityOrNull(modPlayer.currentObjectSpawnerBlockToEdit)?.spawnContents ?? [];

                for (int i = 0; i < contents.Length; i++)
                {
                    FinalList?.AddElement(new FinalListItem(FinalList, contents[i]));
                }
            }

            isLoaded = true;
        }

        if (ObjectGrid?.Count == 0 && TerrariaXMario.Instance.spawnableObjects != null)
        {
            for (int i = 0; i < TerrariaXMario.Instance.spawnableObjects.Length; i++)
            {
                if (i == 0)
                {
                    Main.instance.LoadItem(ItemID.GoldCoin);

                    ObjectGrid.AddElement(new UIHoverImageButton(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel"), "Gold Coin").With(e =>
                    {
                        e.SetVisibility(1, 1);
                        e.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
                        e.OnLeftClick += OnClickObjectButton;

                        e.AddElement(new UIImage(TextureAssets.Item[ItemID.GoldCoin])).With(f =>
                        {
                            f.HAlign = 0.5f;
                            f.VAlign = 0.5f;
                        });
                    }));
                }

                var obj = TerrariaXMario.Instance.spawnableObjects[i];

                ObjectGrid.AddElement(new UIHoverImageButton(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel"), obj is ModItem or ModProjectile ? Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.{(obj is ModItem ? "Items" : "Projectiles")}.{obj.GetType().Name}.DisplayName") : "").With(e =>
                {
                    e.SetVisibility(1, 1);
                    e.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder"));
                    e.OnLeftClick += OnClickObjectButton;

                    e.AddElement(new UIImage(obj is ModItem modItem ? ModContent.Request<Texture2D>(modItem.Texture) : obj is ModProjectile modProjectile ? ModContent.Request<Texture2D>(modProjectile.Texture) : TextureAssets.Item[0]).With(f =>
                    {
                        f.HAlign = 0.5f;
                        f.VAlign = 0.5f;
                    }));
                }));
            }
        }

        if (FinalList?.Count != 0 && (!FinalListContainer?.HasChild(FinalList) ?? false))
        {
            HowToUseText?.Remove();
            if (FinalList != null) FinalListContainer?.AddElement(FinalList);
            if (FinalListScrollbar != null) FinalListContainer?.AddElement(FinalListScrollbar);
        }

        if (FinalList?.Count == 0 && (FinalListContainer?.HasChild(FinalList) ?? false))
        {
            FinalList.Remove();
            FinalListScrollbar?.Remove();
            if (HowToUseText != null) FinalListContainer?.AddElement(HowToUseText);
        }
    }

    private void OnClickObjectButton(UIMouseEvent evt, UIElement listeningElement)
    {
        int index = ObjectGrid?._items.IndexOf(listeningElement) ?? -1;
        if (index == -1) return;

        if (FinalList?.Count == 0)
        {
            HowToUseText?.Remove();
            FinalListContainer?.AddElement(FinalList);
            if (FinalListScrollbar != null) FinalListContainer?.AddElement(FinalListScrollbar);
        }

        FinalList?.AddElement(new FinalListItem(FinalList, index == 0 ? null : TerrariaXMario.Instance.spawnableObjects?[index - 1]));
    }

    private void Exit(UIMouseEvent evt, UIElement listeningElement)
    {
        Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.currentObjectSpawnerBlockToEdit = Vector2.Zero;
    }

    private void Save(UIMouseEvent evt, UIElement listeningElement)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>()?.currentObjectSpawnerBlockToEdit ?? Vector2.Zero);

        entity?.spawnContents = [];

        for (int i = 0; i < FinalList?.Count; i++)
        {
            ISpawnableObject? objectType = (FinalList._items[i] as FinalListItem)?.objectType;

            entity?.spawnContents = entity?.spawnContents.Append(objectType).ToArray() ?? [];
        }

        entity?.wasPreviouslyStruck = false;

        Exit(evt, listeningElement);
    }
}
