using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.PowerupHUD;

internal class PowerupHUD : UIState
{
    private UIElement? OverchargeBarContainer { get; set; }
    private UIElement? AbilityContainer { get; set; }

    private string? path;
    private Asset<Texture2D>? OverchargeBarTexture => path == null ? null : ModContent.Request<Texture2D>($"{path}/OverchargeBar");
    private Asset<Texture2D>? OverchargeFillTexture => path == null ? null : ModContent.Request<Texture2D>($"{path}/OverchargeBarFill");

    private int abilityCount;

    public override void OnInitialize()
    {
        path = $"{GetType().Namespace!.Replace(".", "/")}";

        OverchargeBarContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(64);
            e.Height = StyleDimension.FromPixels(52);
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(8);
        }));

        AbilityContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Height = StyleDimension.FromPixels(44);
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(68);
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();
        if (modPlayer != null && modPlayer.CanDoCapEffects && modPlayer.currentPowerupType != null)
        {
            base.Draw(spriteBatch);

            if (OverchargeBarTexture == null || OverchargeFillTexture == null || OverchargeBarContainer == null) return;

            Vector2 position = OverchargeBarContainer.GetDimensions().Position();
            Color color = modPlayer.CurrentPowerup!.Color;

            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X, (int)position.Y, 64, 52), new Rectangle(0, 0, 64, 52), Color.White);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y, modPlayer.powerupChargeMax, 52), new Rectangle(66, 0, 2, 52), Color.White);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.powerupChargeMax, (int)position.Y, 16, 52), new Rectangle(70, 0, 16, 52), Color.White);

            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X, (int)position.Y, 64, 52), new Rectangle(0, 54, 64, 52), color);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y, modPlayer.powerupChargeMax, 52), new Rectangle(66, 54, 2, 52), color);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.powerupChargeMax, (int)position.Y, 16, 52), new Rectangle(70, 54, 16, 52), color);

            if (ModContent.TryFind(nameof(TerrariaXMario), modPlayer.CurrentPowerup.Name + "Projectile", out ModProjectile projectile))
            {
                if (OverchargeBarContainer.IsMouseHovering) Main.hoverItemName = $"{projectile.DisplayName}\nCharge: {modPlayer.PowerupCharge}/{modPlayer.powerupChargeMax}";

                spriteBatch.Draw(ModContent.Request<Texture2D>(projectile.Texture).Value, new Rectangle((int)position.X + 18, (int)position.Y + 14, 24, 24), new Rectangle(0, 0, projectile.Projectile.width, projectile.Projectile.height), Color.White);

                if (modPlayer.PowerupCharge > 0)
                {
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 52, (int)position.Y + 20, 2, 12), new Rectangle(0, 0, 2, 12), color);
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y + 20, modPlayer.PowerupCharge, 12), new Rectangle(2, 0, 2, 12), color);
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.PowerupCharge, (int)position.Y + 20, 2, 12), new Rectangle(4, 0, 2, 12), color);
                }
            }

            if (AbilityContainer == null) return;

            Vector2 position2 = AbilityContainer.GetDimensions().Position();

            for (int i = 0; i < int.MaxValue; i++)
            {
                string key = $"Mods.{nameof(TerrariaXMario)}.Projectiles.{projectile.Name}.Abilities.{i}";
                LocalizedText ability = Language.GetText(key);
                string name = Language.GetTextValue(key + ".Name");

                if (name == key + ".Name")
                {
                    abilityCount = i;
                    break;
                }

                string text = Language.GetTextValue(key + ".Text");

                Vector2 abilityPosition = position2 + new Vector2((44 * i) + (i == 0 ? 0 : i * 8), 0);
                spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/Ability").Value, abilityPosition, Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/Ability{name}").Value, abilityPosition, Color.White);

                if (new Rectangle((int)abilityPosition.X, (int)abilityPosition.Y, 44, 44).Contains(Main.MouseScreen.ToPoint())) Main.hoverItemName = text;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || !modPlayer.CanDoCapEffects || modPlayer.currentPowerupType == null)
        {
            modPlayer?.PowerupCharge = modPlayer?.powerupChargeMax ?? 0;
            return;
        }

        if (modPlayer.PowerupCharge == 0) modPlayer?.shouldRemovePowerup = true;
        else modPlayer?.PowerupCharge += 1;

        if (OverchargeBarContainer == null || AbilityContainer == null) return;

        OverchargeBarContainer.Width = StyleDimension.FromPixels(70 + modPlayer.powerupChargeMax);
        AbilityContainer.Width = StyleDimension.FromPixels(abilityCount * 44 + (abilityCount - 1) * 8);
        AbilityContainer.Recalculate();
    }
}
