using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.PromptShowdownUI;
internal class PromptShowdownUI : UIState
{
    private Asset<Texture2D>? promptBarTexture;

    private UIElement? ContentContainer { get; set; }
    private UIImageFramed? PromptBackground { get; set; }
    private UIImageFramed? PromptBar { get; set; }

    private int promptBarWidth = 360;

    public override void OnInitialize()
    {
        promptBarTexture = ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/PromptBar");

        ContentContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(360);
            e.Height = StyleDimension.FromPixels(80);
            e.HAlign = 0.5f;
            e.VAlign = 0.75f;
        }));

        PromptBackground = ContentContainer.AddElement(new UIImageFramed(promptBarTexture, new(0, 0, 360, 24)).With(e =>
        {
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(24);
        }));

        PromptBar = PromptBackground.AddElement(new UIImageFramed(promptBarTexture, new(0, 26, 360, 24)).With(e =>
        {
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.Fill;
        }));

        PromptBackground.AddElement(new UIText(Language.GetText($"Mods.{nameof(TerrariaXMario)}.UI.Showdown.Prompt")).With(f =>
        {
            f.HAlign = 0.5f;
            f.VAlign = 0.5f;
        }));

        ContentContainer.AddElement(new UIImageButton(ModContent.Request<Texture2D>($"{nameof(TerrariaXMario)}/Common/ObjectSpawnerBlockUI/ButtonSubmit")).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 1f;
            e.OnLeftClick += SubmitPrompt;
        }));
    }

    private void SubmitPrompt(UIMouseEvent evt, UIElement listeningElement)
    {
        ShowdownPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>();

        if (modPlayer?.showdownNPCIndex == null) return;

        modPlayer?.showdownState = ShowdownState.Active;
        SubworldSystem.Enter<ShowdownSubworld>();
        Main.npc[(int)modPlayer?.showdownNPCIndex!].GetGlobalNPCOrNull<ShowdownNPC>()?.inShowdown = true;
        promptBarWidth = 360;
        PromptBar?.SetFrame(new(0, 26, promptBarWidth, 24));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Player player = Main.LocalPlayer;
        if (player.GetModPlayerOrNull<ShowdownPlayer>()?.showdownState == ShowdownState.Queried && (player.GetModPlayerOrNull<CapEffectsPlayer>()?.CanDoCapEffects ?? false)) base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        if (ContentContainer?.IsMouseHovering ?? false) player.mouseInterface = true;

        if ((!player.GetModPlayerOrNull<CapEffectsPlayer>()?.CanDoCapEffects ?? true) || modPlayer == null || modPlayer.showdownState != ShowdownState.Queried) return;

        promptBarWidth--;
        if (promptBarWidth <= 0)
        {
            promptBarWidth = 360;
            PromptBar?.SetFrame(new(0, 26, promptBarWidth, 24));
            modPlayer.showdownState = ShowdownState.None;
            if (modPlayer.showdownNPCIndex != null)
            {
                Main.npc[(int)modPlayer.showdownNPCIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.queryShowdown = false;
                modPlayer.showdownNPCIndex = null;
            }
        }

        PromptBar?.SetFrame(new(0, 26, promptBarWidth, 24));
    }
}
