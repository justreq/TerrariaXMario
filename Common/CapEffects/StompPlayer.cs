using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class StompPlayer : CapEffectsPlayer
{
    int? stompHitbox;

    public override void PostUpdate()
    {
        if ((!CapPlayer?.CanDoCapEffects ?? true) || Player.mount.Active)
        {
            if (stompHitbox != null)
            {
                Main.projectile[(int)stompHitbox].Kill();
                stompHitbox = null;
            }

            return;
        }

        if (!Player.IsOnGroundPrecise())
        {
            stompHitbox ??= Projectile.NewProjectile(Player.GetSource_FromThis(), Player.BottomLeft, Vector2.Zero, ModContent.ProjectileType<StompHitbox>(), 1, 0, Player.whoAmI);
        }
        else if (stompHitbox != null)
        {
            Main.projectile[(int)stompHitbox].Kill();
            stompHitbox = null;
            groundPounding = false;
        }
    }
}
