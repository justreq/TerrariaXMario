using Microsoft.Xna.Framework;
using Terraria;

namespace TerrariaXMario.Content.Blocks;
internal class QuestionBlockTile : ObjectSpawnerBlockTile
{
    internal override Color MapColor => new(255, 196, 0);

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        if (++frameCounter >= 10)
        {
            frameCounter = 0;
            frame = ++frame % 4;
        }
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        frameYOffset = 36 * (TerrariaXMario.GetTileEntityOrNull(i, j)?.ShouldShowEmpty ?? false ? 4 : Main.tileFrame[type]);
    }
}

internal class QuestionBlock : ObjectSpawnerBlockItem<QuestionBlockTile>;