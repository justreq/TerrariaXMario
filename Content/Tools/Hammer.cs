using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

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

    public override bool CanUseItem(Player player) => player.GetModPlayerOrNull<CapEffectsPlayer>()?.CanDoCapEffects ?? false;

    public override bool? UseItem(Player player)
    {
        if (Main.cursorOverride == TerrariaXMario.Instance.CursorEditIndex)
        {
            Point mouseTilePosition = Main.MouseWorld.ToTileCoordinates();

            Tile tile = Framing.GetTileSafely(mouseTilePosition.X, mouseTilePosition.Y);
            player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentObjectSpawnerBlockToEdit = new(mouseTilePosition.X - tile.TileFrameX / 18, mouseTilePosition.Y - tile.TileFrameY / 18);
        }

        return base.UseItem(player);
    }
}
