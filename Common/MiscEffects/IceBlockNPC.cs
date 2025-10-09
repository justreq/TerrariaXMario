using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.MiscEffects;
public class IceBlockNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    internal bool frozen;
    private int frozenTimer;

    private bool defaultNoGravity;

    internal Rectangle iceBlockRect = Rectangle.Empty;
    internal bool thrown;

    private readonly Asset<Texture2D> iceBlockTexture = ModContent.Request<Texture2D>($"{nameof(TerrariaXMario)}/Common/MiscEffects/IceBlock");

    internal void KillIceBlock(NPC npc)
    {
        SoundEngine.PlaySound(SoundID.Item27);

        frozen = false;
        thrown = false;
        frozenTimer = 0;

        for (int i = 0; i < 4; i++)
        {
            int gore = Mod.Find<ModGore>($"IceBlockGore_{i + 1}").Type;
            gore = Gore.NewGore(npc.GetSource_Misc("IceBlockTile"), npc.Center, MathHelper.ToRadians(0 - i * 60).ToRotationVector2() * 2.5f, gore);
            Main.gore[gore].timeLeft = 0;
        }
    }

    public override void SetDefaults(NPC entity)
    {
        base.SetDefaults(entity);

        defaultNoGravity = entity.noGravity;
    }

    public override bool PreAI(NPC npc)
    {
        if (frozenTimer <= 0)
        {
            if (frozen)
            {
                npc.noGravity = defaultNoGravity;
                KillIceBlock(npc);
            }

            double factor = npc.frame.Width + npc.frame.Height;
            frozenTimer = 60 * (int)(-4.5f * (Math.Cos(0.00335f * Math.PI * factor + Math.PI) - 1.225f));

            return base.PreAI(npc);
        }

        if (frozen)
        {
            if (thrown)
            {
                npc.velocity.Y += Player.defaultGravity;
                npc.noGravity = false;

                foreach (NPC target in Main.ActiveNPCs)
                {
                    IceBlockNPC? globalNPC = target.GetGlobalNPCOrNull<IceBlockNPC>();
                    if (target != npc && globalNPC != null && globalNPC.frozen && iceBlockRect.Intersects(globalNPC.iceBlockRect))
                    {
                        globalNPC.KillIceBlock(target);
                        target.StrikeInstantKill();
                    }
                }

                if (npc.velocity.X == 0)
                {
                    KillIceBlock(npc);
                    npc.StrikeInstantKill();
                }
            }
            else
            {
                npc.velocity = Vector2.Zero;
                npc.noGravity = true;
            }

            frozenTimer--;

            return false;
        }

        return base.PreAI(npc);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (frozen) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (frozen) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (frozen) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool CanHitNPC(NPC npc, NPC target)
    {
        if (frozen) return false;
        return base.CanHitNPC(npc, target);
    }

    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        if (frozen) return false;
        return base.CanHitPlayer(npc, target, ref cooldownSlot);
    }

    public override void FindFrame(NPC npc, int frameHeight)
    {
        if (frozen)
        {
            npc.frame.X = npc.frame.Y = 0;
            npc.rotation = 0;
            return;
        }

        base.FindFrame(npc, frameHeight);
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);

        if (!frozen)
        {
            this.iceBlockRect = Rectangle.Empty;
            return;
        }

        Vector2 size = npc.frame.Size() * npc.scale;
        Vector2 position = new Vector2(npc.Bottom.X - size.X * 0.5f, npc.Bottom.Y - size.Y + 2) - screenPos;

        if (frozenTimer <= 60) position.X += (float)(2 * Math.Sin((1500 / (frozenTimer + 15)) - 4.3));

        Rectangle iceBlockRect = new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        this.iceBlockRect = iceBlockRect;
        this.iceBlockRect.X += (int)screenPos.X;
        this.iceBlockRect.Y += (int)screenPos.Y;

        Color baseColor = Color.White * 0.5f;

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.TopLeft(), new(0, 0, 4, 4), baseColor); // top left

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X + 4, iceBlockRect.Y, iceBlockRect.Width - 8, 4), new(6, 0, 2, 4), baseColor); // top

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.TopRight() - new Vector2(4, 0), new(10, 0, 4, 4), baseColor); // top right

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X, iceBlockRect.Y + 4, 4, iceBlockRect.Height - 8), new(0, 6, 4, 2), baseColor); // left

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X + 4, iceBlockRect.Y + 4, iceBlockRect.Width - 8, iceBlockRect.Height - 8), new(6, 6, 2, 2), baseColor); // center

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X + iceBlockRect.Width - 4, iceBlockRect.Y + 4, 4, iceBlockRect.Height - 8), new(10, 6, 4, 2), baseColor); // right

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.BottomLeft() - new Vector2(0, 4), new(0, 10, 4, 4), baseColor); // bottom left

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X + 4, iceBlockRect.Y + iceBlockRect.Height - 4, iceBlockRect.Width - 8, 4), new(6, 10, 2, 4), baseColor); // bottom

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.BottomRight() - new Vector2(4), new(10, 10, 4, 4), baseColor); // bottom right

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.TopLeft() + new Vector2(2, 2), new(16, 0, 12, 10), baseColor); // top left shine

        spriteBatch.Draw(iceBlockTexture.Value, iceBlockRect.BottomRight() - new Vector2(8), new(28, 10, 4, 4), baseColor); // bottom right shine

        spriteBatch.Draw(iceBlockTexture.Value, new Rectangle(iceBlockRect.X + 4, iceBlockRect.Y + 4, iceBlockRect.Width - 8, (int)(iceBlockRect.Height * 0.67f)), new(0, 16, 32, 32), Color.White * 0.25f); // glare
    }

    public override bool PreHoverInteract(NPC npc, bool mouseIntersects)
    {
        CapEffectsPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<CapEffectsPlayer>();

        if (!frozen || npc.type == NPCID.TargetDummy || npc.whoAmI == modPlayer?.grabbedNPCIndex || Main.LocalPlayer.Distance(npc.Center) > 64) return base.PreHoverInteract(npc, mouseIntersects);

        modPlayer?.hoverNPCIndex ??= npc.whoAmI;

        return false;
    }

    public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
    {
        if (frozen) return false;

        return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
    }

    public override void SaveData(NPC npc, TagCompound tag)
    {
        tag[nameof(defaultNoGravity)] = defaultNoGravity;
    }

    public override void LoadData(NPC npc, TagCompound tag)
    {
        if (tag.ContainsKey(nameof(defaultNoGravity))) defaultNoGravity = tag.GetBool(nameof(defaultNoGravity));
    }
}
