using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace TerrariaXMario.Content.NPCs;
internal class Goomba : ModNPC
{
    private enum State
    {
        Chill,
        RunAway,
        RunToward,
        Surprise,
    }

    private ref float AI_State => ref NPC.ai[0];
    private ref float AI_Timer => ref NPC.ai[1];

    private float RequestedSpeed = 0f;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 31;
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.aiStyle = -1;
        NPC.damage = 1;
        NPC.defense = 1;
        NPC.lifeMax = 20;
        NPC.value = 1f;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return SpawnCondition.OverworldDaySlime.Chance * 0.25f;
    }

    public override void AI()
    {
        switch (AI_State)
        {
            case (float)State.Chill:
                Chill();
                break;
            case (float)State.RunAway:
                RunAway();
                break;
            case (float)State.RunToward:
                RunToward();
                break;
            case (float)State.Surprise:
                Surprise();
                break;
        }
    }

    private void CycleFrames(int frameHeight, int frameRate, int firstFrame, int lastFrame)
    {
        NPC.frameCounter++;

        if (NPC.frameCounter < frameRate * (lastFrame - firstFrame))
        {
            NPC.frame.Y = frameHeight * (firstFrame + (int)Math.Floor(NPC.frameCounter / frameRate)) + 1;
        }
        else NPC.frameCounter = 0;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.spriteDirection = NPC.direction;

        switch (AI_State)
        {
            case (float)State.Chill:
                if (NPC.velocity.X == 0) NPC.frame.Y = 0;
                else CycleFrames(frameHeight, 5, 17, 24);
                break;
            case (float)State.RunAway:
                CycleFrames(frameHeight, 5, 1, 8);
                break;
            case (float)State.RunToward:
                CycleFrames(frameHeight, 5, 9, 16);
                break;
            case (float)State.Surprise:
                CycleFrames(frameHeight, 5, 25, 31);
                break;
        }
    }

    public override bool? CanFallThroughPlatforms()
    {
        return AI_State == (float)State.RunToward && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y;
    }

    public override void OnKill()
    {
        int gore = Mod.Find<ModGore>($"GoombaGore").Type;
        gore = Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, gore);
        Main.gore[gore].timeLeft = 0;
    }

    private void Chill()
    {
        NPC.TargetClosest(false);

        if (NPC.velocity.X == 0)
        {
            if (NPC.direction == 1) RequestedSpeed = -1f;
            else if (NPC.direction == -1) RequestedSpeed = 1f;
            else RequestedSpeed = Main.rand.NextFromList(-1f, 1f);
        }

        NPC.velocity.X = RequestedSpeed;
        NPC.direction = Math.Sign(RequestedSpeed);

        if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 150f)
        {
            AI_State = (float)State.Surprise;
            AI_Timer = 0;
        }
    }

    private void Surprise()
    {
        AI_Timer++;
        NPC.velocity.X = 0f;

        if (AI_Timer >= 30)
        {
            RequestedSpeed = 0f;
            AI_State = NPC.ai[2] == 0 ? (float)State.RunToward : (float)State.RunAway;
            AI_Timer = 0;
        }
    }

    private void RunToward()
    {
        int requestedDirection = Math.Sign(Main.player[NPC.target].Center.X - NPC.Center.X);

        if (requestedDirection == -1 && RequestedSpeed > -2f) RequestedSpeed -= 0.1f;
        else if (requestedDirection == 1 && RequestedSpeed < 2f) RequestedSpeed += 0.1f;

        NPC.velocity.X = RequestedSpeed;
        NPC.TargetClosest(true);
    }

    private void RunAway()
    {
        int requestedDirection = -Math.Sign(Main.player[NPC.target].Center.X - NPC.Center.X);

        if (requestedDirection == -1 && RequestedSpeed > -2f) RequestedSpeed -= 0.1f;
        else if (requestedDirection == 1 && RequestedSpeed < 2f) RequestedSpeed += 0.1f;

        NPC.velocity.X = RequestedSpeed;
        NPC.direction = requestedDirection;
    }
}
