using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaXMario.Common.GearSlots;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Common.ShowdownSystem;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal enum Jump { None, Single, Double, Triple }
internal enum ForceArmMovementType { None, Swing, Extend }
internal enum FlightState { None, Gliding, Flying }
internal class CapEffectsPlayer : ModPlayer
{
    private GearSlotPlayer? GearSlotPlayer => Player.GetModPlayerOrNull<GearSlotPlayer>();
    private ShowdownPlayer? ShowdownPlayer => Player.GetModPlayerOrNull<ShowdownPlayer>();

    private string? oldCap;
    internal string? currentCap;
    internal string? currentCapToDraw;
    internal string? currentHeadVariant;
    internal string? currentBodyVariant;
    internal string? currentLegsVariant;

    internal int? currentPowerupType;
    internal Powerup? CurrentPowerup => currentPowerupType == null ? null : PowerupLoader.Powerups[(int)currentPowerupType];

    internal bool CanDoCapEffects => currentCap != null && (GearSlotPlayer?.ShowGearSlots ?? false);

    private bool requestedPowerupRightClick;
    internal int forceDirection;
    internal int forceSwitchDirectionCount;
    internal ForceArmMovementType forceArmMovementType;
    internal int forceArmMovementDuration;
    internal int forceArmMovementTimer;

    internal bool GroundPounding
    {
        get => field;
        set
        {
            field = value;
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

    internal int fireFlowerFireballsCast;
    internal int fireFlowerCooldown;

    internal FlightState flightState;
    internal SlotId glideFlySoundSlot;
    internal SlotId pSpeedSoundSlot;

    private int glideLegAnimationTimer;
    internal int? forceLegFrameY;
    private int powerupRightClickActionTimer;

    internal bool CanShowTail => !Main.projectile.Any(e => e.type == ModContent.ProjectileType<TailSwipe>() && e.active && e.owner == Player.whoAmI);

    public override void ResetEffects()
    {
        currentCap = null;
        currentCapToDraw = null;
    }

    public override void SaveData(TagCompound tag)
    {
        if (currentPowerupType != null) tag[nameof(currentPowerupType)] = currentPowerupType;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(currentPowerupType))) currentPowerupType = tag.GetInt(nameof(currentPowerupType));
    }

    public override bool CanUseItem(Item item)
    {
        if (grabbedNPCIndex != null || currentJump is Jump.Double or Jump.Triple) return false;

        return base.CanUseItem(item);
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (!CanDoCapEffects) return;

        modifiers.DisableSound();
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!CanDoCapEffects) return;

        if (currentPowerupType != null)
        {
            currentPowerupType = null;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown") { Volume = 0.4f });
        }

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{currentCap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f });
    }

    public override void FrameEffects()
    {
        if (!CanDoCapEffects && !Main.gameMenu)
        {
            if (currentPowerupType != null) currentPowerupType = null;
            return;
        }

        int head = EquipLoader.GetEquipSlot(Mod, $"{CurrentPowerup?.Name}{currentCapToDraw}{currentHeadVariant ?? ""}", EquipType.Head);
        Player.head = head == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentHeadVariant ?? ""}", EquipType.Head) : head;

        int body = EquipLoader.GetEquipSlot(Mod, $"{CurrentPowerup?.Name}{currentCapToDraw}{currentBodyVariant ?? ""}", EquipType.Body);
        Player.body = body == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentBodyVariant ?? ""}", EquipType.Body) : body;

        int legs = EquipLoader.GetEquipSlot(Mod, $"{CurrentPowerup?.Name}{currentCapToDraw}{currentLegsVariant ?? ""}", EquipType.Legs);
        Player.legs = legs == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentLegsVariant ?? ""}", EquipType.Legs) : legs;

        if ((CurrentPowerup?.ShowTail ?? false) && CanShowTail)
        {
            Player.waist = EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}Tail", EquipType.Waist);
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (!CanDoCapEffects || Player.mount.Active)
        {
            player.legPosition.Y = 0;
            return;
        }

        if (flightState != FlightState.None && new int?[] { 10, 11, 12, 13, 17, 18, 19, 20 }.Contains(player.legFrame.Y / 56)) player.legPosition.Y = -2;
        else player.legPosition.Y = 0;
    }

    public override void PreUpdate()
    {
        if (!CanDoCapEffects || Player.mount.Active)
        {
            glideLegAnimationTimer = 0;
            forceLegFrameY = null;
            return;
        }

        GrabEffect();

        if (flightState == FlightState.Gliding)
        {
            glideLegAnimationTimer++;

            if (glideLegAnimationTimer >= 4)
            {
                forceLegFrameY ??= 7;
                forceLegFrameY = forceLegFrameY >= 19 ? 7 : forceLegFrameY + 1;

                glideLegAnimationTimer = 0;
            }
        }
        else
        {
            forceLegFrameY = null;
            glideLegAnimationTimer = 0;
        }
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

        CurrentPowerup?.UpdateConsumed(Player);
    }

    public override void PostUpdate()
    {
        CapEffect();

        if (!CanDoCapEffects || Player.mount.Active)
        {
            KillStompHitbox();
            runTime = 0;
            hasPSpeed = false;

            return;
        }

        JumpEffect();
        StompEffect();
        GrabEffectCompositeArms();
        ObjectSpawnerBlockHitEffect();

        if (currentJump is Jump.Double or Jump.Triple && !PlayerInput.Triggers.Current.Down && !Player.IsOnGroundPrecise() && flightState == FlightState.None) Player.bodyFrame.Y = 56 * 10;

        if (fireFlowerCooldown > 0) fireFlowerCooldown--;
        else if (fireFlowerFireballsCast > 0) fireFlowerFireballsCast = 0;

        if (flightState != FlightState.None && Player.IsOnGroundPrecise())
        {
            currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
            flightState = FlightState.None;
        }

        if (powerupRightClickActionTimer > 0) powerupRightClickActionTimer--;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!CanDoCapEffects) return;

        if (PlayerInput.Triggers.JustPressed.MouseLeft)
        {
            if (Main.cursorOverride == TerrariaXMario.Instance.CursorThrowIndex)
            {

                NPC grabbedNPC = Main.npc[(int)grabbedNPCIndex!];
                SetForceDirection(10, ForceArmMovementType.Swing, Math.Sign(Main.MouseWorld.X - Player.position.X));
                grabbedNPC.velocity.X = 7.5f * forceDirection;
                grabbedNPC.GetGlobalNPCOrNull<IceBlockNPC>()?.thrown = true;
                grabbedNPCIndex = null;
            }
            else if (Main.cursorOverride == TerrariaXMario.Instance.CursorGrabIndex) grabbedNPCIndex = hoverNPCIndex;
        }

        if (CurrentPowerup != null && grabbedNPCIndex == null && (!ShowdownPlayer?.isPlayerInShowdownSubworld ?? true))
        {
            if (PlayerInput.Triggers.JustPressed.MouseRight && !Player.mouseInterface)
            {
                if (CurrentPowerup.LookTowardRightClick)
                {
                    requestedPowerupRightClick = true;
                    SetForceDirection(10, CurrentPowerup.RightClickArmMovementType, Math.Sign(Main.MouseWorld.X - Player.position.X));
                }
                else if (powerupRightClickActionTimer == 0)
                {
                    CurrentPowerup.OnRightClick(Player);
                    powerupRightClickActionTimer = CurrentPowerup.RightClickActionCooldown;
                }
            }

            if (PlayerInput.Triggers.Current.Jump && !Player.mount.Active) CurrentPowerup?.OnJumpHeldDown(Player);

            if (PlayerInput.Triggers.JustReleased.Jump)
            {
                if (flightState != FlightState.None)
                {
                    glideFlySoundSlot = SlotId.Invalid;
                    flightState = FlightState.None;
                    currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
                }
            }
        }

        if (PlayerInput.Triggers.JustPressed.Down && stompHitbox != null && (!ShowdownPlayer?.isPlayerInShowdownSubworld ?? true))
        {
            StompHitbox proj = (StompHitbox)Main.projectile[(int)stompHitbox].ModProjectile;

            if (!proj.groundPound)
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPoundStart") { Volume = 0.4f });
                Player.fullRotation = 0;
                proj.groundPound = true;
            }
        }

        if (Player.wet && PlayerInput.Triggers.JustPressed.Jump) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f });
    }

    internal void SetForceDirection(int duration, ForceArmMovementType type, int direction)
    {
        if (duration <= 0 || Math.Abs(direction) != 1) return;

        forceArmMovementType = type;
        forceArmMovementTimer = duration;
        forceDirection = direction;
        if (forceSwitchDirectionCount > 0) forceSwitchDirectionCount--;
        Player.direction = direction;

        if (requestedPowerupRightClick)
        {
            if (CurrentPowerup != null && powerupRightClickActionTimer == 0)
            {
                CurrentPowerup.OnRightClick(Player);
                powerupRightClickActionTimer = CurrentPowerup.RightClickActionCooldown;
            }

            requestedPowerupRightClick = false;
        }
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
            forceSwitchDirectionCount = 0;
            forceArmMovementType = ForceArmMovementType.None;
            forceArmMovementDuration = 0;
            forceArmMovementTimer = 0;
            return;
        }

        if (forceArmMovementTimer > 0)
        {
            if (forceArmMovementDuration == 0) forceArmMovementDuration = forceArmMovementTimer;

            if (forceArmMovementType != ForceArmMovementType.None)
            {
                int i = Math.Clamp(Math.Max(1, 4 - (int)Math.Floor((double)forceArmMovementTimer / forceArmMovementDuration * 4)), 1, 4);

                if (forceArmMovementType == ForceArmMovementType.Swing) Player.bodyFrame.Y = 56 * i;
                else if (forceArmMovementType == ForceArmMovementType.Extend) Player.SetCompositeArmFront(true, (Player.CompositeArmStretchAmount)(i == 4 ? 0 : i), Player.Center.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2);
            }

            forceArmMovementTimer--;

            if (forceArmMovementTimer <= 0)
            {
                if (forceSwitchDirectionCount > 0) SetForceDirection(forceArmMovementDuration, forceArmMovementType, -forceDirection);
                else
                {
                    forceDirection = 0;
                    forceArmMovementType = ForceArmMovementType.None;
                    forceArmMovementDuration = 0;
                    forceArmMovementTimer = 0;
                }
            }
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
            if (Player.fullRotation != 0 && !GroundPounding) Player.fullRotation = 0;
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
        if (flightState == FlightState.Flying) return;

        if ((runTime != 0 && !Player.controlLeft && !Player.controlRight) || (Player.controlLeft && Player.direction == 1) || (Player.controlRight && Player.direction == -1) || (ShowdownPlayer?.isPlayerInShowdownSubworld ?? false)) runTime = 0;
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f && runTime < runTimeRequiredForPSpeed) runTime++;

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
            if (!SoundEngine.TryGetActiveSound(pSpeedSoundSlot, out var pSpeedSound))
            {
                pSpeedSoundSlot = SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/PSpeed") { Volume = 0.4f });
            }
        }
        else if (hasPSpeed)
        {
            hasPSpeed = false;
            pSpeedSoundSlot = SlotId.Invalid;
        }
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
            Projectile.NewProjectile(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8), new Vector2(0, spawnFromBottom ? projectile.SpawnDownSpeed : projectile.SpawnUpSpeed), projectile.Type, 0, 0);
        }
        else if (objectToSpawn is ModItem item)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/ItemSpawn") { Volume = 0.4f });
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), entity.Position.ToWorldCoordinates() + new Vector2(8, 24 * (spawnFromBottom ? 1 : -1)), new Item(item.Type), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }
        else if (objectToSpawn is DefaultSpawnableObject)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Coin") { Volume = 4f });
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
