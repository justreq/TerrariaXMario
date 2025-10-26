using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ActionRing : ModProjectile
{
    private readonly Vector2[] offsets = [new Vector2(0, 18), new Vector2(36, 0), new Vector2(0, -18), new Vector2(-36, 0)];

    private Projectile[] actionBoxes = [];
    internal int currentAction;

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.alpha = 255;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override bool PreDraw(ref Color lightColor) => false;

    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < 4; i++)
        {
            int type = i switch
            {
                0 => ModContent.ProjectileType<ActionJump>(),
                1 => ModContent.ProjectileType<ActionSpecial>(),
                2 => ModContent.ProjectileType<ActionItem>(),
                3 => ModContent.ProjectileType<ActionFlee>(),
                _ => 0,
            };

            actionBoxes = [.. actionBoxes, Projectile.NewProjectileDirect(source, Projectile.position, Vector2.Zero, type, 0, 0, Projectile.owner)];
        }
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        ShowdownPlayer? modPlayer = player.GetModPlayerOrNull<ShowdownPlayer>();

        Projectile.timeLeft++;

        if (modPlayer?.queriedAction == ShowdownAction.None && modPlayer?.currentAction == ShowdownAction.None)
        {
            if (Projectile.alpha > 0) Projectile.alpha -= 30;
            else if (Projectile.alpha != 0) Projectile.alpha = 0;

            if (actionBoxes.Length > 1 && player.IsOnGroundPrecise() && (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right))
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Showdown/ScrollActions") { Volume = 0.2f });

                if (PlayerInput.Triggers.JustPressed.Left) currentAction = (currentAction == 0 ? actionBoxes.Length : currentAction) - 1;
                if (PlayerInput.Triggers.JustPressed.Right) currentAction = currentAction == actionBoxes.Length - 1 ? 0 : currentAction + 1;
            }
        }
        else
        {
            if (Projectile.alpha < 255) Projectile.alpha += 30;
            else if (Projectile.alpha != 0) Projectile.alpha = 255;
        }

        for (int i = 0; i < actionBoxes.Length; i++)
        {
            Projectile actionBox = actionBoxes[i];

            actionBox.alpha = Projectile.alpha;

            Vector2 offset = offsets[(i + (offsets.Length - currentAction)) % offsets.Length];
            actionBox.position.X = MathHelper.Lerp(actionBox.position.X, Projectile.position.X + offset.X, 0.25f);
            actionBox.position.Y = MathHelper.Lerp(actionBox.position.Y, Projectile.position.Y + offset.Y, 0.25f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        foreach (Projectile actionBox in actionBoxes)
        {
            actionBox.Kill();
        }
    }
}