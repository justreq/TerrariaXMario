using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using TerrariaXMario.Core;

namespace TerrariaXMario.Content.Consumables;

internal class RefreshingHerb : ModItem, ISpawnableObject
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(24, 165, 48), new(45, 229, 45), new(255, 255, 211)];
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 30;
        Item.UseSound = SoundID.Item2;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.useTurn = true;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.consumable = true;
    }

    public override bool? UseItem(Player player)
    {
        for (int i = 0; i < player.buffType.Length; i++)
        {
            int buffType = player.buffType[i];
            if (!BuffID.Sets.NurseCannotRemoveDebuff[buffType] && Main.debuff[buffType])
            {
                player.ClearBuff(buffType);
                i--;
            }
        }

        return true;
    }
}