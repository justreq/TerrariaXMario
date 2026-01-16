using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.SpawnableObject;

namespace TerrariaXMario.Content.Consumables;

internal class Nut1 : ModItem, ISpawnableObject
{
    internal virtual SpawnRarity SpawnRarity { get; set; }
    SpawnRarity ISpawnableObject.SpawnRarity { get => SpawnRarity; set => SpawnRarity = value; }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 215, 0), new(204, 153, 0), new(102, 51, 0)];
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
        Item.potion = true;
    }

    public override void OnConsumeItem(Player player)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.MountedCenter);

        foreach (Player p in Main.ActivePlayers)
        {
            if (p == player) continue;

            if (p.team == player.team && p.Center.DistanceSQ(player.MountedCenter) <= 500000)
            {
                p.Heal(Item.healLife);
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, p.Center);
            }

        }
    }
}

internal class Nut2 : Nut1
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(218, 165, 32), new(0, 255, 0), new(255, 191, 0)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 100;
        SpawnRarity = SpawnRarity.Uncommon;
    }
}

internal class Nut3 : Nut1
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 221, 85), new(85, 255, 85), new(255, 170, 0)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 200;
        SpawnRarity = SpawnRarity.Rare;
    }
}

internal class Nut4 : Nut1
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.FoodParticleColors[Item.type] = [new(255, 204, 0), new(255, 255, 15), new(153, 102, 0)];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 300;
        SpawnRarity = SpawnRarity.Epic;
    }
}