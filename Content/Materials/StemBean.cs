using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.SpawnableObject;

namespace TerrariaXMario.Content.Materials;

internal class StemBean : ModItem, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Epic;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.maxStack = 30;
    }
}