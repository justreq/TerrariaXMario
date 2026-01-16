using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal abstract class PowerupProjectile : ModProjectile, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; }
    internal virtual int? PowerupType => null;
    internal Powerup? Powerup => PowerupType == null ? null : PowerupLoader.Powerups[(int)PowerupType];
    /// <summary>
    /// A list of the caps that can consume this PowerupData.
    /// </summary>
    internal virtual string[] Caps => [];
    /// <summary>
    /// A list of texture variations this PowerupData should use
    /// </summary>
    internal virtual string[] Variations => [];
    /// <summary>
    /// The conditions that must be met for this Powerup to be considered in the object spawner loot pool
    /// </summary>
    /// <param name="player">The player that hit the block</param>
    internal virtual bool CanSpawn(Player player) => false;
    internal virtual float SpawnUpSpeed => -0.61f;
    internal virtual float SpawnDownSpeed => 0.61f;
    internal virtual int TimeBeforePickable => 45;
    internal virtual bool Head => true;
    internal virtual bool Body => true;
    internal virtual bool Legs => true;
    internal virtual bool GroundPound => true;

    private int updateCount;

    private void LoadEquipTextures(string cap, string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Head}", EquipType.Head, name: $"{Name}{cap}{variation}");
        if (body) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Body}", EquipType.Body, name: $"{Name}{cap}{variation}");
        if (legs) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Legs}", EquipType.Legs, name: $"{Name}{cap}{variation}");
    }

    private void SetupEquipTextures(string cap, string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name}{cap}{variation}", EquipType.Head);
            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
        if (body)
        {
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name}{cap}{variation}", EquipType.Body);
            if (equipSlotBody != -1)
            {
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            }
        }
        if (legs)
        {
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name}{cap}{variation}", EquipType.Legs);
            if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            string cap = Caps[i];

            LoadEquipTextures(cap, head: Head, body: Body, legs: Legs);
            if (GroundPound && Legs) LoadEquipTextures(cap, "GroundPound", false, false);

            for (int j = 0; j < Variations.Length; j++)
            {
                LoadEquipTextures(cap, Variations[j], head: Head, body: Body, legs: Legs);
            }
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server) return;

        for (int i = 0; i < Caps.Length; i++)
        {
            string cap = Caps[i];

            SetupEquipTextures(cap);
            if (GroundPound && Legs) SetupEquipTextures(cap, "GroundPound", false, false);

            for (int j = 0; j < Variations.Length; j++)
            {
                SetupEquipTextures(cap, Variations[j], head: Head, body: Body, legs: Legs);
            }
        }
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hide = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        if (PowerupType == null)
        {
            Projectile.Kill();
            return;
        }

        if (updateCount > TimeBeforePickable) Powerup!.UpdateWorld(Projectile, updateCount);
        updateCount++;

        foreach (Player player in Main.ActivePlayers)
        {
            if (!Projectile.Hitbox.Intersects(player.Hitbox)) continue;

            if (!player.immune)
            {
                player.immuneTime = 30;
                player.immune = true;
            }

            Projectile.Kill();

            SoundEngine.PlaySound(new(Powerup!.EquipSound) { Volume = 0.4f }, player.MountedCenter);
            player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentPowerupType = PowerupType;
            Powerup!.OnConsume(player);
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);

        if (updateCount > TimeBeforePickable && behindNPCsAndTiles.Contains(index))
        {
            Projectile.hide = false;
            behindNPCsAndTiles.Remove(index);
        }
    }
}
