using System;
using UnityEngine;

namespace ManagedUi.Widgets
{

[Serializable]
public class ManagedColor
{
    [SerializeField] private bool _fixedColor = true;
    [SerializeField] private bool _useInAnimation = false;
    [SerializeField] private Color _customColor = Color.white;
    [SerializeField] private UiSettings.ColorName _theme = UiSettings.ColorName.Main;


    public UiSettings.ColorName Theme => _theme;

    public bool IsFixedColor() => _fixedColor;
    public void SetFixedColor(bool fixedColor)
    {
        _fixedColor = fixedColor;
    }

    public bool UseInAnimation() => _useInAnimation;

    public Color GetColor(UiSettings settings, bool isTextColor = false)
    {
        if (!_fixedColor) return _customColor;
        return isTextColor ? settings.GetTextColorByEnum(_theme) :  settings.GetImageColorByEnum(_theme);
    }

    public Color SetColorByTheme(UiSettings.ColorName currentTheme, UiSettings _manager, bool ColorAsText = false)
    {
        if (!_manager) return Color.white;
        Color colorTemp;
        colorTemp = ColorAsText ? _manager.GetTextColorByEnum(currentTheme) : _manager.GetImageColorByEnum(currentTheme);
        _theme = currentTheme;
        return colorTemp;
    }

    public ManagedColor(Color color)
    {
        _fixedColor = false;
        _customColor = color;
        _useInAnimation = true;
    }

    public ManagedColor(bool useInAnimation)
    {
        _fixedColor = false;
        _useInAnimation = useInAnimation;
        _customColor = Color.white;
    }

    public ManagedColor(UiSettings.ColorName theme)
    {
        _theme = theme;
        _fixedColor = true;
        _useInAnimation = true;
    }
    public Color SetColor(Color colorTypeColorValue)
    {
        _customColor = colorTypeColorValue;
        return _customColor;
    }

}


}