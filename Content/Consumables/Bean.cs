using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Content.Materials;

namespace TerrariaXMario.Content.Consumables;

internal class BeanDefenseDown : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(205, 222, 246), new(106, 131, 148), new(74, 98, 123)];
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
        CapEffectsPlayer.IncreaseMaxStat(player, 2, -1);
        return true;
    }

    public override void AddRecipes()
    {
        Recipe.Create(Item.type)
            .AddIngredient(ModContent.ItemType<StemBean>())
            .Register();
    }
}

internal class BeanDefense : BeanDefenseDown, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 2, 1);
        return true;
    }
}

internal class BeanHPDown : BeanDefenseDown
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 139, 255), new(189, 32, 189), new(156, 0, 156)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 1, -1);
        return true;
    }
}

internal class BeanHP : BeanHPDown, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;
    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 1, 1);
        return true;
    }
}

internal class BeanSPDown : BeanDefenseDown
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(148, 255, 49), new(41, 172, 8), new(0, 90, 8)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 0, -1);
        return true;
    }
}

internal class BeanSP : BeanSPDown, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;
    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 0, 1);
        return true;
    }
}

internal class BeanPowDown : BeanDefenseDown
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 180, 41), new(213, 49, 0), new(164, 16, 0)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 3, -1);
        return true;
    }
}

internal class BeanPow : BeanPowDown, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;
    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.IncreaseMaxStat(player, 3, 1);
        return true;
    }
}