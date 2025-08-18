using System;

namespace TerrariaXMario.Common.GearSlots;
internal enum GearType
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
internal class GearTypeAttribute(GearType value) : Attribute
{
    internal GearType value = value;
}