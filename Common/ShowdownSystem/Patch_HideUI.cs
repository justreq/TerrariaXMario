using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class Patch_HideUI : BasePatch
{
    // Allows me to show certain UI while the F11 hide UI option is turned on
    internal override void Patch(Mod mod)
    {
        //IL_Main.DrawInterface_8_CheckF11UIHideToggle += IL_Main_DrawInterface_8_CheckF11UIHideToggle;
        //IL_Main.DrawInterface_9_WireSelection += IL_Main_DrawInterface_9_WireSelection;
        //IL_Main.DrawInterface_14_EntityHealthBars += IL_Main_DrawInterface_14_EntityHealthBars;
        //IL_Main.DoDraw += IL_Main_DoDraw;
    }

    private void IL_Main_DrawInterface_8_CheckF11UIHideToggle(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("hideUI"))) ThrowError("Ldsfld");
        c.Remove();

        c.EmitDelegate(() => Main.hideUI && (!Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? true));
    }

    private void IL_Main_DrawInterface_9_WireSelection(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("hideUI"))) ThrowError("Ldsfld");
        c.Remove();

        c.EmitDelegate(() => !Main.hideUI || (Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false));
    }

    private void IL_Main_DrawInterface_14_EntityHealthBars(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("hideUI"))) ThrowError("Ldsfld");
        c.Remove();

        c.EmitDelegate(() => !Main.hideUI || (Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false));
    }

    private void IL_Main_DoDraw(ILContext il)
    {
        ILCursor c = new(il);

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("hideUI"))) ThrowError("Ldsfld");
        c.Remove();

        c.EmitDelegate(() => !Main.hideUI || (Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false));
    }
}
