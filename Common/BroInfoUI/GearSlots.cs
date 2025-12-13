using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.BroInfoUI;

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

internal abstract class GearSlot : ModAccessorySlot
{
    private GearType GearType => GetType().GetCustomAttribute<GearTypeAttribute>()?.value ?? GearType.None;

    internal static Vector2 GetCustomLocation(int slotOffsetX = 0, int slotOffsetY = 0) => new(Main.screenWidth - 92 + 47 * slotOffsetX, AccessorySlotLoader.DrawVerticalAlignment + slotOffsetY * 56 * Main.inventoryScale + (slotOffsetY > 2 ? 4 : 0));

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) => context == AccessorySlotType.FunctionalSlot && checkItem.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType == GearType;

    public override bool DrawDyeSlot => GearType == GearType.Cap;

    public override bool DrawVanitySlot => false;

    public override string FunctionalTexture => GetType().FullName!.Replace(".", "/");

    public override bool IsEnabled() => true;

    public override bool IsHidden() => (!Player.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) || Main.EquipPage != 0;

    public override bool IsVisibleWhenNotEnabled() => false;

    public override bool HasEquipmentLoadoutSupport => false;

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) => item.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType == GearType;

    public override void OnMouseHover(AccessorySlotType context)
    {
        if (context == AccessorySlotType.DyeSlot) base.OnMouseHover(context);
        else Main.hoverItemName = Language.GetTextValue($"Mods.{nameof(TerrariaXMario)}.UI.GearType.{GearType}");
    }

    public override void BackgroundDrawColor(AccessorySlotType context, ref Color color)
    {
        color = (context == AccessorySlotType.FunctionalSlot ? new Color(122, 63, 83) : new Color(84, 37, 87)) * 0.7f;
    }
}

[GearType(GearType.Cap)]
internal class GearSlot_Cap : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation();
}

[GearType(GearType.Overalls)]
internal class GearSlot_Overalls : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 1);
}

[GearType(GearType.Gloves)]
internal class GearSlot_Gloves : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 1);
}

[GearType(GearType.Boots)]
internal class GearSlot_Boots : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 2);
}

[GearType(GearType.Socks)]
internal class GearSlot_Socks : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(-1, 2);
}

[GearType(GearType.Badge)]
internal class GearSlot_Badge : GearSlot
{
    public override Vector2? CustomLocation => GetCustomLocation(slotOffsetY: 3);
}