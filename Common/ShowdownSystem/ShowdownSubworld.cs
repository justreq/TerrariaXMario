using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;

internal class ShowdownSubworld : Subworld
{
    public override LocalizedText DisplayName => Language.GetText($"Mods.{nameof(TerrariaXMario)}.Showdown.DisplayName");
    public override int Width => 256;
    public override int Height => 512;
    public override bool ShouldSave => false;
    public override bool NoPlayerSaving => true;
    public override List<GenPass> Tasks => [new ShowdownSubworldPass()];

    public override void Load()
    {
        string showdownMusic = $"Assets/Sounds/Music/Showdown";
        string showdownBossMusic = $"Assets/Sounds/Music/ShowdownBoss";
        Main.musicNoCrossFade[MusicLoader.GetMusicSlot(showdownMusic)] = true;
        Main.musicNoCrossFade[MusicLoader.GetMusicSlot(showdownBossMusic)] = true;
    }

    public override void OnEnter()
    {
        SubworldSystem.noReturn = true;
        Main.time = Main.dayLength * 0.5f;
    }

    public override bool ChangeAudio()
    {
        Main.newMusic = MusicLoader.GetMusicSlot($"{TerrariaXMario.Sounds}/Music/Showdown");
        return true;
    }

    public override void DrawMenu(GameTime gameTime)
    {
        // draw loading menu
    }

    public override void Update()
    {
        Main.mapFullscreen = false;
        Main.CaptureModeDisabled = true;
        Main.GameZoomTarget = 2.25f;
        Main.mapStyle = 0;
    }
}

internal class ShowdownSubworldSystem : ModSystem
{
    public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
    {
        if (SubworldSystem.Current != null && SubworldSystem.Current is ShowdownSubworld) timeRate = 0;
    }

    public override void ModifyScreenPosition()
    {
        if (SubworldSystem.Current != null && SubworldSystem.Current is ShowdownSubworld)
        {
            ShowdownPlayer? modPlayer = Main.LocalPlayer.GetModPlayerOrNull<ShowdownPlayer>();
            if (modPlayer?.lockCameraPosition != Vector2.Zero) Main.screenPosition = (Vector2)modPlayer?.lockCameraPosition!;
        }
    }
}

internal class ShowdownSubworldPass : GenPass
{
    public ShowdownSubworldPass() : base("Showdown", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        Main.worldSurface = Main.maxTilesY;
        Main.rockLayer = Main.maxTilesY;
        int j = (int)(Main.maxTilesY * 0.5f);

        for (int i = 0; i < Main.maxTilesX; i++)
        {
            progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY));
            Tile tile = Main.tile[i, j];
            tile.HasTile = true;
            tile.TileType = TileID.EchoBlock;
        }
    }
}