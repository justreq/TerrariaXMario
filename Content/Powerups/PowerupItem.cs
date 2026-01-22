using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SPBar;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal abstract class PowerupItem : ModItem
{
    internal virtual int? PowerupType => null;
    internal Powerup? PowerupData => PowerupType == null ? null : PowerupLoader.Powerups[(int)PowerupType];

    public override string Texture => base.Texture.Replace("Item", "");
    internal virtual int FrameCount => 1;

    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(1, FrameCount) { NotActuallyAnimating = true });

        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.consumable = true;
        Item.maxStack = 30;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.GetGlobalItemOrNull<SPItem>()?.useSP = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }

    public override bool CanUseItem(Player player)
    {
        return player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatSP >= Item.GetGlobalItemOrNull<SPItem>()?.useSP && PowerupData != null && PowerupData.ProjectileType != null;
    }

    public override bool? UseItem(Player player)
    {
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatSP -= Item.GetGlobalItemOrNull<SPItem>()?.useSP ?? 0;
        Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center - new Vector2(0, 62), Vector2.Zero, (int)PowerupData!.ProjectileType!, 0, 0, player.whoAmI, 1);
        return true;
    }
}
