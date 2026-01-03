using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Consumables;

internal class RevivingMushroomPlayer : ModPlayer
{
    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        bool hasOneUp = Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<RevivingMushroom1>());
        bool hasOneUpDeluxe = Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<RevivingMushroom2>());

        if (!hasOneUp && !hasOneUpDeluxe) return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/OneUp") { Volume = 0.4f }, Player.MountedCenter);

        int item = -1;
        bool inVoidBag = false;
        int inventorySlot = 0;

        for (int i = 0; i < Player.inventory.Length; i++)
        {
            if (Player.inventory[i].type == ModContent.ItemType<RevivingMushroom1>() || Player.inventory[i].type == ModContent.ItemType<RevivingMushroom2>())
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
                if (Player.bank4.item[i].type == ModContent.ItemType<RevivingMushroom1>() || Player.bank4.item[i].type == ModContent.ItemType<RevivingMushroom2>())
                {
                    item = Player.bank4.item[i].type;
                    inventorySlot = i;
                    break;
                }
            }
        }

        int healAmount = Player.statLifeMax2 / (item == ModContent.ItemType<RevivingMushroom1>() ? 2 : 1);
        Player.Heal(healAmount);

        if (inVoidBag) Player.bank4.item[inventorySlot].stack--;
        else Player.inventory[inventorySlot].stack--;

        return false;
    }
}
