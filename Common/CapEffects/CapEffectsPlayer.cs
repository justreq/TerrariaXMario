using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal enum Jump { None, Single, Double, Triple }
internal class CapEffectsPlayer : ModPlayer
{
    private GearSlotPlayer? GearSlotPlayer => Player.GetModPlayerOrNull<GearSlotPlayer>();

    private string? oldCap;
    internal string? currentCap;
    internal string? currentHeadVariant;
    internal string? currentBodyVariant;
    internal string? currentLegsVariant;

    internal Powerup? currentPowerup;

    internal bool CanDoCapEffects => currentCap != null && (GearSlotPlayer?.ShowGearSlots ?? false);

    internal int forceDirection;
    internal int forceSwingDuration;
    internal int forceSwingTimer;

    private bool groundPounding;
    internal bool GroundPounding
    {
        get => groundPounding;
        set
        {
            groundPounding = value;
            currentLegsVariant = value ? "GroundPound" : null;
        }
    }

    private int jumpInputBuffer;
    private int jumpFlipTimer;
    private int jumpFlipDuration;
    private bool backflip;

    internal Jump currentJump;

    int? stompHitbox;

    internal int runTime;
    internal int runTimeRequiredForPSpeed = 120;
    internal bool hasPSpeed;

    internal int? hoverNPCIndex;
    internal int? grabbedNPCIndex;

    internal Vector2 currentObjectSpawnerBlockToEdit;

    public override void ResetEffects()
    {
        currentCap = null;
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (!CanDoCapEffects) return;

        modifiers.DisableSound();
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!CanDoCapEffects) return;

        if (currentPowerup != null)
        {
            currentPowerup = null;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown") { Volume = 0.4f });
        }

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{currentCap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f });
    }

    public override void FrameEffects()
    {
        if (!CanDoCapEffects)
        {
            if (currentPowerup != null) currentPowerup = null;
            return;
        }

        Player.head = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}{currentHeadVariant ?? ""}", EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}{currentBodyVariant ?? ""}", EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, $"{currentPowerup?.Name ?? ""}{currentCap}{currentLegsVariant ?? ""}", EquipType.Legs);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (player.mount.Active || !CanDoCapEffects)
        {
            player.headPosition = Vector2.Zero;
            player.headRotation = 0;
            return;
        }

        if (!player.controlDown || player.IsOnGroundPrecise()) player.headPosition.X = 0;

        if (currentCap == "Luigi")
        {
            player.bodyPosition.Y = -2;

            if (player.GetModPlayerOrNull<CapEffectsPlayer>()?.groundPounding ?? false)
            {
                player.legPosition.Y = -2;
            }
        }
    }

    public override void PreUpdate()
    {
        if (!CanDoCapEffects || Player.mount.Active) return;

        GrabEffect();
    }

    public override void PostUpdateRunSpeeds()
    {
        if (!CanDoCapEffects) return;

        if (currentCap == "Luigi") Player.maxRunSpeed *= 1.05f;

        PSpeedEffect();
    }

    public override void PostUpdateEquips()
    {
        if (!CanDoCapEffects)
        {
            if (currentObjectSpawnerBlockToEdit != Vector2.Zero) currentObjectSpawnerBlockToEdit = Vector2.Zero;
            return;
        }

        if (currentJump == Jump.Double) Player.jumpSpeedBoost += 1.25f;
        else if (currentJump == Jump.Triple) Player.jumpSpeedBoost += 2.5f;

        currentPowerup?.UpdateConsumed(Player);
    }

    public override void PostUpdate()
    {
        if (!CanDoCapEffects || Player.mount.Active)
        {
            KillStompHitbox();
            runTime = 0;
            hasPSpeed = false;

            return;
        }

        CapEffect();
        JumpEffect();
        StompEffect();
        GrabEffectCompositeArms();
        ObjectSpawnerBlockHitEffect();

        if (currentJump is Jump.Double or Jump.Triple && !PlayerInput.Triggers.Current.Down && !Player.IsOnGroundPrecise()) Player.bodyFrame.Y = 56 * 10;
    }

    internal void SetForceDirection(int duration, int direction)
    {
        if (duration <= 0 || Math.Abs(direction) != 1) return;

        forceSwingTimer = duration;
        forceDirection = direction;
        Player.direction = direction;
    }

    private void CapEffect()
    {
        if ((GearSlotPlayer?.ShowGearSlots ?? false) && oldCap != currentCap)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{currentCap ?? oldCap}{(currentCap == null ? "Unequip" : "Equip")}") { Volume = 0.4f });
            oldCap = currentCap;
        }

        if (!CanDoCapEffects)
        {
            forceDirection = 0;
            forceSwingDuration = 0;
            forceSwingTimer = 0;
            return;
        }

        if (Player.wet && PlayerInput.Triggers.JustPressed.Jump || Player.justJumped)
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f });

        if (forceSwingTimer > 0)
        {
            if (forceSwingDuration == 0) forceSwingDuration = forceSwingTimer;

            Player.bodyFrame.Y = 56 * Math.Clamp(Math.Max(1, 4 - (int)Math.Floor((double)forceSwingTimer / forceSwingDuration * 4)), 1, 4);

            forceSwingTimer--;

            if (forceSwingTimer <= 0)
            {
                forceDirection = 0;
                forceSwingDuration = 0;
                forceSwingTimer = 0;
            }
        }

        if (PlayerInput.Triggers.JustPressed.MouseLeft && !Player.mouseInterface && Main.cursorOverride != TerrariaXMario.Instance.CursorGrabIndex && Main.cursorOverride != TerrariaXMario.Instance.CursorThrowIndex && Player.HeldItem.IsAir && Main.mouseItem.IsAir && (!Player.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? true))
        {
            if (currentPowerup?.OnLeftClick(Player) ?? false) SetForceDirection(10, Math.Sign(Main.MouseWorld.X - Player.position.X));
        }
    }

    private void JumpEffect()
    {
        if (jumpInputBuffer > 0 && !Player.IsOnGroundPrecise()) jumpInputBuffer = 0;

        if (jumpInputBuffer > 10 || Player.sliding)
        {
            currentJump = Jump.None;
            jumpInputBuffer = 0;
        }

        if ((currentJump is Jump.Single or Jump.Double) && Player.IsOnGroundPrecise())
        {
            jumpInputBuffer++;

            if (!Player.controlLeft && !Player.controlRight)
            {
                currentJump = Jump.None;
                jumpInputBuffer = 0;
            }
        }

        if (currentJump == Jump.Triple && Player.IsOnGroundPrecise()) currentJump = Jump.None;

        if (jumpFlipTimer > 0)
        {
            if (Player.IsOnGroundPrecise() || currentJump == Jump.None)
            {
                jumpFlipTimer = 0;
                jumpFlipDuration = 0;
                backflip = false;
                return;
            }

            if (jumpFlipDuration == 0) jumpFlipDuration = jumpFlipTimer;
            jumpFlipTimer--;

            Player.fullRotationOrigin = Player.Size * 0.5f;
            Player.fullRotation = (jumpFlipTimer / (float)jumpFlipDuration) * MathHelper.TwoPi * -Player.direction * (backflip ? -1 : 1);
        }
        else
        {
            if (jumpFlipDuration != 0) jumpFlipDuration = 0;
            if (Player.fullRotation != 0) Player.fullRotation = 0;
        }

        if (Player.justJumped)
        {
            currentJump = currentJump == Jump.Triple ? Jump.Single : currentJump + 1;

            if (currentJump == Jump.Triple)
            {
                jumpFlipTimer = 45;
                backflip = false;
            }

            if (currentJump is not (Jump.None or Jump.Single) && (int)currentJump < Enum.GetNames(typeof(Jump)).Length) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{currentCap}{currentJump}Jump") { Volume = 0.4f });
        }
    }

    private void StompEffect()
    {
        if (!Player.IsOnGroundPrecise())
        {
            stompHitbox ??= Projectile.NewProjectile(Player.GetSource_FromThis(), Player.BottomLeft, Vector2.Zero, ModContent.ProjectileType<StompHitbox>(), 1, 4f, Player.whoAmI);
        }
        else KillStompHitbox();
    }

    internal void KillStompHitbox()
    {
        if (stompHitbox != null)
        {
            Main.projectile[(int)stompHitbox].Kill();
            stompHitbox = null;
        }
    }

    private void PSpeedEffect()
    {
        if ((runTime != 0 && !Player.controlLeft && !Player.controlRight) || (Player.GetModPlayerOrNull<ShowdownPlayer>()?.isPlayerInShowdownSubworld ?? false)) runTime = 0;
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f) runTime++;

        if (runTime >= runTimeRequiredForPSpeed)
        {
            if (!hasPSpeed)
            {
                hasPSpeed = true;
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/FastRunStart") { Volume = 0.4f });

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Player.Center, DustID.Cloud, Scale: 1.5f);
                }
            }

            Player.accRunSpeed *= currentCap == "Luigi" ? 1.5f : 1.25f;
        }
        else if (hasPSpeed) hasPSpeed = false;
    }

    private void GrabEffect()
    {
        if (grabbedNPCIndex != null)
        {
            NPC grabbedNPC = Main.npc[(int)grabbedNPCIndex];
            IceBlockNPC? globalNPC = grabbedNPC.GetGlobalNPCOrNull<IceBlockNPC>();

            if (!globalNPC?.frozen ?? true)
            {
                grabbedNPCIndex = null;
                return;
            }

            Main.cursorOverride = TerrariaXMario.Instance.CursorThrowIndex;

            grabbedNPC.Bottom = Player.Top;

            if (PlayerInput.Triggers.JustPressed.MouseLeft)
            {
                SetForceDirection(10, Math.Sign(Main.MouseWorld.X - Player.position.X));
                grabbedNPC.velocity.X = 7.5f * forceDirection;
                globalNPC?.thrown = true;
                grabbedNPCIndex = null;
            }
        }
        else if (hoverNPCIndex != null)
        {
            NPC hoverNPC = Main.npc[(int)hoverNPCIndex];
            IceBlockNPC? globalNPC = hoverNPC.GetGlobalNPCOrNull<IceBlockNPC>();

            if (globalNPC == null) return;

            if (!globalNPC.frozen || globalNPC.thrown || !globalNPC.iceBlockRect.Intersects(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 1, 1)))
            {
                hoverNPCIndex = null;
                return;
            }

            Main.cursorOverride = TerrariaXMario.Instance.CursorGrabIndex;

            if (PlayerInput.Triggers.JustPressed.MouseLeft) grabbedNPCIndex = hoverNPCIndex;
        }
    }

    private void GrabEffectCompositeArms()
    {
        if (grabbedNPCIndex != null)
        {
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
        }
    }

    private void ObjectSpawnerBlockHitEffect()
    {
        if (Player.velocity.Y >= 0 || Player.IsOnGroundPrecise()) return;

        foreach (Point point in HitObjectSpawnerBlocks(1))
        {
            SpawnObjectFromBlock(point);
        }
    }

    internal Point[] HitObjectSpawnerBlocks(float yOffset = 0f, bool checkBottom = false)
    {
        static Point[] HitObjectSpawnerBlocks(in float startX, float y, int width)
        {
            if (width <= 0)
            {
                throw new ArgumentException("width cannot be negative");
            }

            float fx = startX;

            IEnumerable<Point> blocks = [];

            //Needs atleast one iteration (in case width is 0)
            do
            {
                Point point = new Vector2(fx, y + 0.01f).ToTileCoordinates(); //0.01f is a magic number vanilla uses
                if (TerrariaXMario.SolidTile(point.X, point.Y) && TileLoader.GetTile(Framing.GetTileSafely(point).TileType) is ObjectSpawnerBlockTile)
                {
                    blocks = blocks.Append(point);
                }
                fx += 16;
            }
            while (fx < startX + width);

            return [.. blocks];
        }

        return HitObjectSpawnerBlocks(checkBottom ? Player.BottomLeft.X : Player.TopLeft.X, checkBottom ? Player.BottomLeft.Y + yOffset : Player.TopLeft.Y - yOffset, Player.width);
    }

    internal void SpawnObjectFromBlock(Point? point, bool spawnFromBottom = false)
    {
        if (point == null) return;

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/BlockHit") { Volume = 8 });

        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull((Point)point);
        if (entity == null || entity.justStruck || entity.wasPreviouslyStruck) return;

        if (entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(BrickBlockTile))
        {
            WorldGen.KillTile(point.Value.X, point.Value.Y);
            return;
        }

        ISpawnableObject objectToSpawn = entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(QuestionBlockTile) ? new DefaultSpawnableObject() : entity!.spawnContents[0];

        if (objectToSpawn is PowerupProjectile projectile)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            Projectile.NewProjectile(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8), new Vector2(0, spawnFromBottom ? projectile.PowerupData.SpawnDownSpeed : projectile.PowerupData.SpawnUpSpeed), projectile.Type, 0, 0);
        }
        else if (objectToSpawn is ModItem item)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"));
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, 24 * (spawnFromBottom ? 1 : -1)), new Item(item.Type), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }
        else if (objectToSpawn is DefaultSpawnableObject)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Coin") { Volume = 3f });
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, 24 * (spawnFromBottom ? 1 : -1)), new Item(ItemID.GoldCoin), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }

        entity?.spawnContents = [.. entity.spawnContents.Skip(1).ToArray()];
        entity?.justStruck = true;
        if (entity?.spawnContents.Length == 0) entity.wasPreviouslyStruck = true;
        Player.velocity.Y = 1;
    }
}
