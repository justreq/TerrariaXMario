using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.SPBar;

internal class SPBar : UIState
{
    private string TexturePath => GetType().Namespace!.Replace(".", "/");
    private string ResourceBarStyle => Main.ResourceSetsManager.ActiveSet.DisplayedName;

    private UIElement? Container { get; set; }
    private UIImage? ResourceSingle { get; set; }
    private UIImage? ResourceTop { get; set; }
    private UIImage? ResourceMiddle1 { get; set; }
    private UIImage? ResourceMiddle2 { get; set; }
    private UIImage? ResourceMiddle3 { get; set; }
    private UIImage? ResourceBottom { get; set; }
    private UIImage? ResourceFillSingle { get; set; }
    private UIImage? ResourceFillTop { get; set; }
    private UIImage? ResourceFillMiddle1 { get; set; }
    private UIImage? ResourceFillMiddle2 { get; set; }
    private UIImage? ResourceFillMiddle3 { get; set; }
    private UIImage? ResourceFillBottom { get; set; }

    public override void OnInitialize()
    {

        Container = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(40);
            e.HAlign = 1;
            e.Left = StyleDimension.FromPixels(-48);
            e.Top = StyleDimension.FromPixels(78);
        }));

        ResourceSingle = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPSingle")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(38);

            ResourceFillSingle = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));

        ResourceTop = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPTop")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(34);

            ResourceFillTop = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Top = StyleDimension.FromPixels(2);
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));

        ResourceMiddle1 = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPMiddle")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(32);
            e.Top = StyleDimension.FromPixels(34);

            ResourceFillMiddle1 = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Top = StyleDimension.FromPixels(1);
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));

        ResourceMiddle2 = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPMiddle")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(32);
            e.Top = StyleDimension.FromPixels(66);

            ResourceFillMiddle2 = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Top = StyleDimension.FromPixels(1);
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));

        ResourceMiddle3 = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPMiddle")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(32);
            e.Top = StyleDimension.FromPixels(98);

            ResourceFillMiddle3 = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Top = StyleDimension.FromPixels(1);
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));

        ResourceBottom = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPBottom")).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 1;
            e.Width = StyleDimension.Fill;
            e.Height = StyleDimension.FromPixels(36);

            ResourceFillBottom = e.AddElement(new UIImage(ModContent.Request<Texture2D>($"{TexturePath}/SPFill")).With(f =>
            {
                f.VAlign = 0.5f;
                f.HAlign = 0.5f;
                f.Width = StyleDimension.FromPixels(32);
                f.Height = StyleDimension.FromPixels(30);
            }));
        }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer?.CanDoCapEffects ?? false)
        {
            base.Draw(spriteBatch);
            if (Container?.IsMouseHovering ?? false) Main.hoverItemName = $"{modPlayer.StatSP}/{modPlayer.maxSP}";
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        if (modPlayer == null || Container == null) return;

        if (modPlayer.maxSP <= 20)
        {
            Container.Height = StyleDimension.FromPixels(38);
            if (!Container.HasChild(ResourceSingle)) Container.Append(ResourceSingle);
            if (Container.HasChild(ResourceTop)) ResourceTop?.Remove();
            if (Container.HasChild(ResourceMiddle1)) ResourceMiddle1?.Remove();
            if (Container.HasChild(ResourceMiddle2)) ResourceMiddle2?.Remove();
            if (Container.HasChild(ResourceMiddle3)) ResourceMiddle3?.Remove();
            if (Container.HasChild(ResourceBottom)) ResourceBottom?.Remove();
        }
        else
        {
            Container.Height = StyleDimension.FromPixels(70 + (modPlayer.maxSP - 40) / 20 * 32);
            if (Container.HasChild(ResourceSingle)) ResourceSingle?.Remove();
            if (!Container.HasChild(ResourceTop)) Container.Append(ResourceTop);
            if (!Container.HasChild(ResourceBottom)) Container.Append(ResourceBottom);

            if (modPlayer.maxSP > 40)
            {
                if (!Container.HasChild(ResourceMiddle1)) Container.Append(ResourceMiddle1);
            }
            else if (Container.HasChild(ResourceMiddle1)) ResourceMiddle1?.Remove();

            if (modPlayer.maxSP > 60)
            {
                if (!Container.HasChild(ResourceMiddle2)) Container.Append(ResourceMiddle2);
            }
            else if (Container.HasChild(ResourceMiddle2)) ResourceMiddle2?.Remove();

            if (modPlayer.maxSP > 80)
            {
                if (!Container.HasChild(ResourceMiddle3)) Container.Append(ResourceMiddle3);
            }
            else if (Container.HasChild(ResourceMiddle3)) ResourceMiddle3?.Remove();
        }

        ResourceFillSingle?.ImageScale = Math.Clamp(modPlayer.StatSP / 20f, 0f, 1f);
        ResourceFillTop?.ImageScale = Math.Clamp(modPlayer.StatSP / 20f, 0f, 1f);
        ResourceFillMiddle1?.ImageScale = Math.Clamp((modPlayer.StatSP - 20) / 20f, 0f, 1f);
        ResourceFillMiddle2?.ImageScale = Math.Clamp((modPlayer.StatSP - 40) / 20f, 0f, 1f);
        ResourceFillMiddle3?.ImageScale = Math.Clamp((modPlayer.StatSP - 60) / 20f, 0f, 1f);
        ResourceFillBottom?.ImageScale = Math.Clamp((modPlayer.StatSP - (modPlayer.maxSP - 20)) / 20f, 0f, 1f);

        if (!modPlayer.CanDoCapEffects) return;
        modPlayer.maxSP = 100;
        modPlayer.StatSP = 70;
    }
}
