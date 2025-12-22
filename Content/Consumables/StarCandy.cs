using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Core;

namespace TerrariaXMario.Content.Consumables;

internal class StarCandy : ModItem, ISpawnableObject
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(181, 0, 57), new(255, 134, 0), new(255, 231, 156)];
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
        Item.healLife = 15;
        Item.healMana = 15;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine? line = tooltips.Find(e => e.Name == "HealMana");
        line?.Text = line?.Text.Replace("mana", "special points");
    }

    public override void OnConsumeItem(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.Center);
    }
}

internal class SuperStarCandy : StarCandy
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 30;
        Item.healMana = 30;
    }
}

internal class UltraStarCandy : StarCandy
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 60;
        Item.healMana = 60;
    }
}

internal class MaxStarCandy : StarCandy
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 120;
        Item.healMana = 120;
    }
}