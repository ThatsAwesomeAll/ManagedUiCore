using ManagedUi.Selectables;
using UnityEngine;

namespace ManagedUi.Widgets
{
public class SelectionAnimator
{
    private Color _animationSavedColor;
    private Color color;
    private bool animationEnabled;
    private bool disableOnAnimationEnd;
    private bool _fixColor = false;
    private Color _customColorSave = Color.white;
    
    private UiSettings.ColorName _colorTheme = UiSettings.ColorName.Background;
    private readonly UiSettings _manager;
    
    
    public ManagedColor selectColor = new ManagedColor(false);
    public ManagedColor confirmColor = new ManagedColor(false);
    
    public SelectionAnimator(UiSettings manager)
    {
        _manager = manager;
    }

    public bool SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        _animationSavedColor = color;
        if (!animationEnabled) return true;
        return (mode != ISelectableAnimator.Mode.Default) || !disableOnAnimationEnd;
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpColor(_customColorSave, _colorTheme, currentValue);
                break;
            case ISelectableAnimator.Mode.Selected:
                if (selectColor?.UseInAnimation() != null && selectColor.UseInAnimation())
                {
                    LerpColor(selectColor.GetColor(_manager), currentValue);
                }
                else
                {
                    LerpColor(_manager.SelectedColor, currentValue);
                }
                break;
            case ISelectableAnimator.Mode.Confirmed:
                if (confirmColor?.UseInAnimation() != null && confirmColor.UseInAnimation())
                {
                    LerpColor(confirmColor.GetColor(_manager), currentValue);
                }
                else
                {
                    LerpColor(_manager.ConfirmedColor, currentValue);
                }
                break;
        }
    }

    private void LerpColor(Color customColor, UiSettings.ColorName theme, float currentValue)
    {
        if (_fixColor)
        {
            color = Color.Lerp(_animationSavedColor, _manager.GetImageColorByEnum(theme), currentValue);
        }
        else
        {
            color = Color.Lerp(_animationSavedColor, customColor, currentValue);
        }
    }
    private void LerpColor(Color customColor, float currentValue)
    {
        color = Color.Lerp(_animationSavedColor, customColor, currentValue);
    }
}
}