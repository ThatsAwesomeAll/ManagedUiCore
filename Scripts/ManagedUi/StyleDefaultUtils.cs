using ManagedUi.Widgets;
using UnityEngine;

namespace ManagedUi
{
public static class StyleDefaultUtils
{
    public static void SetFullScreen(RectTransform rect)
    {
        rect.anchorMax = Vector2.one;
        rect.anchorMin = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public static void StyleSelectionMarker(ManagedImage image)
    {
        image.FixColor = true;
        image.ignoreLayout = true;
        image.animationEnabled = true;
        image.disableOnAnimationEnd = true;
        image.ColorTheme = UiSettings.ColorName.Light;
        SetFullScreen(image.rectTransform);
    }
    
    public static void ActiveDefaultButtonAnimation(ManagedImage image, UiSettings.ColorName selectColor = UiSettings.ColorName.Background, UiSettings.ColorName confirmColor = UiSettings.ColorName.BackgroundDarker)
    {
        image.animationEnabled = true;
        image.SelectColor = new ManagedColor(selectColor);
        image.ConfirmColor = new ManagedColor(confirmColor);
    }    
    public static void ActiveDefaultButtonAnimation(ManagedText image, UiSettings.ColorName selectColor = UiSettings.ColorName.Background, UiSettings.ColorName confirmColor = UiSettings.ColorName.BackgroundDarker)
    {
        image.animationEnabled = true;
        image.SelectColor = new ManagedColor(selectColor);
        image.ConfirmColor = new ManagedColor(confirmColor);
    }
}
}