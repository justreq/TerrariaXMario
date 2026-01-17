using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Common.SPBar;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Consumables;

internal class SyrupJar1 : ModItem, ISpawnableObject
{
    internal HealSPItem? HealSPItem => Item.GetGlobalItemOrNull<HealSPItem>();
    internal virtual SpawnRarity SpawnRarity { get; set; }
    SpawnRarity ISpawnableObject.SpawnRarity { get => SpawnRarity; set => SpawnRarity = value; }

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
        HealSPItem?.healSP = 25;
    }

    public override bool? UseItem(Player player)
    {
        CapEffectsPlayer.RestoreSP(player, HealSPItem?.healSP ?? 0);
        return true;
    }
}

internal class SyrupJar2 : SyrupJar1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        SpawnRarity = SpawnRarity.Uncommon;
        HealSPItem?.healSP = 50;
    }
}

internal class SyrupJar3 : SyrupJar1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        SpawnRarity = SpawnRarity.Rare;
        HealSPItem?.healSP = 75;
    }
}

internal class SyrupJar4 : SyrupJar1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        SpawnRarity = SpawnRarity.Epic;
        HealSPItem?.healSP = 100;
    }
}