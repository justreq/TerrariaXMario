using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;
internal abstract class PowerupProjectile : ModProjectile
{
    internal virtual string[] Caps => [];

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{Caps[i]}_{EquipType.Head}", EquipType.Head, name: $"{Name}{Caps[i]}");
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{Caps[i]}_{EquipType.Body}", EquipType.Body, name: $"{Name}{Caps[i]}");
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{Caps[i]}_{EquipType.Legs}", EquipType.Legs, name: $"{Name}{Caps[i]}");
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name}{Caps[i]}", EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name}{Caps[i]}", EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name}{Caps[i]}", EquipType.Legs);

            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            if (equipSlotBody != -1)
            {
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            }
            if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void PostAI()
    {
        foreach (Player player in Main.ActivePlayers)
        {
            if (!Projectile.Hitbox.Intersects(player.Hitbox)) continue;

            CapPlayer? capPlayer = player.GetModPlayerOrNull<CapPlayer>();

            if (capPlayer?.CanDoCapEffects ?? false)
            {
                if (!player.immune)
                {
                    player.immuneTime = 30;
                    player.immune = true;
                }

                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp") { Volume = 0.4f });
                Projectile.Kill();
                capPlayer.powerup = Name;
            }
        }
    }
}
