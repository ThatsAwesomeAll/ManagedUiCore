using ManagedUi.Selectables;
using UnityEngine;

namespace ManagedUi.Widgets
{
public class ColorAnimation
{
    private Color _animationSavedColor;

    private bool _disableOnAnimationEnd;

    private readonly UiSettings _manager;
    private ManagedColor _selectColor;
    private ManagedColor _basicColor;
    private ManagedColor _confirmColor;

    public ColorAnimation(UiSettings manager,
        ManagedColor basicColor,
        ManagedColor selectColor,
        ManagedColor confirmColor
    )
    {
        _selectColor = selectColor;
        _basicColor = basicColor;
        _confirmColor = confirmColor;
        _manager = manager;
    }

    public void SetEnabled(Color currentColor)
    {
        _animationSavedColor = currentColor;
    }

    public Color LerpTo(ISelectableAnimator.Mode mode, float currentValue, bool isContrast = false)
    {
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                return LerpColor(_basicColor.GetColor(_manager, isContrast), currentValue);
            case ISelectableAnimator.Mode.Selected:
                if (_selectColor?.UseInAnimation() != null && _selectColor.UseInAnimation())
                {
                    return LerpColor(_selectColor.GetColor(_manager, isContrast), currentValue);
                }
                return LerpColor(_manager.SelectedColor, currentValue);
            case ISelectableAnimator.Mode.Confirmed:
                if (_confirmColor?.UseInAnimation() != null && _confirmColor.UseInAnimation())
                {
                    return LerpColor(_confirmColor.GetColor(_manager, isContrast), currentValue);
                }
                return LerpColor(_manager.ConfirmedColor, currentValue);
            default:
                return _basicColor.GetColor(_manager, isContrast);
        }
    }

    private Color LerpColor(Color customColor, float currentValue)
    {
        return Color.Lerp(_animationSavedColor, customColor, currentValue);
    }
}
}