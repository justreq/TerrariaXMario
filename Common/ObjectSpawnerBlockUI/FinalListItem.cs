using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
/// <summary>
/// List item for Object Spawner Blocks<br></br>Set objectType as <see cref="DefaultSpawnableObject"/> for Gold Coin
/// </summary>
/// <param name="objectType"></param>
internal class FinalListItem : UIPanel
{
    internal readonly ISpawnableObject objectType;
    private readonly UIList parentList;
    internal int position;

    private UIHoverImageButton? ShiftUpButton { get; set; }
    private UIHoverImageButton? ShiftDownButton { get; set; }

    internal FinalListItem(UIList parentList, ISpawnableObject objectType) : base(ModContent.Request<Texture2D>($"{TerrariaXMario.Textures}/AlternativePanelBackground"), ModContent.Request<Texture2D>($"{TerrariaXMario.Textures}/AlternativePanelBorder"))
    {
        this.parentList = parentList;
        this.objectType = objectType;
        position = parentList.Count;

        BorderColor = new Color(83, 102, 171);
        BackgroundColor = new Color(63, 82, 151) * 0.7f;
        Width = StyleDimension.Fill;
        Height = StyleDimension.FromPixels(52);
        PaddingBottom = PaddingTop = 0;

        string assetPath = GetType().Namespace!.Replace(".", "/");

        if (objectType is DefaultSpawnableObject) Main.instance.LoadItem(ItemID.GoldCoin);

        if (objectType is ModProjectile modProjectile)
        {
            this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>(modProjectile.Texture), new Rectangle(0, 0, modProjectile.Projectile.width, modProjectile.Projectile.height)).With(e =>
            {
                e.VAlign = 0.5f;
            }));
        }
        else if (objectType is ModItem modItem)
        {
            this.AddElement(new UIImage(ModContent.Request<Texture2D>(modItem.Texture)).With(e =>
            {
                e.VAlign = 0.5f;
            }));
        }
        else
        {
            this.AddElement(new UIImage(TextureAssets.Item[ItemID.GoldCoin]).With(e =>
            {
                e.VAlign = 0.5f;
            }));
        }

        foreach (string direction in new string[] { "Up", "Down" })
        {
            UIHoverImageButton button = new UIHoverImageButton(ModContent.Request<Texture2D>($"{assetPath}/ListItemShift{direction}", ReLogic.Content.AssetRequestMode.ImmediateLoad), $"Shift {direction}").With(e =>
            {
                e.Left = StyleDimension.FromPixels(-34);
                e.Top = StyleDimension.FromPixels(11 * (direction == "Up" ? -1 : 1));
                e.HAlign = 1;
                e.VAlign = 0.5f;
                e.SetVisibility(1, 1);
                e.SetHoverImage(ModContent.Request<Texture2D>($"{assetPath}/ListItemButtonBorder"));
                e.OnLeftClick += (evt, listeningElement) =>
                {
                    SwitchPosition((FinalListItem?)parentList._items.Find(e => e is FinalListItem item && item.position == position + (direction == "Up" ? -1 : 1)));
                };
            });

            if (direction == "Up") ShiftUpButton = button;
            else ShiftDownButton = button;
        }

        this.AddElement(new UIHoverImageButton(ModContent.Request<Texture2D>($"{assetPath}/ListItemDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Delete").With(e =>
        {
            e.HAlign = 1;
            e.VAlign = 0.5f;
            e.SetVisibility(1, 1);
            e.SetHoverImage(ModContent.Request<Texture2D>($"{assetPath}/ListItemButtonBorder"));
            e.OnLeftClick += (evt, listeningElement) =>
            {
                foreach (FinalListItem item in parentList._items.Cast<FinalListItem>())
                {
                    if (item.position > position) item.position--;
                }

                parentList.Remove(this);
            };
        }));
    }

    public override int CompareTo(object obj)
    {
        return position.CompareTo(((FinalListItem)obj).position);
    }

    protected override void DrawChildren(SpriteBatch spriteBatch)
    {
        if (position == 0)
        {
            if (parentList.Count > 1)
            {
                if (!HasChild(ShiftDownButton) && ShiftDownButton != null) this.AddElement(ShiftDownButton);
            }
            else if (HasChild(ShiftDownButton)) ShiftDownButton?.Remove();
            if (HasChild(ShiftUpButton)) ShiftUpButton?.Remove();
        }
        else if (position == parentList.Count - 1)
        {
            if (parentList.Count > 1)
            {
                if (!HasChild(ShiftUpButton) && ShiftUpButton != null) this.AddElement(ShiftUpButton);
            }
            else if (HasChild(ShiftUpButton)) ShiftUpButton?.Remove();
            if (HasChild(ShiftDownButton)) ShiftDownButton?.Remove();
        }
        else
        {
            if (!HasChild(ShiftUpButton) && ShiftUpButton != null) this.AddElement(ShiftUpButton);
            if (!HasChild(ShiftDownButton) && ShiftDownButton != null) this.AddElement(ShiftDownButton);
        }

        base.DrawChildren(spriteBatch);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (IsMouseHovering) Main.hoverItemName = objectType.Name;
    }

    private void SwitchPosition(FinalListItem? otherItem)
    {
        if (otherItem == null) return;

        (position, otherItem.position) = (otherItem.position, position);
        parentList.UpdateOrder();
    }
}
