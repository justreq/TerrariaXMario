using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TerrariaXMario.Content.Furniture;
internal abstract class StatueTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.addTile(Type);

        DustType = DustID.Silver;

        AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
    }

    private readonly string[] sounds = ["DoubleJump", "TripleJump", "Unequip", "Equip", "Hurt1", "Hurt2", "Hurt3", "Hurt4"];

    public override void HitWire(int i, int j)
    {
        Point16 topLeft = TileObjectData.TopLeft(i, j);

        for (int y = topLeft.Y; y < topLeft.Y + 3; y++)
        {
            for (int x = topLeft.X; x < topLeft.X + 2; x++)
            {
                Wiring.SkipWire(x, y);
            }
        }

        string soundToPlay = sounds[Main.rand.Next(sounds.Length)];

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/{(soundToPlay.Contains("Hurt") ? "PlayerOverrides" : "CapEffects")}/{GetType().Name.Replace("StatueTile", "")}{soundToPlay}") { Volume = 0.4f }, new((topLeft.X + 2 * 0.5f) * 16, (topLeft.Y + 3 * 0.65f) * 16));
    }
}

internal class MarioStatueTile : StatueTile;
internal class LuigiStatueTile : StatueTile;

internal abstract class StatueItem<T> : ModItem where T : StatueTile
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.ArmorStatue);
        Item.createTile = ModContent.TileType<T>();
    }
}

internal class MarioStatue : StatueItem<MarioStatueTile>;
internal class LuigiStatue : StatueItem<LuigiStatueTile>;