using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerrariaXMario.Common.SpawnableObject;

internal enum SpawnRarity { Common, Uncommon, Rare, Epic, Legendary }
internal interface ISpawnableObject : IModType, ILoadable
{
    internal SpawnRarity SpawnRarity { get; set; }
}

internal class DefaultSpawnableObject : ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Common;

    Mod IModType.Mod => TerrariaXMario.Instance;

    string IModType.Name => "";

    string IModType.FullName => "";

    void ILoadable.Load(Mod mod) { }

    void ILoadable.Unload() { }
}

internal class ISpawnableObjectSerializer : TagSerializer<ISpawnableObject, TagCompound>
{
    public override TagCompound Serialize(ISpawnableObject value) => new()
    {
        ["name"] = value.GetType().Name,
    };

    public override ISpawnableObject Deserialize(TagCompound tag) => ModContent.GetContent<ISpawnableObject>().FirstOrDefault(e => e.Name == tag.GetString("name")) ?? new DefaultSpawnableObject();
}