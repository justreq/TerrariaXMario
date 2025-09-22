using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.CapEffects;
internal class SpawnedItem : GlobalItem
{
    internal Point16 objectSpawnerBlockEntity;
    internal bool spawnedFromBottom;

    public override bool InstancePerEntity => true;

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
    {
        if (objectSpawnerBlockEntity == Point16.Zero)
        {
            base.Update(item, ref gravity, ref maxFallSpeed);
            return;
        }

        if (item.noGrabDelay > 0)
        {
            item.velocity = Vector2.Zero;
            item.Center = objectSpawnerBlockEntity.ToWorldCoordinates() + new Vector2(8, spawnedFromBottom ? 36 : -24);
            gravity = 0;
        }

        base.Update(item, ref gravity, ref maxFallSpeed);
    }
}
