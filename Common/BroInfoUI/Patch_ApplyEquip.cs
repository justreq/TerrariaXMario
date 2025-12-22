using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

internal class Patch_ApplyEquip : BasePatch
{
    internal override void Patch(Mod mod)
    {
        // removes armor / accessory functionality and visuals when gear slots are enabled
        On_Player.ApplyEquipFunctional += On_Player_ApplyEquipFunctional;
        On_Player.ApplyEquipVanity_int_Item += On_Player_ApplyEquipVanity_int_Item;
        On_Player.ApplyEquipVanity_Item += On_Player_ApplyEquipVanity_Item;
        On_Player.UpdateVisibleAccessories_Item_bool_int_bool += On_Player_UpdateVisibleAccessories_Item_bool_int_bool;
        On_Player.UpdateVisibleAccessory += On_Player_UpdateVisibleAccessory;
        On_Player.SetArmorEffectVisuals += On_Player_SetArmorEffectVisuals;
        On_Player.GrantArmorBenefits += On_Player_GrantArmorBenefits;
        On_Player.UpdateArmorSets += On_Player_UpdateArmorSets;
        On_Player.ApplyArmorSoundAndDustChanges += On_Player_ApplyArmorSoundAndDustChanges;
    }

    private void On_Player_ApplyEquipFunctional(On_Player.orig_ApplyEquipFunctional orig, Player self, Item currentItem, bool hideVisual)
    {
        if ((!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || (currentItem.ModItem?.Mod == TerrariaXMario.Instance))
        {
            orig(self, currentItem, hideVisual);
        }
    }

    private void On_Player_ApplyEquipVanity_int_Item(On_Player.orig_ApplyEquipVanity_int_Item orig, Player self, int itemSlot, Item currentItem)
    {
        if ((!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || (currentItem.ModItem?.Mod == TerrariaXMario.Instance))
        {
            orig(self, itemSlot, currentItem);
        }
    }

    private void On_Player_ApplyEquipVanity_Item(On_Player.orig_ApplyEquipVanity_Item orig, Player self, Item currentItem)
    {
        if ((!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || (currentItem.ModItem?.Mod == TerrariaXMario.Instance))
        {
            orig(self, currentItem);
        }
    }

    private void On_Player_SetArmorEffectVisuals(On_Player.orig_SetArmorEffectVisuals orig, Player self, Player drawPlayer)
    {
        if (!drawPlayer.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true)
        {
            orig(self, drawPlayer);
        }
    }

    private void On_Player_GrantArmorBenefits(On_Player.orig_GrantArmorBenefits orig, Player self, Item armorPiece)
    {
        if (!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true)
        {
            orig(self, armorPiece);
        }
    }

    private void On_Player_UpdateArmorSets(On_Player.orig_UpdateArmorSets orig, Player self, int i)
    {
        if (!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true)
        {
            orig(self, i);
        }
    }

    private void On_Player_ApplyArmorSoundAndDustChanges(On_Player.orig_ApplyArmorSoundAndDustChanges orig, Player self)
    {
        if (!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true)
        {
            orig(self);
        }
    }

    private void On_Player_UpdateVisibleAccessories_Item_bool_int_bool(On_Player.orig_UpdateVisibleAccessories_Item_bool_int_bool orig, Player self, Item item, bool invisible, int slot, bool modded)
    {
        if ((!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || (item.ModItem?.Mod == TerrariaXMario.Instance))
        {
            orig(self, item, invisible, slot, modded);
        }
    }

    private void On_Player_UpdateVisibleAccessory(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
    {
        if ((!self.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || (item.ModItem?.Mod == TerrariaXMario.Instance))
        {
            orig(self, itemSlot, item, modded);
        }
    }
}
