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

    public bool IsFixedColor() => _fixedColor;
    public bool UseInAnimation() => _useInAnimation;

    public Color GetColor(UiSettings settings)
    {
        return _fixedColor ? settings.GetImageColorByEnum(_theme) : _customColor;
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
}


}