using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Powerups;

internal abstract class PowerupProjectile : ModProjectile, ISpawnableObject
{
    public override string Texture => base.Texture.Replace("Projectile", "");

    SpawnRarity ISpawnableObject.SpawnRarity { get; set; }
    internal virtual int? PowerupType => null;
    internal Powerup? PowerupData => PowerupType == null ? null : PowerupLoader.Powerups[(int)PowerupType];
    /// <summary>
    /// A list of the caps that can consume this Powerup.
    /// </summary>
    internal virtual string[] Caps => [];
    /// <summary>
    /// A list of texture variations this Powerup should use
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

    private Vector2 bubbleDirection;

    private void LoadEquipTextures(string cap, string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Head}", EquipType.Head, name: $"{Name.Replace("Projectile", "")}{cap}{variation}");
        if (body) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Body}", EquipType.Body, name: $"{Name.Replace("Projectile", "")}{cap}{variation}");
        if (legs) EquipLoader.AddEquipTexture(Mod, $"{Texture}{cap}{variation}_{EquipType.Legs}", EquipType.Legs, name: $"{Name.Replace("Projectile", "")}{cap}{variation}");
    }

    private void SetupEquipTextures(string cap, string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Head);
            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
        if (body)
        {
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Body);
            if (equipSlotBody != -1)
            {
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            }
        }
        if (legs)
        {
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name.Replace("Projectile", "")}{cap}{variation}", EquipType.Legs);
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
        bubbleDirection = Main.rand.NextVector2Circular(1, 1) * 0.25f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    public override void AI()
    {
        if (PowerupType == null)
        {
            Projectile.Kill();
            return;
        }

        foreach (Player player in Main.ActivePlayers)
        {
            CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

            if (!Projectile.Hitbox.Intersects(player.Hitbox) || (!modPlayer?.CanDoCapEffects ?? true)) continue;

            player.SetImmuneTimeForAllTypes(30);

            if (PowerupData != null && PowerupData.ItemType != null && modPlayer?.CurrentPowerup != null)
            {
                player.QuickSpawnItem(Projectile.GetSource_FromThis(), modPlayer?.currentPowerupType == PowerupType ? (int)PowerupData.ItemType : (int)modPlayer!.CurrentPowerup!.ItemType!);
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupReserve") { Volume = 0.4f }, player.MountedCenter);
            }

            SoundEngine.PlaySound(new(PowerupData!.EquipSound) { Volume = 0.4f }, player.MountedCenter);
            player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentPowerupType = PowerupType;
            PowerupData!.OnConsume(player);
            Projectile.Kill();
        }

        if (Projectile.ai[0] == 1)
        {
            if (Main.GameUpdateCount % Main.rand.Next(20, 60) == 0) bubbleDirection = Main.rand.NextVector2Circular(1, 1) * 0.25f;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, bubbleDirection.X, 0.15f);
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, bubbleDirection.Y, 0.15f);
        }
        else
        {
            if (updateCount > TimeBeforePickable) PowerupData!.UpdateWorld(Projectile, updateCount);
            updateCount++;
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (Projectile.ai[0] == 1 || (updateCount > TimeBeforePickable && behindNPCsAndTiles.Contains(index)))
        {
            Projectile.hide = false;
            behindNPCsAndTiles.Remove(index);
        }
        else if (!behindNPCsAndTiles.Contains(index))
        {
            Projectile.hide = true;
            behindNPCsAndTiles.Add(index);
        }
    }

    Vector2 scale = Vector2.One;

    public override void PostDraw(Color lightColor)
    {
        if (Projectile.ai[0] != 1) return;

        bool big = Projectile.width > 32 || Projectile.height > 32;

        if (big) Main.instance.LoadGore(415);
        else Main.instance.LoadGore(414);

        //scale.X = Math.Clamp(Math.Abs(Projectile.velocity.X) * 8, 1, 1.5f);
        //scale.Y = Math.Clamp(Math.Abs(Projectile.velocity.Y) * 8, 1, 1.5f);

        scale.X = MathHelper.Lerp(scale.X, 0.05f * (float)Math.Sin(Main.GameUpdateCount * 0.05f) + 1.05f, 0.15f);
        scale.Y = MathHelper.Lerp(scale.Y, 0.05f * (float)Math.Sin(Main.GameUpdateCount * 0.05f) + 1.05f, 0.15f);

        Main.EntitySpriteDraw(new(TextureAssets.Gore[414].Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(big ? 32 : 24), scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None));
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[0] != 1) return;

        for (int i = 0; i < Main.rand.Next(3, 7); i++)
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_Blue);
            Main.dust[dust].scale = Main.rand.Next(2, 4);
            Main.dust[dust].noGravity = true;
        }
    }
}
