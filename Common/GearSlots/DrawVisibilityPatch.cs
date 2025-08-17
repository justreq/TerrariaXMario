using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal sealed class DrawVisibilityPatch : BasePatch
{
    private static bool ShowGearSlots => Main.LocalPlayer.GetModPlayerOrNull<GearSlotPlayer>()?.showGearSlots ?? false;

    internal override void Patch(Mod mod)
    {
        // Prevents gear slot visibility buttons from drawing
        MonoModHooks.Add(typeof(AccessorySlotLoader).GetMethod("DrawVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), On_DrawVisibility);
        MonoModHooks.Modify(typeof(AccessorySlotLoader).GetMethod("DrawVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), DrawVisibility);
    }

    private void DrawVisibility(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel target = c.DefineLabel();

        if (!c.TryGotoNext(MoveType.After, i => i.MatchCallOrCallvirt(typeof(Rectangle).GetMethod("Contains", [typeof(Point)])!), i => i.MatchBrfalse(out target!))) ThrowError("CallOrCallvirt, Brfalse");

        c.EmitDelegate(() => ShowGearSlots);
        c.Emit(OpCodes.Brtrue, target);
    }

    delegate bool orig_DrawVisibility(AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4);
    private bool On_DrawVisibility(orig_DrawVisibility orig, AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4)
    {
        bool skipCheck = orig(self, ref visbility, context, xLoc, yLoc, out xLoc2, out yLoc2, out value4);

        if (ShowGearSlots)
        {
            value4 = Asset<Texture2D>.DefaultValue;
            skipCheck = false;
        }

        return skipCheck;
    }
}
