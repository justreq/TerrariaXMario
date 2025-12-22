using Terraria.ModLoader;
using Terraria;
using TerrariaXMario.Core;

namespace TerrariaXMario.Content.Consumables;

internal abstract class RevivingMushroom : ModItem, ISpawnableObject
{
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

internal class OneUpMushroom : RevivingMushroom { }

internal class OneUpDeluxe : RevivingMushroom { }