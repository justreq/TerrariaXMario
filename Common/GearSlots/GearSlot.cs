using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Common.GearSlots;
internal abstract class GearSlot : ModAccessorySlot
{
    private GearContext GearContext => GetType().GetCustomAttribute<GearContextAttribute>()?.value ?? GearContext.None;

    internal static Vector2 GetCustomLocation(int slotOffsetX = 0, int slotOffsetY = 0) => new(Main.screenWidth - 92 + 47 * slotOffsetX, AccessorySlotLoader.DrawVerticalAlignment + slotOffsetY * 56 * Main.inventoryScale + (slotOffsetY > 2 ? 4 : 0));

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) => checkItem.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearContext == GearContext;

    public override bool DrawDyeSlot => GearContext == GearContext.Cap;

    public override bool DrawVanitySlot => false;

    public override string FunctionalTexture => GetType().FullName!.Replace(".", "/");

    public override bool HasEquipmentLoadoutSupport => false;

    public override bool IsEnabled() => Player.GetModPlayerOrNull<GearSlotPlayer>()?.showGearSlots ?? false;

    public override bool IsHidden() => Main.EquipPage != 0;

    public override bool IsVisibleWhenNotEnabled() => false;

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) => item.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearContext == GearContext;

    public override void OnMouseHover(AccessorySlotType context)
    {
        if (context == AccessorySlotType.DyeSlot) base.OnMouseHover(context);
        else Main.hoverItemName = GearContext.ToString();
    }
}