using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Common.SPBar;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Consumables;

internal class StarCandy1 : ModItem, ISpawnableObject
{
    internal HealSPItem? HealSPItem => Item.GetGlobalItemOrNull<HealSPItem>();
    internal virtual SpawnRarity SpawnRarity { get; set; } = SpawnRarity.Uncommon;
    SpawnRarity ISpawnableObject.SpawnRarity { get => SpawnRarity; set => SpawnRarity = value; }

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
        Item.potion = true;
        HealSPItem?.healSP = 25;
    }

    public override void OnConsumeItem(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.MountedCenter);
        CapEffectsPlayer.RestoreSP(player, HealSPItem?.healSP ?? 0);
    }
}

internal class StarCandy2 : StarCandy1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 100;
        HealSPItem?.healSP = 50;
        SpawnRarity = SpawnRarity.Rare;
    }
}

internal class StarCandy3 : StarCandy1
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 200;
        HealSPItem?.healSP = 75;
        SpawnRarity = SpawnRarity.Epic;
    }
}

internal class StarCandy4 : StarCandy1
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(87, 24, 99), new(51, 102, 204), new(181, 218, 255)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 300;
        HealSPItem?.healSP = 100;
        SpawnRarity = SpawnRarity.Legendary;
    }
}