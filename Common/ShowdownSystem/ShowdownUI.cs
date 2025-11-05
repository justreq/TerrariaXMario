using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Common.ShowdownActions;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownUI : UIState
{
    private UIKeypressIndicator? CancelIndicator { get; set; }
    private UIText? TargetSelectText { get; set; }
    private UIImageFramed? TargetArrow { get; set; }

    private int frame;
    private int frameRate;
    private bool isChoosingTarget;

    public override void OnInitialize()
    {
        CancelIndicator = this.AddElement(new UIKeypressIndicator().With(e =>
        {
            e.Left = StyleDimension.FromPixels(16);
            e.Top = StyleDimension.FromPixels(16);
        }));

        TargetSelectText = this.AddElement(new UIText(Language.GetText($"Mods.{nameof(TerrariaXMario)}.Showdown.UI.TargetSelect"), large: true).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 0.15f;
            e.TextColor = Color.Transparent;
        }));

        TargetArrow = new UIImageFramed(ModContent.Request<Texture2D>($"{GetType().Namespace!.Replace(".", "/")}/TargetArrow"), new Rectangle(0, 0, 100, 88)).With(e =>
        {
            e.Width = StyleDimension.FromPixels(100);
            e.Height = StyleDimension.FromPixels(88);
        });
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false) base.Draw(spriteBatch);

        frameRate++;

        if (frameRate >= 4)
        {
            frame = (frame + 1) % 10;
            TargetArrow?.SetFrame(new(0, 88 * frame + 2 * frame, 100, 88));
            frameRate = 0;
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        CancelIndicator?.SetKey($"{KeybindSystem.KeybindSystem.GetVanillaKeybindKey(TriggerNames.Inventory)} ({Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.Showdown.UI.Cancel")})");

        if (!modPlayer?.isPlayerInShowdownSubworld ?? true) return;

        CancelIndicator?.canDraw = modPlayer?.queriedAction != null;

        if (modPlayer?.queriedAction is ActionJump)
        {
            if (!isChoosingTarget)
            {
                NPC npc = Main.npc[(int)modPlayer.showdownNPCPuppetIndex!];

                TargetArrow?.Left = StyleDimension.FromPixels(npc.Center.X - Main.screenPosition.X + Main.screenWidth / Main.GameZoomTarget * 0.25f - (npc.frame.Width * npc.scale) - 100);
                TargetArrow?.Top = StyleDimension.FromPixels(npc.Center.Y - Main.screenPosition.Y + Main.screenHeight / Main.GameZoomTarget * 0.15f - npc.frame.Height * npc.scale * 0.5f);
                Append(TargetArrow);
                isChoosingTarget = true;
            }

            TargetSelectText?.TextColor = Color.White;
        }
        else
        {
            if (isChoosingTarget)
            {
                TargetArrow?.Remove();
                isChoosingTarget = false;
            }

            TargetSelectText?.TextColor = Color.Transparent;
        }
    }
}
