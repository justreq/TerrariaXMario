using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal class BroInfoUI : UIState
{
    private string BroInfoPageButtonPath => $"{GetType().Namespace!.Replace(".", "/")}/BroInfoPageButton";

    private UIImageButton? BroInfoPageButton { get; set; }
    private bool showBroInfo;
    private bool showBroInfoTemporarily;

    public override void OnInitialize()
    {
        BroInfoPageButton = this.AddElement(new UIImageButton(ModContent.Request<Texture2D>(BroInfoPageButtonPath)).With(e =>
        {
            e.Left = StyleDimension.FromPixels(TerrariaXMario.BroInfoPageButtonPosition.X);
            e.Top = StyleDimension.FromPixels(TerrariaXMario.BroInfoPageButtonPosition.Y);
            e.Width = StyleDimension.FromPixels(32);
            e.Height = StyleDimension.FromPixels(32);
            e.SetVisibility(1, 1);
            e.SetHoverImage(ModContent.Request<Texture2D>(BroInfoPageButtonPath + "Hover"));
            e.OnLeftClick += BroInfoPageButtonClick;
        }));
    }

    private void BroInfoPageButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        showBroInfoTemporarily = false;
        ToggleBroInfoPage(switchToPage: true);
    }

    internal void ToggleBroInfoPage(bool? value = null, bool switchToPage = false)
    {
        bool finalValue = ToggleBroInfoPageButton(value);
        if (switchToPage && finalValue) Main.EquipPageSelected = 0;

        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();
        if (modPlayer?.currentCap != null) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{modPlayer?.currentCap}{(finalValue ? "Equip" : "Unequip")}") { Volume = 0.4f }, Main.LocalPlayer.MountedCenter);
        modPlayer?.BroInfoPlayer?.ShowBroInfo = finalValue;
    }

    internal bool ToggleBroInfoPageButton(bool? value = null)
    {
        if (value == null) showBroInfo ^= true;
        else showBroInfo = (bool)value;

        string path = $"{BroInfoPageButtonPath}{(showBroInfo ? "Active" : "")}";
        BroInfoPageButton?.SetImage(ModContent.Request<Texture2D>(path));
        BroInfoPageButton?.SetHoverImage(ModContent.Request<Texture2D>($"{path}Hover"));
        BroInfoPageButton?.Width = StyleDimension.FromPixels(32);
        BroInfoPageButton?.Height = StyleDimension.FromPixels(32);
        BroInfoPageButton?.Recalculate();

        return showBroInfo;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.playerInventory)
        {
            base.Draw(spriteBatch);
            if (BroInfoPageButton?.IsMouseHovering ?? false) Main.hoverItemName = Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.BroInfoUI.PageButton");

            if (Main.EquipPage == 0 && showBroInfo) // copied from Main.DrawDefenseCounter()
            {
                Texture2D texture = TextureAssets.Extra[ExtrasID.DefenseShield].Value;
                Vector2 position = AccessorySlotLoader.DefenseIconPosition + new Vector2(48, -340);
                Player player = Main.LocalPlayer;
                float scale = Main.inventoryScale * 1.42f;

                Vector2 vector = new(position.X - 10 - 47 - 47 - 14, (float)position.Y + (float)TextureAssets.InventoryBack.Height() * 0.5f);
                spriteBatch.Draw(texture, vector, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
                Vector2 vector2 = FontAssets.MouseText.Value.MeasureString(player.statDefense.ToString());
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, player.statDefense.ToString(), vector - vector2 * 0.5f * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale));
                if (Utils.CenteredRectangle(vector, texture.Size()).Contains(new Point(Main.mouseX, Main.mouseY)) && !PlayerInput.IgnoreMouseInterface)
                {
                    player.mouseInterface = true;
                    Player.DefenseStat statDefense = player.statDefense;
                    string value = statDefense.ToString() + " " + Lang.inter[10].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        Main.hoverItemName = value;
                    }
                }
                UILinkPointNavigator.SetPosition(1557, vector + texture.Size() * scale / 4f);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Main.LocalPlayer.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo != showBroInfo) ToggleBroInfoPageButton();

        BroInfoPageButton?.Left = StyleDimension.FromPixels(TerrariaXMario.BroInfoPageButtonPosition.X);
        BroInfoPageButton?.Top = StyleDimension.FromPixels(TerrariaXMario.BroInfoPageButtonPosition.Y);

        if (BroInfoPageButton?.IsMouseHovering ?? false) Main.LocalPlayer.mouseInterface = true;

        if (!Main.mouseItem.IsAir && Main.mouseItem.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType != GearType.None && !showBroInfo)
        {
            ToggleBroInfoPage(true, true);
            showBroInfoTemporarily = true;
        }

        if (showBroInfoTemporarily && Main.mouseItem.IsAir)
        {
            ToggleBroInfoPage(false, true);
            showBroInfoTemporarily = false;
        }
    }
}
