using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Reflection.Emit;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.MiscEffects;

internal class Patch_AdjustArmPositionForFrogSuit : BasePatch
{
    internal override void Patch(Mod mod)
    {
        IL_PlayerDrawLayers.DrawPlayer_27_HeldItem += IL_PlayerDrawLayers_DrawPlayer_27_HeldItem;
        IL_Main.GetPlayerArmPosition += IL_Main_GetPlayerArmPosition;
    }

    private void IL_PlayerDrawLayers_DrawPlayer_27_HeldItem(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(MoveType.After, i => i.MatchCall<Vector2>(".ctor"))) ThrowError("Call");

        c.EmitLdarg0();
        c.EmitLdfld(typeof(PlayerDrawSet).GetField("drawPlayer")!);
        c.EmitLdloca(4);
        c.EmitDelegate((Player player, ref Vector2 position) =>
        {
            position.Y += player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentPowerupType == ModContent.GetInstance<FrogSuitData>().Type ? 10 : 0;
        });
    }

    private void IL_Main_GetPlayerArmPosition(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();

        if (!c.TryGotoNext(MoveType.After, i => i.MatchStloc2())) ThrowError("Stloc2");

        c.EmitLdloc0();
        c.EmitLdloca(2);
        c.EmitDelegate((Player player, ref Vector2 position) =>
        {
            position.Y += player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentPowerupType == ModContent.GetInstance<FrogSuitData>().Type ? 10 : 0;
        });
    }
}
