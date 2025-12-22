using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using TerrariaXMario.Core;
using System.Collections.Generic;

namespace TerrariaXMario.Content.Consumables;

internal class SyrupJar : ModItem, ISpawnableObject
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(247, 65, 0), new(173, 0, 0), new(255, 190, 82)];
        ItemID.Sets.DrinkParticleColors[Item.type] = [new(255, 231, 156), new(255, 244, 127), new(201, 171, 50)];
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 30;
        Item.UseSound = SoundID.Item3;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.consumable = true;
        Item.healMana = 15;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine? line = tooltips.Find(e => e.Name == "HealMana");
        line?.Text = line?.Text.Replace("mana", "special points");
    }
}

internal class SuperSyrupJar : SyrupJar
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healMana = 30;
    }
}

internal class UltraSyrupJar : SyrupJar
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healMana = 60;
    }
}

internal class MaxSyrupJar : SyrupJar
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healMana = 120;
    }
}