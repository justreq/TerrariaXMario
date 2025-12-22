using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Consumables;

internal class RevivingMushroomPlayer : ModPlayer
{
    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        bool hasOneUp = Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<OneUpMushroom>());
        bool hasOneUpDeluxe = Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<OneUpDeluxe>());

        if (!hasOneUp && !hasOneUpDeluxe) return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/OneUp") { Volume = 0.4f }, Player.Center);

        int item = -1;
        bool inVoidBag = false;
        int inventorySlot = 0;

        for (int i = 0; i < Player.inventory.Length; i++)
        {
            if (Player.inventory[i].type == ModContent.ItemType<OneUpMushroom>() || Player.inventory[i].type == ModContent.ItemType<OneUpDeluxe>())
            {
                item = Player.inventory[i].type;
                inventorySlot = i;
                break;
            }
        }

        if (item == -1)
        {
            inVoidBag = true;

            for (int i = 0; i < Player.bank4.item.Length; i++)
            {
                if (Player.bank4.item[i].type == ModContent.ItemType<OneUpMushroom>() || Player.bank4.item[i].type == ModContent.ItemType<OneUpDeluxe>())
                {
                    item = Player.bank4.item[i].type;
                    inventorySlot = i;
                    break;
                }
            }
        }

        int healAmount = Player.statLifeMax2 / (item == ModContent.ItemType<OneUpMushroom>() ? 2 : 1);
        Player.Heal(healAmount);

        if (inVoidBag) Player.bank4.item[inventorySlot].stack--;
        else Player.inventory[inventorySlot].stack--;

        return false;
    }
}
