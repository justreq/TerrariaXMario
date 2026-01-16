using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.SpawnableObject;

namespace TerrariaXMario.Content.Consumables;

internal class RevivingMushroom1 : ModItem, ISpawnableObject
{
    internal virtual SpawnRarity SpawnRarity { get; set; } = SpawnRarity.Epic;
    SpawnRarity ISpawnableObject.SpawnRarity { get => SpawnRarity; set => SpawnRarity = value; }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 30;
    }
}

internal class RevivingMushroom2 : RevivingMushroom1
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        SpawnRarity = SpawnRarity.Legendary;
    }
}