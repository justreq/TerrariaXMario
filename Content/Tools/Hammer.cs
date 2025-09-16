using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Tools;
internal class Hammer : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.useTime = 24;
        Item.useAnimation = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item1;
    }
}
