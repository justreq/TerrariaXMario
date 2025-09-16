using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.ObjectSpawnerBlockUI;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;
internal abstract class PowerupProjectile<T> : ModProjectile, ISpawnableObject where T : Powerup, new()
{
    internal T? PowerupData = new();

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < PowerupData?.Caps.Length; i++)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{PowerupData.Caps[i]}_{EquipType.Head}", EquipType.Head, name: $"{Name}{PowerupData.Caps[i]}");
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{PowerupData.Caps[i]}_{EquipType.Body}", EquipType.Body, name: $"{Name}{PowerupData.Caps[i]}");
            EquipLoader.AddEquipTexture(Mod, $"{Texture}{PowerupData.Caps[i]}_{EquipType.Legs}", EquipType.Legs, name: $"{Name}{PowerupData.Caps[i]}");
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < PowerupData?.Caps.Length; i++)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name}{PowerupData.Caps[i]}", EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name}{PowerupData.Caps[i]}", EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name}{PowerupData.Caps[i]}", EquipType.Legs);

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

    public override void AI()
    {
        PowerupData?.UpdateWorld(Projectile);

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

                Projectile.Kill();

                if (PowerupData == null) return;

                SoundEngine.PlaySound(new(PowerupData.EquipSound) { Volume = 0.4f });
                capPlayer.currentPowerup = PowerupData;
                PowerupData.OnConsume(player);
            }
        }
    }
}
