using Terraria.ModLoader;
using TerrariaXMario.Content.Blocks;

namespace TerrariaXMario.Common.MiscEffects;
internal class BrickBlockKillTile : GlobalTile
{
    public override bool CanDrop(int i, int j, int type)
    {
        if (type == ModContent.TileType<BrickBlockTile>()) return false;
        return base.CanDrop(i, j, type);
    }
}
