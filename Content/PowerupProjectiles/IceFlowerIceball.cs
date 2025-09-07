using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.PowerupProjectiles;
internal class IceFlowerIceball : FireFlowerFireball
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        gravity = 0.2f;
        bounceSpeed = -4;
        dustType = DustID.BlueTorch;
        dustChance = 2;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        base.OnTileCollide(oldVelocity);

        return tileCollideCount >= 2;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage.CombineWith(new(0, 0));
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        SoundEngine.PlaySound(new($"{GetType().FullName!.Replace(".", "/")}Kill") { Volume = 0.4f });
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Freeze"));
        IceBlock iceBlock = (IceBlock)Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<IceBlock>(), 0, 0f, Projectile.owner)].ModProjectile;
        target.GetGlobalNPC<IceBlockNPC>().iceBlock = iceBlock;
        iceBlock.npc = target;
        iceBlock.Projectile.width = target.frame.Width;
        iceBlock.Projectile.height = target.frame.Height;
        iceBlock.Projectile.Bottom = target.Bottom;
    }
}
