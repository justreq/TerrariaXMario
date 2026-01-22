using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using TerrariaXMario.Common.BroInfoUI;
using TerrariaXMario.Common.CapeFlight;
using TerrariaXMario.Common.FrogSwim;
using TerrariaXMario.Common.MiscEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Common.StatueForm;
using TerrariaXMario.Content.Blocks;
using TerrariaXMario.Content.Caps;
using TerrariaXMario.Content.Consumables;
using TerrariaXMario.Content.Materials;
using TerrariaXMario.Content.PowerupProjectiles;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;

internal enum Jump { None, Single, Double, Triple }
internal enum ForceArmMovementType { None, Swing, Extend }
internal enum FlightState { None, Gliding, Flying }
internal class CapEffectsPlayer : ModPlayer
{
    internal BroInfoPlayer? BroInfoPlayer => Player.GetModPlayerOrNull<BroInfoPlayer>();

    private string? oldCap;
    internal string? currentCap;
    internal string? currentCapToDraw;
    internal string? currentHeadVariant;
    internal string? currentBodyVariant;
    internal string? currentLegsVariant;

    internal string?[] CurrentVariants => [.. new string?[] { currentHeadVariant, currentBodyVariant, currentLegsVariant }.Where(e => e != null).Distinct()];

    internal int? currentPowerupType;
    internal Powerup? CurrentPowerup => currentPowerupType == null ? null : PowerupLoader.Powerups[(int)currentPowerupType];

    internal bool CanDoCapEffects => currentCap != null && (BroInfoPlayer?.ShowBroInfo ?? false);

    private bool requestedPowerupRightClick;
    internal int forceDirection;
    internal int forceSwitchDirectionCount;
    internal ForceArmMovementType forceArmMovementType;
    internal int forceArmMovementDuration;
    internal int forceArmMovementTimer;

    internal bool GroundPounding
    {
        get; set
        {
            field = value;
            currentLegsVariant = value ? "GroundPound" : currentLegsVariant == "GroundPound" ? null : currentLegsVariant;
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

    internal int fireFlowerFireballsCast;
    internal int fireFlowerCooldown;

    internal FlightState flightState;
    internal SlotId glideFlySoundSlot;
    internal SlotId pSpeedSoundSlot;

    private int glideLegAnimationTimer;
    internal int? forceLegFrameY;
    private int powerupRightClickActionTimer;

    internal bool CanShowTail => !Main.projectile.Any(e => e.type == ModContent.ProjectileType<TailSwipe>() && e.active && e.owner == Player.whoAmI);
    internal bool CanShowCape => !Main.projectile.Any(e => e.type == ModContent.ProjectileType<CapeSpin>() && e.active && e.owner == Player.whoAmI);

    internal bool statueForm;
    private int statueFormCooldown;
    private int directionWhenTurnedToStatue = 1;

    internal int capeRiseToFlightTimer = 0;
    internal bool doCapeFlight;
    internal int capeBoostFactor;
    internal int capeBoostTimer;
    internal int capeFrameTimer;
    internal int CapeFrame
    {
        get; set
        {
            if (doCapeFlight && field != value)
            {
                int offset = value - field;

                if (((PlayerInput.Triggers.Current.Left && Player.direction == 1) || (PlayerInput.Triggers.Current.Right && Player.direction == -1)) && offset == -1 && field > 2 && capeBoostFactor == 0)
                {
                    capeBoostFactor = field - 2;
                    SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/CapeRise") { Volume = 0.4f }, Player.MountedCenter);
                }

                if (value == 0 && capeBoostFactor != 0)
                {
                    Player.velocity.Y = -3 * capeBoostFactor;
                    capeBoostTimer = 30;
                }
            }

            field = value;
        }
    }

    internal int maxSP = 20;

    internal int StatPower { get; set { field = Math.Max(value, 0); } } = 5;
    internal int StatDefense { get; set { field = Math.Max(value, 0); } } = 0;
    internal int StatHP { get; set { field = Math.Max(value, 0); } } = 0;
    internal int StatSP { get; set { field = Math.Clamp(value, 0, maxSP); } } = 20;

    private int statPowerToAdd;
    private int statDefenseToAdd;
    private int statHPToAdd;
    private int statSPToAdd;

    internal bool frogSwimming;
    internal int frogSwimFrame;

    internal float forceFullRotation;
    internal float forceHeadRotation;

    internal int powerupChargeMax = 100;
    internal int PowerupCharge { get; set { field = Math.Clamp(value, 0, powerupChargeMax); } }
    internal float XPNeededToLevelUp => 20 * (float)Math.Pow(1.5f, Level);

    internal float XP
    {
        get; set
        {
            if (value < XPNeededToLevelUp)
            {
                field = value;
                return;
            }

            while (value >= XPNeededToLevelUp)
            {
                field = value - XPNeededToLevelUp;
                Level++;
            }
        }
    }

    internal int Level
    {
        get; private set
        {
            if (value > field)
            {
                IsLevellingUpForFlagAndTextEffects = true;
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/LevelUp") { Volume = 0.4f }, Player.MountedCenter);
                for (int i = 0; i < Math.Abs(value - field); i++)
                {
                    Player.QuickSpawnItem(Player.GetSource_Misc("Level Up"), ModContent.ItemType<StemBean>(), Main.rand.Next(1, 6));
                    statPowerToAdd += Main.rand.Next(1, 6);
                    statDefenseToAdd += Main.rand.Next(1, 6);
                    statHPToAdd += Main.rand.Next(1, 6);
                    statSPToAdd += Main.rand.Next(1, 6);
                }
            }

            field = value;
        }
    }

    internal int groundPoundJumpTimer;

    private float levelFlagPositionPercent = 0;
    internal bool IsLevellingUpForFlagAndTextEffects { get; private set; }
    internal float LevelUpTimer { get; private set; } = 60;
    private float levelFlagOpacity;

    internal bool shouldRemovePowerup;

    internal static void RestoreSP(Player player, int amount, bool effectOnly = false)
    {
        if (!effectOnly) player.GetModPlayerOrNull<CapEffectsPlayer>()?.StatSP += amount;
        CombatText.NewText(player.getRect(), Color.Orange, amount);
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/HealSP") { Volume = 0.4f }, player.MountedCenter);
    }

    /// <summary>
    /// Increase the value of a stat by a given amount
    /// </summary>
    /// <param name="stat">0 = SP, 1 = HP, 2 = DEF, 3 = POW</param>
    internal static void IncreaseMaxStat(Player player, int stat, int value)
    {
        if (stat < 0 || stat > 3) return;

        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        Color color = new Color[] { new(98, 230, 32), new(246, 74, 246), new(172, 197, 213), new(255, 115, 8) }[stat];
        string statString = new string[] { "SP", "HP", "DEF", "POW" }[stat];
        string text = $"{(value < 0 ? "" : "+")}{value} {statString}";
        CombatText.NewText(player.getRect(), color, text);
        switch (stat)
        {
            case 0:
                modPlayer?.maxSP += value;
                break;
            case 1:
                modPlayer?.StatHP += value;
                break;
            case 2:
                modPlayer?.StatDefense += value;
                break;
            case 3:
                modPlayer?.StatPower += value;
                break;
            default:
                break;
        }
    }

    public override void ResetEffects()
    {
        currentCap = null;
        currentCapToDraw = null;
    }

    public override void SaveData(TagCompound tag)
    {
        if (currentPowerupType != null) tag[nameof(currentPowerupType)] = currentPowerupType;
        tag[nameof(maxSP)] = maxSP;
        tag[nameof(StatPower)] = StatPower;
        tag[nameof(StatDefense)] = StatDefense;
        tag[nameof(StatHP)] = StatHP;
        tag[nameof(StatSP)] = StatSP;
        tag[nameof(XP)] = XP;
        tag[nameof(Level)] = Level;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(currentPowerupType))) currentPowerupType = tag.GetInt(nameof(currentPowerupType));
        //if (tag.ContainsKey(nameof(maxSP))) maxSP = tag.GetInt(nameof(maxSP));
        //if (tag.ContainsKey(nameof(StatPower))) StatPower = tag.GetInt(nameof(StatPower));
        //if (tag.ContainsKey(nameof(StatDefense))) StatDefense = tag.GetInt(nameof(StatDefense));
        //if (tag.ContainsKey(nameof(StatHP))) StatHP = tag.GetInt(nameof(StatHP));
        //if (tag.ContainsKey(nameof(StatSP))) StatSP = tag.GetInt(nameof(StatSP));
        //if (tag.ContainsKey(nameof(XP))) XP = tag.GetFloat(nameof(XP));
        //if (tag.ContainsKey(nameof(Level))) Level = tag.GetInt(nameof(Level));
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

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PlayerOverrides/{currentCap}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.4f }, Player.MountedCenter);
    }

    public override void FrameEffects()
    {
        if (currentCap == null && (BroInfoPlayer?.ShowBroInfo ?? false))
        {
            Player.head = Player.body = Player.legs = -1;
        }

        if (!CanDoCapEffects && !Main.gameMenu)
        {
            currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
            return;
        }

        if (Main.gameMenu && (!BroInfoPlayer?.ShowBroInfo ?? true)) return;

        if (statueForm) currentHeadVariant = "Statue";
        else if (currentHeadVariant == "Statue") currentHeadVariant = null;

        if (frogSwimming) currentHeadVariant = "FrogSwimming";
        else if (currentHeadVariant == "FrogSwimming") currentHeadVariant = null;

        int head = EquipLoader.GetEquipSlot(Mod, $"{(statueForm || frogSwimming ? "" : CurrentPowerup?.Name)}{currentCapToDraw}{currentHeadVariant ?? ""}", EquipType.Head);
        Player.head = head == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentHeadVariant ?? ""}", EquipType.Head) : head;

        if (statueForm || frogSwimming) return;

        int body = EquipLoader.GetEquipSlot(Mod, $"{CurrentPowerup?.Name}{currentCapToDraw}{currentBodyVariant ?? ""}", EquipType.Body);
        Player.body = body == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentBodyVariant ?? ""}", EquipType.Body) : body;

        int legs = EquipLoader.GetEquipSlot(Mod, $"{CurrentPowerup?.Name}{currentCapToDraw}{currentLegsVariant ?? ""}", EquipType.Legs);
        Player.legs = legs == -1 ? EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}{currentLegsVariant ?? ""}", EquipType.Legs) : legs;

        if (CurrentPowerup != null && PowerupID.Sets.ShowTail[CurrentPowerup.Type] && CanShowTail)
        {
            Player.waist = EquipLoader.GetEquipSlot(Mod, $"{currentCapToDraw}Tail{(flightState == FlightState.Flying ? "Flying" : "")}", EquipType.Waist);
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (!CanDoCapEffects || Player.mount.Active) return;

        if (flightState != FlightState.None && !Player.IsOnGroundPrecise() && new int?[] { 10, 11, 12, 13, 17, 18, 19, 20 }.Contains(player.legFrame.Y / 56)) player.legPosition.Y = -2;

        if (frogSwimming)
        {
            player.bodyFrame.Y = 0;
            if (!drawInfo.headOnlyRender) drawInfo.hideEntirePlayer = true;
        }

        if (forceHeadRotation != 0) player.headRotation = forceHeadRotation;
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

        if (flightState == FlightState.Gliding /*&& CurrentPowerup is not CapeFeatherData*/)
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


        if (statueForm) Player.fullRotation = 0;

        if (shouldRemovePowerup && flightState == FlightState.Flying) flightState = FlightState.Gliding;
    }

    public override void PostUpdateRunSpeeds()
    {
        if (!CanDoCapEffects) return;

        if (currentCap == "Luigi") Player.maxRunSpeed *= 1.05f;

        if (currentPowerupType == ModContent.GetInstance<FrogSuit>().Type && !frogSwimming && !CurrentVariants.Contains("Running") && new int[] { 12, 18 }.Contains(Player.legFrame.Y / 56))
        {
            Player.maxRunSpeed *= 0.25f;
            Player.velocity.X = 0.4f * Player.direction;
        }

        PSpeedEffect();
    }

    public override void PostUpdateEquips()
    {
        if (!CanDoCapEffects) return;

        if (groundPoundJumpTimer > 0)
        {
            Player.jumpSpeedBoost += 1.25f;
            Player.jumpSpeed += 0.5f;
        }
        else if (currentJump == Jump.Double) Player.jumpSpeedBoost += 1.25f;
        else if (currentJump == Jump.Triple) Player.jumpSpeedBoost += 2.5f;
    }

    public override void PostUpdate()
    {
        CapEffect();
        ObjectSpawnerBlockHitEffect();

        if (currentPowerupType != null && currentPowerupType != ModContent.GetInstance<FrogSuit>().Type) frogSwimming = false;

        if (!CanDoCapEffects || Player.mount.Active)
        {
            KillStompHitbox();
            runTime = 0;
            hasPSpeed = false;
            statueForm = false;
            doCapeFlight = false;
            frogSwimming = false;

            return;
        }

        JumpEffect();
        StompEffect();
        GrabEffectCompositeArms();
        DoPowerupRightClick();
        CapeFlightEffect();

        if (currentJump is Jump.Double or Jump.Triple && !PlayerInput.Triggers.Current.Down && !Player.IsOnGroundPrecise() && flightState == FlightState.None) Player.bodyFrame.Y = 56 * 10;

        if (fireFlowerCooldown > 0) fireFlowerCooldown--;
        else if (fireFlowerFireballsCast > 0) fireFlowerFireballsCast = 0;

        if (flightState != FlightState.None && Player.IsOnGroundPrecise())
        {
            currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
            flightState = FlightState.None;
        }

        if (powerupRightClickActionTimer > 0) powerupRightClickActionTimer--;

        if (statueFormCooldown > 0) statueFormCooldown--;
        else if (statueForm && !Player.controlDown)
        {
            statueForm = false;
            Player.direction = directionWhenTurnedToStatue;
            SpawnStatueSparkle();
        }

        if (statueForm && CurrentPowerup is not TanookiSuit) statueForm = false;

        if (frogSwimming)
        {
            if (Player.velocity != Vector2.Zero)
            {
                if (Player.velocity.X != 0) Player.direction = Math.Sign(Player.velocity.X);
                Player.fullRotationOrigin = Player.Hitbox.Size() * 0.5f;
                forceFullRotation = Utils.AngleLerp(forceFullRotation, MathHelper.ToRadians((40 * (Player.velocity.X != 0 ? 0.5f : 1)) * Math.Sign(Player.velocity.Y) * Player.direction), 0.15f);
            }
        }
        else forceFullRotation = 0;

        if (forceFullRotation != 0) Player.fullRotation = forceFullRotation;

        if (groundPoundJumpTimer > 0) groundPoundJumpTimer--;

        if (shouldRemovePowerup && (Player.IsOnGroundPrecise() || Player.wet)) RemovePowerup();
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

        if (CurrentPowerup != null && grabbedNPCIndex == null)
        {
            if (PlayerInput.Triggers.Current.Jump && !Player.mount.Active) CurrentPowerup?.OnJumpHeldDown(Player);

            if (PlayerInput.Triggers.JustReleased.Jump)
            {
                if (flightState != FlightState.None)
                {
                    glideFlySoundSlot = SlotId.Invalid;
                    flightState = FlightState.None;
                    doCapeFlight = false;
                    currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
                }
            }
        }

        if (PlayerInput.Triggers.JustPressed.Down && !doCapeFlight && stompHitbox != null)
        {
            if (currentPowerupType == null || !PowerupID.Sets.DisableGroundPound[(int)currentPowerupType])
            {
                StompHitbox stompHitboxProjectile = (StompHitbox)Main.projectile[(int)stompHitbox].ModProjectile;

                if (!stompHitboxProjectile.groundPound)
                {
                    SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/GroundPoundStart") { Volume = 0.4f }, Player.MountedCenter);
                    Player.fullRotation = 0;
                    stompHitboxProjectile.groundPound = true;
                }

                if (CurrentPowerup is TanookiSuit && !statueForm)
                {
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.owner == Player.whoAmI && projectile.type == ModContent.ProjectileType<TailSwipe>()) projectile.Kill();
                    }

                    PowerupCharge -= 50;
                    SpawnStatueSparkle();
                    forceDirection = 0;
                    directionWhenTurnedToStatue = Player.direction;
                    Player.direction = 1;
                    statueForm = true;
                    statueFormCooldown = 15;
                }
            }
        }

        if (PlayerInput.Triggers.JustPressed.Jump && (Player.IsOnGroundPrecise() || Player.wet) && !statueForm) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{(Player.wet ? "Swim" : "Jump")}") { Volume = 0.4f }, Player.MountedCenter);

        if (doCapeFlight && !Player.IsOnGroundPrecise())
        {
            if (PlayerInput.Triggers.Current.Right)
            {
                if (capeFrameTimer == 1) CapeFrame = (int)MathHelper.Clamp(CapeFrame + Math.Sign(Player.velocity.X), 0, 5);
                else if (capeFrameTimer == 0) capeFrameTimer = 3;
            }

            if (PlayerInput.Triggers.Current.Left)
            {
                if (capeFrameTimer == 1) CapeFrame = (int)MathHelper.Clamp(CapeFrame - Math.Sign(Player.velocity.X), 0, 5);
                else if (capeFrameTimer == 0) capeFrameTimer = 3;
            }

            if (!PlayerInput.Triggers.Current.Right && !PlayerInput.Triggers.Current.Left)
            {
                if (CapeFrame != 2 && capeFrameTimer == 1) CapeFrame += (2 > CapeFrame).ToInt() - (2 < CapeFrame).ToInt();
                else if (capeFrameTimer == 0) capeFrameTimer = 10;
            }
        }

        if (frogSwimming)
        {
            if ((PlayerInput.Triggers.Current.Right || PlayerInput.Triggers.Current.Left || PlayerInput.Triggers.Current.Up || PlayerInput.Triggers.Current.Down || PlayerInput.Triggers.Current.Jump) && Main.GameUpdateCount % 10 == 0)
            {
                frogSwimFrame = frogSwimFrame == 2 ? 0 : frogSwimFrame + 1;
            }
        }
    }

    public override void SetControls()
    {
        if (!CanDoCapEffects) return;

        if (statueForm)
        {
            Player.controlLeft = false;
            Player.controlMap = false;
            Player.controlMount = false;
            Player.controlQuickHeal = false;
            Player.controlQuickMana = false;
            Player.controlRight = false;
            Player.controlUp = false;
            Player.controlUseItem = false;
            Player.controlJump = false;
        }

        if (doCapeFlight)
        {
            Player.controlLeft = false;
            Player.controlRight = false;
            Player.controlUp = false;
            Player.controlDown = false;
        }
    }

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        if (!CanDoCapEffects) return;

        if (statueForm || doCapeFlight || frogSwimming)
        {
            foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
            {
                if (layer != PlayerDrawLayers.Head && (statueForm && layer is not StatueDrawLayer) || (doCapeFlight && layer is not CapeFlightDrawLayer) || (frogSwimming && layer is not FrogSwimDrawLayer))
                {
                    if (frogSwimming && drawInfo.headOnlyRender && layer == PlayerDrawLayers.Head) continue;
                    layer.Hide();
                }
            }
        }
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        if (!statueForm && !GroundPounding) return base.CanBeHitByNPC(npc, ref cooldownSlot);
        return false;
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (!statueForm && !GroundPounding) return base.CanBeHitByProjectile(proj);
        return false;
    }

    public override void UpdateDead()
    {
        Player.fullRotation = 0;
        currentPowerupType = null;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!CanDoCapEffects || target.life > 0) return;

        float hpRef = 40;
        float damageRef = 10;
        float defenseRef = 5;

        float damageWeight = 0.7f;
        float defenseWeight = 0.5f;

        float xpToGive = Math.Max(target.lifeMax / hpRef + damageWeight * target.defDamage / damageRef + defenseWeight * target.defDefense / defenseRef, 1);
        XP += xpToGive;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!CanDoCapEffects) return;

        Vector2 position = drawInfo.drawPlayer.position;
        position -= Main.screenPosition;
        string path = $"{GetType().Namespace!.Replace(".", "/")}";
        float newPercent = XP / XPNeededToLevelUp;

        levelFlagOpacity = MathHelper.Lerp(levelFlagOpacity, IsLevellingUpForFlagAndTextEffects ? 1 : (Math.Abs(newPercent - levelFlagPositionPercent) > 0.001f ? 0.25f : 0), IsLevellingUpForFlagAndTextEffects ? 0.1f : 0.25f);
        levelFlagPositionPercent = MathHelper.Lerp(levelFlagPositionPercent, IsLevellingUpForFlagAndTextEffects ? 1 : newPercent, 0.1f);

        if (IsLevellingUpForFlagAndTextEffects && LevelUpTimer >= 0)
        {
            if (LevelUpTimer % 45 == 0)
            {
                int index = (int)(LevelUpTimer / 45);

                if (index == 4) CombatText.NewText(Player.getRect(), TerrariaXMario.capColors[currentCap!], Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.LevelUp"));
                else IncreaseMaxStat(Player, index, index == 0 ? statSPToAdd : index == 1 ? statHPToAdd : index == 2 ? statDefenseToAdd : statPowerToAdd);
            }

            LevelUpTimer--;
        }
        else
        {
            IsLevellingUpForFlagAndTextEffects = false;
            LevelUpTimer = 180;
        }

        Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/LevelFlag{currentCap}").Value, position + new Vector2(36 - 4 * levelFlagPositionPercent, 20 - 44 * levelFlagPositionPercent), Color.White * levelFlagOpacity);
        Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{path}/LevelFlagpole").Value, position + new Vector2(22, -28), Color.White * levelFlagOpacity);
    }

    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        if (!CanDoCapEffects || stompHitbox == null || damageSource.SourceNPCIndex == -1) return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);

        if (Main.projectile[(int)stompHitbox].getRect().Contains(Main.npc[damageSource.SourceNPCIndex].getRect())) return true;

        return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }

    internal void SetForceDirection(int duration, ForceArmMovementType type, int direction)
    {
        if (duration <= 0 || Math.Abs(direction) != 1) return;

        forceArmMovementType = type;
        forceArmMovementTimer = duration;
        forceDirection = direction;
        if (forceSwitchDirectionCount > 0) forceSwitchDirectionCount--;
        Player.direction = direction;

        if (requestedPowerupRightClick && !shouldRemovePowerup)
        {
            if (CurrentPowerup != null && powerupRightClickActionTimer == 0)
            {
                CurrentPowerup.OnRightClick(Player);
                powerupRightClickActionTimer = CurrentPowerup.RightClickActionCooldown;
            }

            requestedPowerupRightClick = false;
        }
    }

    internal void RemovePowerup()
    {
        if (currentPowerupType != null)
        {
            Player.fullRotation = 0;
            shouldRemovePowerup = false;
            currentPowerupType = null;
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown") { Volume = 0.4f }, Player.MountedCenter);
        }
    }

    private void CapEffect()
    {
        if (oldCap != currentCap)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{currentCap ?? oldCap}{(currentCap == null ? "Unequip" : "Equip")}") { Volume = 0.4f }, Player.MountedCenter);
            if (!BroInfoPlayer?.ShowBroInfo ?? false) BroInfoPlayer?.ShowBroInfo = true;
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
                else if (forceArmMovementType == ForceArmMovementType.Extend) Player.SetCompositeArmFront(true, (Player.CompositeArmStretchAmount)(i == 4 ? 0 : i), Player.MountedCenter.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2);
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

        if (jumpInputBuffer > 10 || Player.sliding || frogSwimming)
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
            if (currentPowerupType == ModContent.GetInstance<FrogSuit>().Type && !CurrentVariants.Contains("Running")) return;
            if (groundPoundJumpTimer == 0) currentJump = currentJump == Jump.Triple ? Jump.Single : currentJump + 1;

            if (currentJump == Jump.Triple)
            {
                jumpFlipTimer = 45;
                backflip = false;
            }

            if ((currentJump is not (Jump.None or Jump.Single) || groundPoundJumpTimer > 0) && (int)currentJump < Enum.GetNames(typeof(Jump)).Length) SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/{currentCap}{(groundPoundJumpTimer > 0 ? "Double" : currentJump)}Jump") { Volume = 0.4f }, Player.MountedCenter);
        }
    }

    private void StompEffect()
    {
        if (!Player.IsOnGroundPrecise() && !frogSwimming)
        {
            stompHitbox ??= Projectile.NewProjectile(Player.GetSource_FromThis(), Player.BottomLeft, Vector2.Zero, ModContent.ProjectileType<StompHitbox>(), StatPower, 4f, Player.whoAmI);
        }
        else KillStompHitbox();
    }

    internal void KillStompHitbox()
    {
        if (stompHitbox != null)
        {
            Main.projectile[(int)stompHitbox].Kill();
            stompHitbox = null;
            GroundPounding = false;
        }
    }

    private void PSpeedEffect()
    {
        if (flightState == FlightState.Flying || frogSwimming) return;

        if ((!Player.controlLeft && !Player.controlRight) || Math.Abs(Player.velocity.X) <= 2.5f)
        {
            if (runTime != 0) runTime = 0;
            if (CurrentVariants.Contains("Running") && !Player.mount.Active) currentHeadVariant = currentBodyVariant = currentLegsVariant = null;
        }
        if (Player.IsOnGroundPrecise() && (Player.controlLeft || Player.controlRight) && Math.Abs(Player.velocity.X) > 2.5f && runTime < runTimeRequiredForPSpeed) runTime++;

        if (runTime >= runTimeRequiredForPSpeed)
        {
            if (!hasPSpeed)
            {
                hasPSpeed = true;
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/FastRunStart") { Volume = 0.4f }, Player.MountedCenter);

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Player.MountedCenter, DustID.Cloud, Scale: 1.5f);
                }
            }

            Player.accRunSpeed *= currentCap == "Luigi" ? 1.5f : 1.25f;
            if (!SoundEngine.TryGetActiveSound(pSpeedSoundSlot, out var pSpeedSound))
            {
                pSpeedSoundSlot = SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/CapEffects/PSpeed") { Volume = 0.2f }, Player.MountedCenter);
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
            SpawnObjectFromBlock(point, bufferPlayer: true);
        }
    }

    private void DoPowerupRightClick()
    {
        if (!Player.tileInteractionHappened && CurrentPowerup != null && grabbedNPCIndex == null && PlayerInput.Triggers.JustPressed.MouseRight && !Player.mouseInterface && Main.mouseItem.IsAir && !statueForm && !shouldRemovePowerup)
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

    internal void SpawnObjectFromBlock(Point? point, bool spawnFromBottom = false, bool bufferPlayer = false)
    {
        if (point == null) return;

        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/BlockHit") { Volume = 8 }, Player.MountedCenter);

        ObjectSpawnerBlockEntity? entity = TerrariaXMario.GetTileEntityOrNull((Point)point);
        if (entity == null || entity.justStruck || entity.WasPreviouslyStruck) return;

        if (entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(BrickBlockTile))
        {
            WorldGen.KillTile(point.Value.X, point.Value.Y);
            return;
        }

        ISpawnableObject objectToSpawn = entity.spawnContents.Length == 0 && entity.tileInternalName == nameof(QuestionBlockTile) ? GetObjectToSpawn() : entity!.spawnContents[0];
        Vector2 center = TerrariaXMario.CenterOfMultitileInWorld((Point)point);
        TileObjectData data = TileObjectData.GetTileData(Framing.GetTileSafely((Point)point));

        void SpawnItem(Item item)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/ItemSpawn") { Volume = 0.4f }, center);
            int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), center - item.Size * 0.5f + new Vector2(0, data.Height * 16 * (spawnFromBottom ? -1 : 1)), new Item(item.type), noGrabDelay: true);
            Main.item[spawnedItem].noGrabDelay = 15;
            SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
            globalItem?.objectSpawnerBlockEntity = entity.Position;
            globalItem?.spawnedFromBottom = spawnFromBottom;
        }

        if (objectToSpawn is PowerupProjectile projectile)
        {
            SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/PowerupSpawn"), center);
            Projectile.NewProjectile(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), center + new Vector2(0, spawnFromBottom ? 4 : -4), new Vector2(0, spawnFromBottom ? projectile.SpawnDownSpeed : projectile.SpawnUpSpeed), projectile.Type, 0, 0);
        }
        else if (objectToSpawn is ModItem item) SpawnItem(item.Item);
        else if (objectToSpawn is DefaultSpawnableObject)
        {
            if (Main.rand.NextBool()) SpawnItem(new(ItemID.Mushroom));
            else
            {
                SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Coin") { Volume = 4f }, center);
                int spawnedItem = Item.NewItem(Player.GetSource_TileInteraction(point.Value.X, point.Value.Y), center - new Vector2(6, 8) + new Vector2(0, data.Height * 16 * (spawnFromBottom ? -1 : 1)), new Item(ItemID.GoldCoin), noGrabDelay: true);
                Main.item[spawnedItem].noGrabDelay = 15;
                SpawnedItem? globalItem = Main.item[spawnedItem].GetGlobalItemOrNull<SpawnedItem>();
                globalItem?.objectSpawnerBlockEntity = entity.Position;
                globalItem?.spawnedFromBottom = spawnFromBottom;
            }
        }

        entity?.spawnContents = [.. entity.spawnContents.Skip(1).ToArray()];
        entity?.justStruck = true;
        if (entity?.spawnContents.Length == 0) entity.WasPreviouslyStruck = true;
        if (bufferPlayer) Player.velocity.Y = 1;
    }

    private ISpawnableObject GetObjectToSpawn()
    {
        bool hasMariosCap = Player.HasItemInAnyInventory(ModContent.ItemType<Mario>());
        bool hasLuigisCap = Player.HasItemInAnyInventory(ModContent.ItemType<Luigi>());

        if (!hasMariosCap && !hasLuigisCap && currentCap == null && Player.trashItem.ModItem is not CapItem) return Main.rand.NextFromCollection([.. ModContent.GetContent<ISpawnableObject>().Where(e => e is CapItem)]);

        if (Main.rand.NextBool(4))
        {
            var pool = ModContent.GetContent<PowerupProjectile>().Where(i => i.CanSpawn(Player)).ToList();
            if (pool.Count > 0) return Main.rand.NextFromCollection(pool);
        }

        IEnumerable<ISpawnableObject> list = ModContent.GetContent<ISpawnableObject>().Where(e => e is not PowerupProjectile);
        if (Main.netMode == NetmodeID.SinglePlayer) list = [.. list.Where(e => e is not Nut1)];
        float num = Main.rand.NextFloat();

        if (num < 0.01f) return Main.rand.NextFromCollection([.. list.Where(e => e.SpawnRarity == SpawnRarity.Legendary)]);
        if (num < 0.05f) return Main.rand.NextFromCollection([.. list.Where(e => e.SpawnRarity == SpawnRarity.Epic)]);
        if (num < 0.1f) return Main.rand.NextFromCollection([.. list.Where(e => e.SpawnRarity == SpawnRarity.Rare)]);
        if (num < 0.5f) return Main.rand.NextFromCollection([.. list.Where(e => e.SpawnRarity == SpawnRarity.Uncommon)]);
        return Main.rand.NextFromCollection([.. list.Where(e => e.SpawnRarity == SpawnRarity.Common), new DefaultSpawnableObject()]);
    }

    private void SpawnStatueSparkle()
    {
        Projectile.NewProjectile(Player.GetSource_Misc("Statue"), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<StatueSparkle>(), 0, 0, Player.whoAmI);
    }

    private void CapeFlightEffect()
    {
        if (capeFrameTimer > 0) capeFrameTimer--;
        if (capeBoostTimer > 0)
        {
            capeBoostTimer--;
            if (capeBoostTimer == 1) capeBoostFactor = 0;
        }

        if (!doCapeFlight) CapeFrame = capeFrameTimer = 0;
        else
        {
            if (Player.IsOnGroundPrecise())
            {
                if (Player.velocity.X != 0)
                {
                    Dust dust = Dust.NewDustPerfect(Math.Sign(Player.velocity.X) == -1 ? Player.BottomLeft : Player.BottomRight, ModContent.DustType<StatueDust>(), new Vector2(-Player.velocity.X * 0.5f, 0));
                    dust.noGravity = false;
                    dust.scale = 0.75f;
                }
                else doCapeFlight = false;
            }
        }
    }
}
