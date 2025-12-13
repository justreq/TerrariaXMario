using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.ObjectSpawnerBlockUI;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal class BroInfoUI : UIState
{
    private string BroInfoPageButtonPath => $"{GetType().Namespace!.Replace(".", "/")}/BroInfoPageButton";
    private UIHoverImageButton? BroInfoPageButton { get; set; }
    private bool showBroInfo;
    private bool showBroInfoTemporarily;

    public override void OnInitialize()
    {
        BroInfoPageButton = this.AddElement(new UIHoverImageButton(ModContent.Request<Texture2D>(BroInfoPageButtonPath), Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.BroInfoUI.PageButton")).With(e =>
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
        ToggleBroInfoPage(switchToPage: true);
    }

    internal void ToggleBroInfoPage(bool? value = null, bool switchToPage = false)
    {
        bool finalValue = ToggleBroInfoPageButton(value);
        if (switchToPage && finalValue) Main.EquipPageSelected = 0;

        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();
        if (modPlayer?.currentCap != null) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{modPlayer?.currentCap}{(finalValue ? "Equip" : "Unequip")}") { Volume = 0.4f });
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
        if (Main.playerInventory) base.Draw(spriteBatch);
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
