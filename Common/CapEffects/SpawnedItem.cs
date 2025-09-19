using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.CapEffects;
internal class SpawnedItem : GlobalItem
{
    internal Point16 objectSpawnerBlockEntity;

    public override bool InstancePerEntity => true;

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
    {
        if (item.noGrabDelay > 0)
        {
            item.Center = objectSpawnerBlockEntity.ToWorldCoordinates() + new Vector2(8, -24);
            gravity = 0;
        }

        base.Update(item, ref gravity, ref maxFallSpeed);
    }
}
