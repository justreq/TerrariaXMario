using Terraria.GameInput;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.KeybindSystem;
internal class KeybindSystem : ModSystem
{
    internal static string GetVanillaKeybindKey(string triggerName) => PlayerInput.GenerateInputTag_ForCurrentGamemode(tagForGameplay: false, triggerName);
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
