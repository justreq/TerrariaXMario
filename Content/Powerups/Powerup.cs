using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Powerups;
internal abstract class Powerup : ModType
{
    protected override void Register()
    {
        ModTypeLookup<Powerup>.Register(this);
    }

    internal virtual string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp";

    /// <summary>
    /// The behavior of this PowerupData when it exists in the world, e.g. movement behavior. Similar in concept to ModItem.Update.
    /// </summary>
    /// <param name="projectile">The projectile that exists in the world and gets consumed by the playerIndex upon contact.</param>
    internal virtual void UpdateWorld(Projectile projectile, int updateCount) { }
    /// <summary>
    /// Any special effects this PowerupData gives when consumed, e.g. dust or unrelated state changes.
    /// </summary>
    /// <param name="player">The playerIndex that consumed this PowerupData</param>
    internal virtual void OnConsume(Player player) { }
    /// <summary>
    /// The effects this PowerupData gives to the playerIndex that consumed it. This is run during ModPlayer.PostUpdateEquips.
    /// </summary>
    /// <param name="player">The playerIndex that consumed this PowerupData</param>
    internal virtual void UpdateConsumed(Player player) { }
    /// <summary>
    /// Use this method to spawn projectiles when left clicking, e.g. Fire Flower Fireball or Hammer Suit Hammer. Return true to force the playerIndex's front arm to swing
    /// </summary>
    /// <param name="player">The playerIndex that consumed this PowerupData</param>
    internal virtual bool OnLeftClick(Player player) => false;
}
