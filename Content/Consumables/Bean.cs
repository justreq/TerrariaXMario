using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Consumables;

internal class BeanDefense : ModItem, ISpawnableObject
{
    internal static CapEffectsPlayer? CapEffectsPlayer(Player player) => player.GetModPlayerOrNull<CapEffectsPlayer>();

    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Rare;
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
        CapEffectsPlayer(player)?.StatDefense += 1;
        return true;
    }
}

internal class BeanDefenseDown : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(50, 33, 9), new(149, 124, 107), new(181, 157, 132)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.StatDefense -= 1;
        return true;
    }
}

internal class BeanHP : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 139, 255), new(189, 32, 189), new(156, 0, 156)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.StatHP += 1;
        return true;
    }
}

internal class BeanHPDown : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(0, 116, 0), new(66, 223, 66), new(99, 255, 99)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.StatHP -= 1;
        return true;
    }
}

internal class BeanSP : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(148, 255, 49), new(41, 172, 8), new(0, 90, 8)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.maxSP += 1;
        return true;
    }
}

internal class BeanSPDown : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(107, 0, 206), new(214, 83, 247), new(255, 165, 247)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.maxSP -= 1;
        return true;
    }
}

internal class BeanPow : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 180, 41), new(213, 49, 0), new(164, 16, 0)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.StatPower += 1;
        return true;
    }
}

internal class BeanPowDown : BeanDefense
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(0, 75, 214), new(42, 206, 255), new(91, 239, 255)];
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer(player)?.StatPower -= 1;
        return true;
    }
}