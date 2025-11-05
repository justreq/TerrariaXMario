using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.ShowdownActions;
internal abstract class ShowdownAction : ModType
{
    protected override void Register() { }

    internal int updateCount;

    internal bool Active { get; private set; }

    /// <summary>
    /// This is run once when the action is initiated
    /// </summary>
    internal virtual void Initiate(Entity owner) { }
    /// <summary>
    /// This is run in AI() for NPCs and SetControls() for Players (in case the attack should programatically trigger player inputs) while the action is active
    /// </summary>
    internal virtual void Update(Entity owner) { }
    /// <summary>
    /// What happens when the owner of this move hits an opponent while the action is active
    /// </summary>
    internal virtual void OnHit(Entity owner, Entity target) { }
}
