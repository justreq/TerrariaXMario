using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using TerrariaXMario.Common.MiscEffects;

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

        SoundEngine.PlaySound(new($"{GetType().FullName!.Replace(".", "/")}Kill") { Volume = 0.4f }, Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Freeze") { Volume = 0.4f }, target.Center);
        target.GetGlobalNPC<IceBlockNPC>().frozen = true;
    }
}
