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
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y, modPlayer.powerupOverchargeMax, 52), new Rectangle(66, 0, 2, 52), Color.White);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.powerupOverchargeMax, (int)position.Y, 16, 52), new Rectangle(70, 0, 16, 52), Color.White);

            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X, (int)position.Y, 64, 52), new Rectangle(0, 54, 64, 52), color);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y, modPlayer.powerupOverchargeMax, 52), new Rectangle(66, 54, 2, 52), color);
            spriteBatch.Draw(OverchargeBarTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.powerupOverchargeMax, (int)position.Y, 16, 52), new Rectangle(70, 54, 16, 52), color);

            if (ModContent.TryFind(nameof(TerrariaXMario), modPlayer.CurrentPowerup.Name.Replace("Data", ""), out ModProjectile projectile))
            {
                if (OverchargeBarContainer.IsMouseHovering) Main.hoverItemName = $"{projectile.PrettyPrintName()}\nCharge: {modPlayer.PowerupOvercharge}/{modPlayer.powerupOverchargeMax}";

                spriteBatch.Draw(ModContent.Request<Texture2D>(projectile.Texture).Value, new Rectangle((int)position.X + 18, (int)position.Y + 14, 24, 24), new Rectangle(0, 0, projectile.Projectile.width, projectile.Projectile.height), Color.White);

                if (modPlayer.PowerupOvercharge > 0)
                {
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 52, (int)position.Y + 20, 2, 12), new Rectangle(0, 0, 2, 12), color);
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 54, (int)position.Y + 20, modPlayer.PowerupOvercharge, 12), new Rectangle(2, 0, 2, 12), color);
                    spriteBatch.Draw(OverchargeFillTexture.Value, new Rectangle((int)position.X + 54 + modPlayer.PowerupOvercharge, (int)position.Y + 20, 2, 12), new Rectangle(4, 0, 2, 12), color);
                }
            }

            if (AbilityContainer == null) return;

            Vector2 position2 = AbilityContainer.GetDimensions().Position();

            for (int i = 0; i < modPlayer.CurrentPowerup.Abilities.Keys.Count; i++)
            {
                PowerupAbility ability = modPlayer.CurrentPowerup.Abilities.Keys.ToArray()[i];

                Vector2 abilityPosition = position2 + new Vector2((44 * i) + (i == 0 ? 0 : i * 8), 0);
                spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/Ability").Value, abilityPosition, Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/Ability{ability}").Value, abilityPosition, Color.White);

                if (new Rectangle((int)abilityPosition.X, (int)abilityPosition.Y, 44, 44).Contains(Main.MouseScreen.ToPoint())) Main.hoverItemName = modPlayer.CurrentPowerup.Abilities.Values.ToArray()[i];
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || !modPlayer.CanDoCapEffects || modPlayer.currentPowerupType == null)
        {
            modPlayer?.PowerupOvercharge = modPlayer?.powerupOverchargeMax ?? 0;
            return;
        }

        modPlayer.PowerupOvercharge++;

        if (modPlayer.PowerupOvercharge == 0) modPlayer.RemovePowerup();

        if (OverchargeBarContainer == null || AbilityContainer == null) return;

        OverchargeBarContainer.Width = StyleDimension.FromPixels(70 + modPlayer.powerupOverchargeMax);
        int num = modPlayer.CurrentPowerup!.Abilities.Count;
        AbilityContainer.Width = StyleDimension.FromPixels(num * 44 + (num - 1) * 8);
        AbilityContainer.Recalculate();
    }
}
