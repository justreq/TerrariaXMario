using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.SPBar;

internal class SPBar : UIState
{
    private UIElement? Container { get; set; }
    private SPResource? ResourceSingle { get; set; }
    private SPResource? ResourceTop { get; set; }
    private SPResource? ResourceMiddle1 { get; set; }
    private SPResource? ResourceMiddle2 { get; set; }
    private SPResource? ResourceMiddle3 { get; set; }
    private SPResource? ResourceBottom { get; set; }
    private UIImage? ResourceTopModern { get; set; }
    private SPResource? ResourceMiddle1Modern { get; set; }
    private SPResource? ResourceMiddle2Modern { get; set; }
    private SPResource? ResourceMiddle3Modern { get; set; }
    private SPResource? ResourceMiddle4Modern { get; set; }
    private SPResource? ResourceMiddle5Modern { get; set; }
    private UIImage? ResourceBottomModern { get; set; }

    public override void OnInitialize()
    {
        string path = GetType().Namespace!.Replace(".", "/");

        Container = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixels(40);
            e.HAlign = 1;
            e.Left = StyleDimension.FromPixels(-48);
            e.Top = StyleDimension.FromPixels(84);
        }));

        ResourceSingle = Container.AddElement(new SPResource("Single", 0, 20).With(e =>
        {
            e.HAlign = 0.5f;
        }));

        ResourceTop = Container.AddElement(new SPResource("Top", 0, 20).With(e =>
        {
            e.HAlign = 0.5f;
        }));

        ResourceMiddle1 = Container.AddElement(new SPResource("Middle", 20, 40).With(e =>
        {
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(34);
        }));

        ResourceMiddle2 = Container.AddElement(new SPResource("Middle", 40, 60).With(e =>
        {
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(66);
        }));

        ResourceMiddle3 = Container.AddElement(new SPResource("Middle", 60, 80).With(e =>
        {
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(98);
        }));

        ResourceBottom = Container.AddElement(new SPResource("Bottom", 80, 100).With(e =>
        {
            e.HAlign = 0.5f;
            e.VAlign = 1;
        }));

        ResourceMiddle1Modern = Container.AddElement(new SPResource("MiddleModern", 0, 20).With(e =>
        {
            e.Top = StyleDimension.FromPixels(38);
            e.Left = StyleDimension.FromPixels(6);
        }));

        ResourceMiddle2Modern = Container.AddElement(new SPResource("MiddleModern", 20, 40).With(e =>
        {
            e.Top = StyleDimension.FromPixels(50);
            e.Left = StyleDimension.FromPixels(6);
        }));

        ResourceMiddle3Modern = Container.AddElement(new SPResource("MiddleModern", 40, 60).With(e =>
        {
            e.Top = StyleDimension.FromPixels(62);
            e.Left = StyleDimension.FromPixels(6);
        }));

        ResourceMiddle4Modern = Container.AddElement(new SPResource("MiddleModern", 60, 80).With(e =>
        {
            e.Top = StyleDimension.FromPixels(74);
            e.Left = StyleDimension.FromPixels(6);
        }));

        ResourceMiddle5Modern = Container.AddElement(new SPResource("MiddleModern", 80, 100).With(e =>
        {
            e.Top = StyleDimension.FromPixels(86);
            e.Left = StyleDimension.FromPixels(6);
        }));

        ResourceTopModern = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{path}/SPTopModern")).With(e =>
        {
            e.HAlign = 0.5f;
            e.Width = StyleDimension.FromPixels(40);
            e.Height = StyleDimension.FromPixels(54);
        }));

        ResourceBottomModern = Container.AddElement(new UIImage(ModContent.Request<Texture2D>($"{path}/SPBottomModern")).With(e =>
        {
            e.Width = StyleDimension.FromPixels(24);
            e.Height = StyleDimension.FromPixels(6);
            e.Left = StyleDimension.FromPixels(6);
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

    internal void ChangeResourceSet(string name)
    {
        if (Container == null) return;

        bool bars = name.Contains("Bars");

        Container.Left = StyleDimension.FromPixels(bars ? -3 : -48);
        Container.Top = StyleDimension.FromPixels(bars ? 30 + (int.TryParse(name.Split(" ").Last(), result: out int variant) ? variant switch { 1 => 0, 2 => 4, 3 => 2, _ => 0 } : 0) : 84);
        Container.Recalculate();

        if (bars)
        {
            if (!Container.HasChild(ResourceTopModern)) Container.Append(ResourceTopModern);
            if (!Container.HasChild(ResourceBottomModern)) Container.Append(ResourceBottomModern);
        }
        else
        {
            if (Container.HasChild(ResourceTopModern)) ResourceTopModern?.Remove();
            if (Container.HasChild(ResourceBottomModern)) ResourceBottomModern?.Remove();
        }
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        bool bars = TerrariaXMario.ResourceBarStyle.Contains("Bars");

        if (modPlayer == null || Container == null) return;

        ChangeResourceSet(TerrariaXMario.ResourceBarStyle);
        ResourceBottomModern?.Top = StyleDimension.FromPixels(38 + (modPlayer.maxSP + 19) / 20 * 12);

        if (modPlayer.maxSP <= 20)
        {
            Container.Height = StyleDimension.FromPixels(bars ? 56 : 38);
            if (!Container.HasChild(ResourceSingle)) Container.Append(ResourceSingle);
            if (Container.HasChild(ResourceTop)) ResourceTop?.Remove();
            if (Container.HasChild(ResourceMiddle1)) ResourceMiddle1?.Remove();
            if (Container.HasChild(ResourceMiddle2)) ResourceMiddle2?.Remove();
            if (Container.HasChild(ResourceMiddle3)) ResourceMiddle3?.Remove();
            if (!Container.HasChild(ResourceMiddle1Modern)) Container.Append(ResourceMiddle1Modern);
            if (Container.HasChild(ResourceMiddle2Modern)) ResourceMiddle2Modern?.Remove();
            if (Container.HasChild(ResourceMiddle3Modern)) ResourceMiddle3Modern?.Remove();
            if (Container.HasChild(ResourceMiddle4Modern)) ResourceMiddle4Modern?.Remove();
            if (Container.HasChild(ResourceMiddle5Modern)) ResourceMiddle5Modern?.Remove();
            if (Container.HasChild(ResourceBottom)) ResourceBottom?.Remove();
        }
        else
        {
            ResourceBottom?.SetRange(modPlayer.maxSP - 20, modPlayer.maxSP);
            Container.Height = StyleDimension.FromPixels(bars ? ResourceBottomModern?.Top.Pixels + 6 ?? 0 : (70 + (modPlayer.maxSP - 40) / 20 * 32));
            if (Container.HasChild(ResourceSingle)) ResourceSingle?.Remove();
            if (!Container.HasChild(ResourceTop)) Container.Append(ResourceTop);
            if (!Container.HasChild(ResourceBottom)) Container.Append(ResourceBottom);
            if (!Container.HasChild(ResourceMiddle1Modern)) Container.Append(ResourceMiddle1Modern);

            if (modPlayer.maxSP > 20)
            {
                if (!Container.HasChild(ResourceMiddle2Modern)) Container.Append(ResourceMiddle2Modern);
            }
            else if (Container.HasChild(ResourceMiddle2Modern)) ResourceMiddle2Modern?.Remove();

            if (modPlayer.maxSP > 40)
            {
                if (!Container.HasChild(ResourceMiddle1)) Container.Append(ResourceMiddle1);
                if (!Container.HasChild(ResourceMiddle3Modern)) Container.Append(ResourceMiddle3Modern);
            }
            else
            {
                if (Container.HasChild(ResourceMiddle1)) ResourceMiddle1?.Remove();
                if (Container.HasChild(ResourceMiddle3Modern)) ResourceMiddle3Modern?.Remove();
            }

            if (modPlayer.maxSP > 60)
            {
                if (!Container.HasChild(ResourceMiddle2)) Container.Append(ResourceMiddle2);
                if (!Container.HasChild(ResourceMiddle4Modern)) Container.Append(ResourceMiddle4Modern);
            }
            else
            {
                if (Container.HasChild(ResourceMiddle2)) ResourceMiddle2?.Remove();
                if (Container.HasChild(ResourceMiddle4Modern)) ResourceMiddle4Modern?.Remove();
            }

            if (modPlayer.maxSP > 80)
            {
                if (!Container.HasChild(ResourceMiddle3)) Container.Append(ResourceMiddle3);
                if (!Container.HasChild(ResourceMiddle5Modern)) Container.Append(ResourceMiddle5Modern);
            }
            else
            {
                if (Container.HasChild(ResourceMiddle3)) ResourceMiddle3?.Remove();
                if (Container.HasChild(ResourceMiddle5Modern)) ResourceMiddle5Modern?.Remove();
            }
        }

        if (!modPlayer.CanDoCapEffects) return;
    }
}
