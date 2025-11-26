using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.PowerupProjectiles;
internal abstract class InteractiveWithObjectSpawnerTileProjectile : ModProjectile
{
    internal virtual float TileCheckExtent => 0;

    public override void PostAI()
    {
        for (int i = (int)(Projectile.TopLeft.X - TileCheckExtent); i < Projectile.BottomRight.X + TileCheckExtent; i++)
        {
            for (int j = (int)(Projectile.TopLeft.Y - TileCheckExtent); j < Projectile.BottomRight.Y + TileCheckExtent; j++)
            {
                Point position = new Vector2(i, j).ToTileCoordinates();
                if (TileLoader.GetTile(Framing.GetTileSafely(position).TileType) is ObjectSpawnerBlockTile) Main.player[Projectile.owner].GetModPlayerOrNull<CapEffectsPlayer>()?.SpawnObjectFromBlock(position);
            }
        }
    }
}
