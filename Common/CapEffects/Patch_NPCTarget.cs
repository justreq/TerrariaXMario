using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Core;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.CapEffects;
internal class Patch_NPCTarget : BasePatch
{
    internal override void Patch(Mod mod)
    {
        // stops NPCs from targetting a player that is in Statue form
        //IL_NPC.TargetClosest += IL_NPC_TargetClosest;
        //IL_NPC.TargetClosestUpgraded += IL_NPC_TargetClosestUpgraded;
        //IL_NPC.TargetClosest_WOF += IL_NPC_TargetClosest_WOF;

        //On_NPC.TargetClosest += On_NPC_TargetClosest;
        //On_NPC.TargetClosestUpgraded += On_NPC_TargetClosestUpgraded;
        //On_NPC.TargetClosest_WOF += On_NPC_TargetClosest_WOF;
    }

    private void SetTargetTrackingValues(NPC self, bool faceTarget, float realDist, int tankTarget)
    {
        if (tankTarget >= 0)
        {
            self.targetRect = new Rectangle((int)Main.projectile[tankTarget].position.X, (int)Main.projectile[tankTarget].position.Y, Main.projectile[tankTarget].width, Main.projectile[tankTarget].height);
            self.direction = 1;
            if ((float)(self.targetRect.X + self.targetRect.Width / 2) < self.position.X + (float)(self.width / 2))
            {
                self.direction = -1;
            }
            self.directionY = 1;
            if ((float)(self.targetRect.Y + self.targetRect.Height / 2) < self.position.Y + (float)(self.height / 2))
            {
                self.directionY = -1;
            }
        }
        else
        {
            if (self.target < 0 || self.target >= 255)
            {
                self.target = 0;
            }
            self.targetRect = new Rectangle((int)Main.player[self.target].position.X, (int)Main.player[self.target].position.Y, Main.player[self.target].width, Main.player[self.target].height);
            if (Main.player[self.target].dead)
            {
                faceTarget = false;
            }
            if (Main.player[self.target].npcTypeNoAggro[self.type] && self.direction != 0)
            {
                faceTarget = false;
            }
            if (faceTarget)
            {
                _ = Main.player[self.target].aggro;
                _ = (Main.player[self.target].height + Main.player[self.target].width + self.height + self.width) / 4;
                bool flag = self.oldTarget >= 0 && self.oldTarget <= 254;
                bool num = Main.player[self.target].itemAnimation == 0 && Main.player[self.target].aggro < 0;
                bool flag2 = !self.boss;
                if (!(num && flag && flag2))
                {
                    self.direction = 1;
                    if ((float)(self.targetRect.X + self.targetRect.Width / 2) < self.position.X + (float)(self.width / 2))
                    {
                        self.direction = -1;
                    }
                    self.directionY = 1;
                    if ((float)(self.targetRect.Y + self.targetRect.Height / 2) < self.position.Y + (float)(self.height / 2))
                    {
                        self.directionY = -1;
                    }
                }
            }
        }
        if (self.confused)
        {
            self.direction *= -1;
        }
        if ((self.direction != self.oldDirection || self.directionY != self.oldDirectionY || self.target != self.oldTarget) && !self.collideX && !self.collideY)
        {
            self.netUpdate = true;
        }
    }

    private void TryTrackingTarget(NPC self, ref float distance, ref float realDist, ref bool t, ref int tankTarget, int j)
    {
        float num = Math.Abs(Main.player[j].position.X + (float)(Main.player[j].width / 2) - self.position.X + (float)(self.width / 2)) + Math.Abs(Main.player[j].position.Y + (float)(Main.player[j].height / 2) - self.position.Y + (float)(self.height / 2));
        num -= (float)Main.player[j].aggro;
        if (Main.player[j].npcTypeNoAggro[self.type] && self.direction != 0)
        {
            num += 1000f;
        }
        if (!t || num < distance)
        {
            t = true;
            tankTarget = -1;
            realDist = Math.Abs(Main.player[j].position.X + (float)(Main.player[j].width / 2) - self.position.X + (float)(self.width / 2)) + Math.Abs(Main.player[j].position.Y + (float)(Main.player[j].height / 2) - self.position.Y + (float)(self.height / 2));
            distance = num;
            self.target = j;
        }
        if (Main.player[j].tankPet >= 0 && !Main.player[j].npcTypeNoAggro[self.type])
        {
            int tankPet = Main.player[j].tankPet;
            float num2 = Math.Abs(Main.projectile[tankPet].position.X + (float)(Main.projectile[tankPet].width / 2) - self.position.X + (float)(self.width / 2)) + Math.Abs(Main.projectile[tankPet].position.Y + (float)(Main.projectile[tankPet].height / 2) - self.position.Y + (float)(self.height / 2));
            num2 -= 200f;
            if (num2 < distance && num2 < 200f && Collision.CanHit(self.Center, 1, 1, Main.projectile[tankPet].Center, 1, 1))
            {
                tankTarget = tankPet;
            }
        }
    }

    private void On_NPC_TargetClosest_WOF(On_NPC.orig_TargetClosest_WOF orig, NPC self, bool faceTarget)
    {
        float distance = 0f;
        float realDist = 0f;
        bool t = false;
        int tankTarget = -1;
        for (int i = 0; i < 255; i++)
        {
            if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && !(Main.player[i].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? true) && Main.player[i].gross)
            {
                TryTrackingTarget(self, ref distance, ref realDist, ref t, ref tankTarget, i);
            }
        }
        SetTargetTrackingValues(self, faceTarget, realDist, tankTarget);
    }

    private void On_NPC_TargetClosestUpgraded(On_NPC.orig_TargetClosestUpgraded orig, NPC self, bool faceTarget, Microsoft.Xna.Framework.Vector2? checkPosition)
    {
        int num = -1;
        int num2 = -1;
        int num3 = -1;
        Vector2 center = self.Center;
        if (checkPosition.HasValue)
        {
            center = checkPosition.Value;
        }
        bool flag = self.direction == 0;
        float num4 = 9999999f;
        for (int i = 0; i < 255; i++)
        {
            Player player = Main.player[i];
            if ((Main.player[i].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false) || !player.active || player.dead || player.ghost)
            {
                continue;
            }
            float num5 = Vector2.Distance(center, player.Center);
            num5 -= (float)player.aggro;
            bool flag2 = player.npcTypeNoAggro[self.type];
            if (flag2 && !flag)
            {
                num5 += 1000f;
            }
            if (num5 < num4)
            {
                num = i;
                num2 = -1;
                num4 = num5;
            }
            if (player.tankPet >= 0 && !flag2)
            {
                num5 = Vector2.Distance(center, Main.projectile[player.tankPet].Center);
                num5 -= 200f;
                if (num5 < num4 && num5 < 200f && Collision.CanHit(self.Center, 0, 0, Main.projectile[player.tankPet].Center, 0, 0))
                {
                    num2 = player.tankPet;
                    num4 = num5;
                }
            }
        }
        for (int j = 0; j < 200; j++)
        {
            NPC nPC = Main.npc[j];
            if (nPC.active && nPC.type == 548)
            {
                float num6 = Vector2.Distance(center, nPC.Center);
                if (num4 > num6)
                {
                    num3 = j;
                    num = -1;
                    num2 = -1;
                    num4 = num6;
                }
            }
        }
        if (num4 == 9999999f)
        {
            return;
        }
        if (num3 >= 0)
        {
            self.target = Main.npc[num3].WhoAmIToTargettingIndex;
            self.targetRect = Main.npc[num3].Hitbox;
            self.direction = ((!((float)self.targetRect.Center.X < self.Center.X)) ? 1 : (-1));
            self.directionY = ((!((float)self.targetRect.Center.Y < self.Center.Y)) ? 1 : (-1));
            return;
        }
        if (num2 >= 0)
        {
            self.target = Main.projectile[num2].owner;
            self.targetRect = Main.projectile[num2].Hitbox;
            self.direction = ((!((float)self.targetRect.Center.X < self.Center.X)) ? 1 : (-1));
            self.directionY = ((!((float)self.targetRect.Center.Y < self.Center.Y)) ? 1 : (-1));
            return;
        }
        if (num < 0 || num >= 255)
        {
            num = 0;
        }
        Player player2 = Main.player[num];
        self.targetRect = player2.Hitbox;
        self.target = num;
        if (player2.dead || (player2.npcTypeNoAggro[self.type] && !flag))
        {
            faceTarget = false;
        }
        if (faceTarget)
        {
            float num7 = (float)(player2.width + player2.height + self.width + self.height) / 4f + 800f;
            float num8 = num4 - (float)player2.aggro;
            if (player2.itemAnimation != 0 || player2.aggro >= 0 || !(num8 > num7) || self.oldTarget < 0 || self.oldTarget >= 255)
            {
                self.direction = ((!((float)self.targetRect.Center.X < self.Center.X)) ? 1 : (-1));
                self.directionY = ((!((float)self.targetRect.Center.Y < self.Center.Y)) ? 1 : (-1));
            }
        }
    }

    private void On_NPC_TargetClosest(On_NPC.orig_TargetClosest orig, NPC self, bool faceTarget)
    {
        float distance = 0f;
        float realDist = 0f;
        bool t = false;
        int tankTarget = -1;
        for (int i = 0; i < 255; i++)
        {
            if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && !(Main.player[i].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? true))
            {
                Main.NewText(Main.GameUpdateCount);
                TryTrackingTarget(self, ref distance, ref realDist, ref t, ref tankTarget, i);
            }
        }
        SetTargetTrackingValues(self, faceTarget, realDist, tankTarget);
    }

    private void IL_NPC_TargetClosest(ILContext il)
    {
        ILCursor c = new(il);
        int index = 0;
        ILLabel label = il.DefineLabel();

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("player"), i => i.MatchLdloc(out index))) ThrowError("ldsfld, ldloc");
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("ghost"), i => i.MatchBrtrue(out label!))) ThrowError("ldfld, brtrue");

        c.EmitLdloc(index);
        c.EmitDelegate((int i) => Main.player[i].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false);
        c.EmitBrtrue(label);
    }

    private void IL_NPC_TargetClosestUpgraded(ILContext il)
    {
        ILCursor c = new(il);
        int index = 0;
        ILLabel label = il.DefineLabel();

        if (!c.TryGotoNext(i => i.MatchLdfld<Player>("dead"))) ThrowError("ldfld");
        if (!c.TryGotoNext(i => i.MatchLdloc(out index), i => i.MatchLdfld<Player>("ghost"), i => i.MatchBrtrue(out label!))) ThrowError("ldloc, dsfld, ldloc");

        c.EmitLdloc(index);
        c.EmitDelegate((Player player) => player.GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false);
        c.EmitBrtrue(label);
    }

    private void IL_NPC_TargetClosest_WOF(ILContext il)
    {
        ILCursor c = new(il);
        int index = 0;
        ILLabel label = il.DefineLabel();

        if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("player"), i => i.MatchLdloc(out index))) ThrowError("ldsfld, ldloc");
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("ghost"), i => i.MatchBrtrue(out label!))) ThrowError("ldfld, brtrue");

        c.EmitLdloc(index);
        c.EmitDelegate((int i) => Main.player[i].GetModPlayerOrNull<CapEffectsPlayer>()?.statueForm ?? false);
        c.EmitBrtrue(label);
    }
}
