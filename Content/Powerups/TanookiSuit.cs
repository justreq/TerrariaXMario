using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;

namespace TerrariaXMario.Content.Powerups;
internal class TanookiSuitData : SuperLeafData
{
    public override string Name => "TanookiSuit";

    internal override void UpdateWorld(Projectile projectile, int updateCount)
    {
        projectile.velocity.Y += 0.4f;
    }

    internal override void OnJumpHeldDown(Player player)
    {
        DoJumpHold(player, 3);
    }
}

internal class TanookiSuit : PowerupProjectile
{
    internal override int? PowerupType => ModContent.GetInstance<TanookiSuitData>().Type;
    internal override string[] Caps => [nameof(Mario), nameof(Luigi)];
    internal override string[] Variations => ["Flying"];
}