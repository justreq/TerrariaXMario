using System.Collections.Generic;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Powerups;
internal class PowerupLoader : ILoadable
{
    internal static readonly List<Powerup> powerups = [];

    internal static IReadOnlyList<Powerup> Powerups => powerups;

    internal static int Add(Powerup powerup)
    {
        int type = powerups.Count;
        powerups.Add(powerup);
        return type;
    }

    public void Load(Mod mod) { }

    public void Unload() { }
}
