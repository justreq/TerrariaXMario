using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Core.Effects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownProjectile : GlobalProjectile
{
    private int? npcOwnerIndex;

    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        base.OnSpawn(projectile, source);
        if (source is EntitySource_Parent parentSource && parentSource.Entity is not Player) npcOwnerIndex = parentSource.Entity.whoAmI;
    }

    public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(projectile, target, info);
        if (npcOwnerIndex == null) return;

        ShowdownPlayer? modPlayer = target.GetModPlayerOrNull<ShowdownPlayer>();

        if (modPlayer?.showdownNPCIndex != null) Main.npc[(int)modPlayer?.showdownNPCIndex!].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.None;
        modPlayer?.showdownNPCIndex = npcOwnerIndex;
        Main.npc[(int)npcOwnerIndex].GetGlobalNPCOrNull<ShowdownNPC>()?.showdownState = NPCShowdownState.Query;
        target.GetModPlayerOrNull<KeybindPlayer>()?.keybindToShowInIndicator = KeybindSystem.KeybindSystem.EnterShowdownKeybind?.GetAssignedKeys().FirstOrDefault();
        Outline.outlineNeeded = true;
    }
}
