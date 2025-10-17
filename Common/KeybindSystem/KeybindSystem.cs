using Terraria.ModLoader;

namespace TerrariaXMario.Common.KeybindSystem;
internal class KeybindSystem : ModSystem
{
    internal static ModKeybind? EnterShowdownKeybind { get; private set; }

    public override void Load()
    {
        EnterShowdownKeybind = KeybindLoader.RegisterKeybind(Mod, "EnterShowdown", "H");
    }

    public override void Unload()
    {
        EnterShowdownKeybind = null;
    }
}
