using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ActionRing : ModProjectile
{
    private readonly Vector2[] offsets = [new Vector2(0, 18), new Vector2(36, 0), new Vector2(0, -18), new Vector2(-36, 0)];

    private Projectile[] actionBoxes = [];
    internal int ActionCount => actionBoxes.Length;
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
                0 => ModContent.ProjectileType<ActionBoxJump>(),
                1 => ModContent.ProjectileType<ActionBoxSpecial>(),
                2 => ModContent.ProjectileType<ActionBoxItem>(),
                3 => ModContent.ProjectileType<ActionBoxFlee>(),
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

        if (modPlayer?.queriedAction == null && modPlayer?.currentAction == null)
        {
            if (Projectile.alpha > 0) Projectile.alpha -= 30;
            else if (Projectile.alpha != 0) Projectile.alpha = 0;
        }
        else
        {
            if (Projectile.alpha < 255) Projectile.alpha += 30;
            else if (Projectile.alpha != 0) Projectile.alpha = 255;
        }

        for (int i = 0; i < ActionCount; i++)
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