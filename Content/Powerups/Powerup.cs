using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;

namespace TerrariaXMario.Content.Powerups;

internal abstract class Powerup : ModType
{
    internal int Type { get; set; }

    protected override void Register()
    {
        ModTypeLookup<Powerup>.Register(this);
        Type = PowerupLoader.Add(this);
    }

    public override void SetupContent()
    {
        PowerupID.Search.Add(FullName, Type);
    }

    internal virtual string EquipSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp";
    internal virtual ForceArmMovementType RightClickArmMovementType => ForceArmMovementType.None;
    internal virtual bool LookTowardRightClick => true;
    internal virtual int RightClickActionCooldown => 5;
    internal virtual Color Color => Color.White;

    /// <summary>
    /// The behavior of this PowerupData when it exists in the world, e.g. movement behavior. Similar in concept to ModItem.Update.
    /// </summary>
    /// <param name="projectile">The projectile that exists in the world and gets consumed by the playerIndex upon contact.</param>
    internal virtual void UpdateWorld(Projectile projectile, int updateCount)
    { }
    /// <summary>
    /// Any special effects this PowerupData gives when consumed, e.g. dust or unrelated state changes.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void OnConsume(Player player) { }
    /// <summary>
    /// The effects this PowerupData gives to the player that consumed it. This is run during ModItem.UpdateAccessory.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void UpdateConsumed(Player player) { }
    /// <summary>
    /// Use this method to spawn projectiles when right clicking, e.g. Fire Flower Fireball or Hammer Suit Hammer.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void OnRightClick(Player player) { }
    /// <summary>
    /// Use this method to do something while the Jump button is pressed down, e.g. Tail Gliding.
    /// </summary>
    /// <param name="player">The player that consumed this PowerupData</param>
    internal virtual void OnJumpHeldDown(Player player) { }
}
