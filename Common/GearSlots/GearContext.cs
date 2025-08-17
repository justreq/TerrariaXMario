using System;

namespace TerrariaXMario.Common.GearSlots;
internal enum GearContext
{
    None,
    Cap,
    Overalls,
    Socks,
    Boots,
    Gloves,
    Badge
}

[AttributeUsage(AttributeTargets.Class)]
internal class GearContextAttribute(GearContext value) : Attribute
{
    internal GearContext value = value;
}