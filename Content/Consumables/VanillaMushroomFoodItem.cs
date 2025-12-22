using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Consumables;

internal class VanillaMushroomFoodItem : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        if (entity.type != ItemID.Mushroom)
        {
            base.SetDefaults(entity);
            return;
        }

        Main.RegisterItemAnimation(entity.type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[entity.type] = true;
        ItemID.Sets.FoodParticleColors[entity.type] = [new(234, 249, 255), new(234, 49, 32), new(255, 188, 140)];

        entity.width = 32;
        entity.height = 32;
        entity.maxStack = 30;
        entity.UseSound = SoundID.Item2;
        entity.useStyle = ItemUseStyleID.EatFood;
        entity.useTurn = true;
        entity.useTime = 17;
        entity.useAnimation = 17;
        entity.consumable = true;
        entity.healLife = 15;
    }

    public override void OnConsumeItem(Item item, Player player)
    {
        if (item.type == ItemID.Mushroom) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/Heal") { Volume = 0.4f }, player.Center);
        else base.OnConsumeItem(item, player);
    }
}
