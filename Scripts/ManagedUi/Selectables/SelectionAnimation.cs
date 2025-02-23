using ManagedUi.GridSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ManagedUi.Selectables
{

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class SelectionAnimation : MonoBehaviour, IManagedGridLayoutElement, ISelectableAnimator
{
    public List<Image> images = new List<Image>();
    public bool enableNeeded = true;
    public bool spriteFixed = false;
       
    private Image _selectionImage;
    private Dictionary<Image,Color> defaultColors = new Dictionary<Image,Color>();

    [SerializeField] private Sprite _selectionSprite;
    public Sprite SelectionSprite
    {
        get => _selectionSprite;
        set
        {
            _selectionSprite = value; 
            if (_selectionImage) _selectionImage.sprite = _selectionSprite;
        }
    }

    public void SetEnabled(bool enable)
    {
        gameObject.SetActive(enabled);
        _selectionImage.enabled = enable;
    }
    
    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpToDefault(currentValue);
                break;
            case ISelectableAnimator.Mode.Selected:
                break;
            case ISelectableAnimator.Mode.Confirmed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void SetColor(Color color)
    {
        foreach (var image in images)
        {
            image.color = color;
        }
    }
    
    private void OnEnable()
    {
        SetUp();
        
        _selectionImage ??= GetComponent<Image>();
        SetUpSelectionMarker();
        SetupBackgroundImages();
    }

    private void SetupBackgroundImages()
    {
        if (images.Contains(_selectionImage))
        {
            Debug.LogError("Selection Marker in Image List. Prohibited");
            images.Remove(_selectionImage);
        }
        defaultColors.Clear();
        foreach (var image in images)
        {
            defaultColors.Add(image, image.color);
        }
        _selectionImage.enabled = false;
    }
    
    private void SetUpSelectionMarker()
    {
        if (_selectionSprite == null && !spriteFixed)
        {
            _selectionSprite = _manager.DefaultSelectionImage();
        }
        _selectionImage.sprite = _selectionSprite;
        _selectionImage.rectTransform.anchorMax = Vector2.one;
        _selectionImage.rectTransform.anchorMin = Vector2.zero;
        _selectionImage.rectTransform.offsetMin = Vector2.zero;
        _selectionImage.rectTransform.offsetMax = Vector2.zero;
    }
    
    private void LerpToDefault(float currentValue)
    {
        foreach (var image in images)
        {
            if (defaultColors.TryGetValue(image,out var defaultColor))
            {
                image.color = Color.Lerp(image.color, defaultColor, currentValue);
            }
        }
    }
    public int VerticalLayoutGrowth() => 1;
    public int HorizontalLayoutGrowth() => 1;
    public bool IgnoreLayout() => true;
    
        
    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }

}
}