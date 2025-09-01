namespace TerrariaXMario.Common.PowerupData;
internal class PowerupData
{
    internal virtual string PowerupSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerUp";
    internal virtual string PowerDownSound => $"{TerrariaXMario.Sounds}/PowerupEffects/PowerDown";
    internal virtual void Load() { }
    internal virtual void Unload() { }
    internal virtual void Update() { }
    internal virtual void LeftClick() { }
    internal virtual void LeftClickUpdate() { }
    internal virtual void RightClick() { }
    internal virtual void RightClickUpdate() { }
}
