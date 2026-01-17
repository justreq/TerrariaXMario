using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.SpawnableObject;

namespace TerrariaXMario.Content.Consumables;

internal class BooBiscuit : ModItem, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(246, 156, 8), new(139, 74, 0), new(189, 106, 0)];
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
        player.AddBuff(ModContent.BuffType<BooBiscuitBuff>(), 1800);

        return true;
    }
}

internal class BooBiscuitBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.invis = true;
    }
}