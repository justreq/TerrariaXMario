using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.ShowdownSystem;
internal class ShowdownUICharacter : UIElement
{
    internal Player player;
    internal float scale;

    internal string? currentCap;
    internal string? currentHeadVariant;
    internal string? currentBodyVariant;
    internal string? currentLegsVariant;

    internal Powerup? currentPowerup;

    internal ShowdownUICharacter(Player player, float scale = 1) : base()
    {
        this.player = player.SerializedClone();
        CapEffectsPlayer? modPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();
        currentCap = modPlayer?.currentCap;
        currentHeadVariant = modPlayer?.currentHeadVariant;
        currentBodyVariant = modPlayer?.currentBodyVariant;
        currentLegsVariant = modPlayer?.currentLegsVariant;
        currentPowerup = modPlayer?.currentPowerup;

        this.scale = scale;
        Width = StyleDimension.FromPixels(40);
        Height = StyleDimension.FromPixels(56);
        UseImmediateMode = true;
    }

    private Vector2 GetPlayerPosition(ref CalculatedStyle dimensions) // Copied from vanilla UICharacter
    {
        Vector2 result = dimensions.Position() + new Vector2(dimensions.Width * 0.5f - (float)(player.width >> 1), dimensions.Height * 0.5f - (float)(player.height >> 1));

        return result;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        player.head = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{currentPowerup?.Name ?? ""}{currentCap}{currentHeadVariant ?? ""}", EquipType.Head);
        player.body = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{currentPowerup?.Name ?? ""}{currentCap}{currentBodyVariant ?? ""}", EquipType.Body);
        player.legs = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{currentPowerup?.Name ?? ""}{currentCap}{currentLegsVariant ?? ""}", EquipType.Legs);

        CalculatedStyle dimensions = GetDimensions();
        Main.PlayerRenderer.DrawPlayer(Main.Camera, player, GetPlayerPosition(ref dimensions) + Main.screenPosition, player.fullRotation, player.fullRotationOrigin, scale: scale);
    }
}
