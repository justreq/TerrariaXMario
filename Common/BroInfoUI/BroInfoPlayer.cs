using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerrariaXMario.Common.BroInfoUI;

internal class BroInfoPlayer : ModPlayer
{
    internal bool ShowBroInfo;

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(ShowBroInfo)] = ShowBroInfo;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(ShowBroInfo))) ShowBroInfo = tag.GetBool(nameof(ShowBroInfo));
    }
}