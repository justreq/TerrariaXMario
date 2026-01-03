using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using System.Collections.Generic;
using TerrariaXMario.Core;
using Terraria.Audio;

namespace TerrariaXMario.Content.Consumables;

internal abstract class EdibleMushroom : ModItem, ISpawnableObject
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
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
        Item.potion = true;
    }

    public override void OnConsumeItem(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.MountedCenter);
    }
}

internal class EdibleMushroom2 : EdibleMushroom
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(234, 249, 255), new(234, 49, 32), new(255, 188, 140)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 100;
    }
}

internal class EdibleMushroom3 : EdibleMushroom
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(234, 249, 255), new(234, 176, 34), new(255, 188, 140)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 200;
    }
}

internal class EdibleMushroom4 : EdibleMushroom
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(234, 51, 34), new(234, 176, 34), new(255, 188, 140)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 300;
    }
}

internal class EdibleMushroomBad : EdibleMushroom
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(251, 195, 230), new(65, 31, 78), new(147, 63, 164)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.buffType = BuffID.Venom;
        Item.buffTime = 300;
        Item.potion = false;
    }

    public override void OnConsumeItem(Player player)
    {
        // play disgusted sound
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.RemoveAt(tooltips.FindIndex(e => e.Name == "BuffTime"));
    }
}