using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Consumables;

internal class StarCandy1 : ModItem, ISpawnableObject
{
    internal virtual int SPReplenishAmount => 25;

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
        Item.healLife = 50;
    }

    public override void OnConsumeItem(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.Center);
        CapEffectsPlayer.RestoreSP(player, SPReplenishAmount);
    }
}

internal class StarCandy2 : StarCandy1
{
    internal override int SPReplenishAmount => 50;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 100;
    }
}

internal class StarCandy3 : StarCandy1
{
    internal override int SPReplenishAmount => 75;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 200;
    }
}

internal class StarCandy4 : StarCandy1
{
    internal override int SPReplenishAmount => 100;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 300;
    }
}