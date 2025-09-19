using System;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace TerrariaXMario.Utilities.Extensions;
internal static class UIElementExtensions
{
    // Thank the great Mirsario for this simple yet surprisingly comfortable method of dealing with UI element initialization
    public static T With<T>(this T element, Action<T> action) where T : UIElement
    {
        action(element);

        return element;
    }

    public static T AddElement<T>(this UIElement parent, T child) where T : UIElement
    {
        if (parent is UIGrid uiGrid) uiGrid.Add(child);
        else if (parent is UIList uiList) uiList.Add(child);
        else parent.Append(child);

        return child;
    }
}