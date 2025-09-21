using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ObjectSpawnerBlockUI;
internal class ObjectSpawnerBlockPlayer : CapEffectsPlayer
{
    public override void PostUpdate()
    {
        if (!CapPlayer?.CanDoCapEffects ?? true) return;

        Point? point = Player.IsBelowObjectSpawnerBlockPrecise(1);
        if (point == null) return;

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/BlockHit"));

        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull((Point)point);
        if (entity == null || entity.spawnContents.Length == 0 || entity.justStruck) return;

        ISpawnableObject objectToSpawn = entity!.spawnContents[0];

        if (objectToSpawn is ModProjectile projectile)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            Projectile.NewProjectile(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8), new Vector2(0, -0.75f), projectile.Type, 0, 0, ai0: 45);
        }
        else if (objectToSpawn is ModItem item)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, -24), new Item(item.Type), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 20;
            Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>()?.objectSpawnerBlockEntity = entity.Position;
        }
        else if (objectToSpawn is DefaultSpawnableObject)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Coin") { Volume = 3f });
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, -24), new Item(ItemID.GoldCoin), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 20;
            Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>()?.objectSpawnerBlockEntity = entity.Position;
        }

        entity?.spawnContents = [.. entity.spawnContents.Skip(1).ToArray()];
        entity?.justStruck = true;
        if (entity?.spawnContents.Length == 0) entity.wasPreviouslyStruck = true;
    }
}
