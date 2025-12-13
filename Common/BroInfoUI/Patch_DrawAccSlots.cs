using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal sealed class Patch_DrawAccSlots : BasePatch
{
    internal override void Patch(Mod mod)
    {
        // Prevents accessory slots from drawing when gear slots are enabled
        MonoModHooks.Add(typeof(AccessorySlotLoader).GetMethod("Draw"), Draw);
    }

    delegate bool orig_Draw(AccessorySlotLoader self, int skip, bool modded, int slot, Color color);
    private bool Draw(orig_Draw orig, AccessorySlotLoader self, int skip, bool modded, int slot, Color color)
    {
        if (!Main.LocalPlayer.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) return orig(self, skip, modded, slot, color);

        if (modded && self.Get(slot) is GearSlot) return orig(self, skip, modded, slot, color);

        return false;
    }
}