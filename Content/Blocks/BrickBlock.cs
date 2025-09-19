using Microsoft.Xna.Framework;

namespace TerrariaXMario.Content.Blocks;
internal class BrickBlockTile : ObjectSpawnerBlockTile
{
    internal override Color MapColor => new(200, 88, 0);

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull(i, j);

        frameYOffset = entity?.wasPreviouslyStruck ?? false && entity?.spawnContents.Length == 0 ? 36 : 0;
    }
}

internal class BrickBlock : ObjectSpawnerBlockItem<BrickBlockTile>;