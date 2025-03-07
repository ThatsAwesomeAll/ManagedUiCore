using ManagedUi.Widgets;
using UnityEngine;
using UnityEngine.UI;

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
    
    public static void ActiveDefaultButtonAnimation(ManagedImage image, UiSettings.ColorName selectColor = UiSettings.ColorName.Darker, UiSettings.ColorName confirmColor = UiSettings.ColorName.Background)
    {
        image.animationEnabled = true;
        image.SelectColor = new ManagedColor(selectColor);
        image.ConfirmColor = new ManagedColor(confirmColor);
    }    
    public static void ActiveDefaultButtonAnimation(ManagedText text, UiSettings.ColorName selectColor = UiSettings.ColorName.Darker, UiSettings.ColorName confirmColor = UiSettings.ColorName.Background)
    {
        text.animationEnabled = true;
        text.SelectColor = new ManagedColor(selectColor);
        text.ConfirmColor = new ManagedColor(confirmColor);
    }
    public static void StyleShadow(ManagedImage shadow, UiSettings manager)
    {
        SetFullScreen(shadow.rectTransform);
        shadow.animationEnabled = false;
        shadow.BasicColor.SetColorByTheme(UiSettings.ColorName.Dark, manager);
        shadow.sprite = manager?.DefaultShadowImage();
    }
    public static void ScaleRectTrans(RectTransform transform)
    {
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(1, 1);
        transform.offsetMin = Vector2.zero;
        transform.offsetMax = Vector2.zero;
        transform.anchoredPosition = new Vector2(0, 0);
        transform.pivot = new Vector2(0.5f, 0.5f);
    }
    public static void StyleBoarder(ManagedImage image, UiSettings manager)
    {
        image.BasicColor.SetColorByTheme(UiSettings.ColorName.Light,manager);
        image.sprite = manager.DefaultBoarderImage();
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = manager.DefaultBoarderImageSliceFactor;
    }
}
}