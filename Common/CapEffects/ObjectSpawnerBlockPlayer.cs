using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class ObjectSpawnerBlockPlayer : CapEffectsPlayer
{
    public override void PostUpdate()
    {
        if (Player.velocity.Y >= 0 || Player.IsOnGroundPrecise() || (!CapPlayer?.CanDoCapEffects ?? true)) return;

        foreach (Point point in HitObjectSpawnerBlocks(1))
        {
            SpawnObjectFromBlock(point);
        }
    }

    private static Point[] HitObjectSpawnerBlocks(in float startX, float y, int width)
    {
        if (width <= 0)
        {
            throw new ArgumentException("width cannot be negative");
        }

        float fx = startX;

        IEnumerable<Point> blocks = [];

        //Needs atleast one iteration (in case width is 0)
        do
        {
            Point point = new Vector2(fx, y + 0.01f).ToTileCoordinates(); //0.01f is a magic number vanilla uses
            if (TerrariaXMario.SolidTile(point.X, point.Y) && TileLoader.GetTile(Framing.GetTileSafely(point).TileType) is ObjectSpawnerBlockTile)
            {
                blocks = blocks.Append(point);
            }
            fx += 16;
        }
        while (fx < startX + width);

        return [.. blocks];
    }

    internal Point[] HitObjectSpawnerBlocks(float yOffset = 0f, bool checkBottom = false)
    {
        return HitObjectSpawnerBlocks(checkBottom ? Player.BottomLeft.X : Player.TopLeft.X, checkBottom ? Player.BottomLeft.Y + yOffset : Player.TopLeft.Y - yOffset, Player.width);
    }

    internal void SpawnObjectFromBlock(Point? point, bool spawnFromBottom = false)
    {
        if (point == null) return;

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/BlockHit") { Volume = 8 });

        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull((Point)point);
        if (entity == null || entity.justStruck || entity.wasPreviouslyStruck) return;

        if (entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(BrickBlockTile))
        {
            WorldGen.KillTile(point.Value.X, point.Value.Y);
            return;
        }

        ISpawnableObject objectToSpawn = entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(QuestionBlockTile) ? new DefaultSpawnableObject() : entity!.spawnContents[0];

        if (objectToSpawn is ModProjectile projectile)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            Projectile.NewProjectile(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8), new Vector2(0, 0.75f * (spawnFromBottom ? 1 : -1)), projectile.Type, 0, 0, ai0: 45);
        }
        else if (objectToSpawn is ModItem item)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, 24 * (spawnFromBottom ? 1 : -1)), new Item(item.Type), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }
        else if (objectToSpawn is DefaultSpawnableObject)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Coin") { Volume = 3f });
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, 24 * (spawnFromBottom ? 1 : -1)), new Item(ItemID.GoldCoin), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }

        entity?.spawnContents = [.. entity.spawnContents.Skip(1).ToArray()];
        entity?.justStruck = true;
        if (entity?.spawnContents.Length == 0) entity.wasPreviouslyStruck = true;
        Player.velocity.Y = 1;
    }
}
