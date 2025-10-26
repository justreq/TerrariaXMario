using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Common.KeybindSystem;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal abstract class ActionBox : ModProjectile
{
    internal abstract ShowdownAction Action { get; set; }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.hide = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        Projectile.timeLeft++;

        if (modPlayer?.queriedAction != Action && Projectile.alpha == 0 && Projectile.Hitbox.Intersects(player.Hitbox))
        {
            player.velocity.Y = 0;
            Projectile.position.Y -= 6;
            modPlayer?.queriedAction = Action;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Showdown/ChooseAction") { Volume = 0.4f });

            if (Action == ShowdownAction.Flee) player.GetModPlayerOrNull<KeybindPlayer>()?.keyToShowInIndicator = KeybindSystem.KeybindSystem.GetVanillaKeybindKey(TriggerNames.Jump);
        }
    }

    public override void PostDraw(Color lightColor)
    {
        if (Projectile.Hitbox.Contains(Main.MouseWorld.ToPoint()) && Projectile.alpha == 0) Main.instance.MouseText(Name.Replace("Action", ""));
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }
}

internal class ActionJump : ActionBox
{
    internal override ShowdownAction Action { get; set; } = ShowdownAction.Jump;
}
internal class ActionSpecial : ActionBox
{
    internal override ShowdownAction Action { get; set; } = ShowdownAction.Special;
}
internal class ActionItem : ActionBox
{
    internal override ShowdownAction Action { get; set; } = ShowdownAction.Item;
}
internal class ActionFlee : ActionBox
{
    internal override ShowdownAction Action { get; set; } = ShowdownAction.Flee;
}