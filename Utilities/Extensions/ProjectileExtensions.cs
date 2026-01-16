using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Utilities.Extensions;

internal static class ProjectileExtensions
{
    public static T? GetGlobalProjectileOrNull<T>(this Projectile projectile) where T : GlobalProjectile => projectile.TryGetGlobalProjectile(out T result) ? result : null;
}