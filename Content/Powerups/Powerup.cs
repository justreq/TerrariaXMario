using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Powerups;
internal class Powerup : ModType
{
    protected override void Register() { }

    /// <summary>
    /// A list of the caps that can consume this PowerupData.
    /// </summary>
    internal virtual string[] Caps => [];
    internal virtual string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp";

    /// <summary>
    /// The behavior of this PowerupData when it exists in the world, e.g. movement behavior. Similar in concept to ModItem.Update.
    /// </summary>
    /// <param name="projectile">The projectile that exists in the world and gets consumed by the player upon contact.</param>
    internal virtual void UpdateWorld(Projectile projectile) { }
    /// <summary>
    /// Any special effects this PowerupData gives when consumed, e.g. dust or unrelated state changes.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void OnConsume(Player player) { }
    /// <summary>
    /// The effects this PowerupData gives to the player that consumed it. This is run during ModPlayer.PostUpdateEquips.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void UpdateConsumed(Player player) { }
    /// <summary>
    /// Use this method to spawn projectiles when left clicking, e.g. Fire Flower Fireball or Hammer Suit Hammer.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void OnLeftClick(Player player) { }
}
