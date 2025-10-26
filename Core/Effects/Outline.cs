using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Utilities;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Core.Effects;
internal class Outline : ILoadable // credits to math2 for all this shader stuff
{
    internal static RenderTarget2D? NPCTarget;

    internal static bool outlineNeeded;

    public void Load(Mod mod)
    {
        if (Main.dedServ) return;

        ResizeTarget();

        Main.OnPreDraw += On_PreDraw;
        On_Main.DrawNPCs += DrawOutline;
    }

    private void DrawOutline(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        orig(self, behindTiles);

        if (outlineNeeded) DrawNPCTarget();
    }

    internal static void ResizeTarget()
    {
        Threading.RunOnMainThread(() => NPCTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));
    }

    public void Unload()
    {
        Main.OnPreDraw -= On_PreDraw;
        On_Main.DrawNPCs -= DrawOutline;
    }

    private void On_PreDraw(GameTime obj)
    {
        var graphicsDevice = Main.graphics.GraphicsDevice;
        var spriteBatch = Main.spriteBatch;

        if (Main.gameMenu || Main.dedServ || spriteBatch is null || NPCTarget is null || graphicsDevice is null) return;

        if (!outlineNeeded) return;

        var bindings = graphicsDevice.GetRenderTargets();
        graphicsDevice.SetRenderTarget(NPCTarget);
        graphicsDevice.Clear(Color.Transparent);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

        for (int i = 0; i < Main.npc.Length; i++)
        {
            NPC NPC = Main.npc[i];
            ShowdownNPC? globalNPC = NPC.GetGlobalNPCOrNull<ShowdownNPC>();

            if (NPC.active && globalNPC != null && globalNPC.showdownState != NPCShowdownState.None && !globalNPC.isCopyOfShowdownNPC) Main.instance.DrawNPC(i, false);
        }

        spriteBatch.End();
        graphicsDevice.SetRenderTargets(bindings);
    }

    private static void DrawNPCTarget()
    {
        var graphicsDevice = Main.graphics.GraphicsDevice;
        var spriteBatch = Main.spriteBatch;

        if (Main.dedServ || spriteBatch == null || NPCTarget == null || graphicsDevice == null)
            return;

        var shader = ModContent.Request<Effect>($"{nameof(TerrariaXMario)}/Core/Effects/Outline", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        shader.Parameters["uColor"].SetValue(Main.OurFavoriteColor.ToVector4());
        shader.Parameters["uSize"].SetValue(new Vector2(NPCTarget.Width / 2, NPCTarget.Height / 2));
        shader.Parameters["uThreshold"].SetValue(0.4f);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, shader, Main.UIScaleMatrix);
        shader.CurrentTechnique.Passes[0].Apply();
        spriteBatch.Draw(NPCTarget, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        spriteBatch.End();
        spriteBatch.Begin();
    }
}