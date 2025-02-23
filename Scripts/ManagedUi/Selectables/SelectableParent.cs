using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace ManagedUi.Selectables
{
[RequireComponent(typeof(RectTransform))]
public class SelectableParent : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [FormerlySerializedAs("_animationImage")] [Header("Animation")]
    public SelectionAnimation animationAnimation;

    public float animationDuration = 0.1f;
    public float animationStrengthPercent = 2f;
    public Ease animationEase = Ease.Default;

    [Header("Grid Settings")]
    public bool _gridFixed = false;

    public Action<SelectableParent> OnSelected;
    public Action<SelectableParent> OnConfirmed;

    ISelectableManager _selectableManager;
    RectTransform _rectTransform;
    Vector2Int _internalGridPosition = new Vector2Int(0, 0);
    private bool _confirmed;
    private bool _selected;
    Tween _currentScaleTween;


    public Vector2 ScreenPosition
    {
        get
        {
            if (_rectTransform) return _rectTransform.position;
            return Vector2.zero;
        }
    }

    public Vector2 Size
    {
        get
        {
            if (!_rectTransform) return Vector2.zero;
            return new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        }
    }


    private void Awake()
    {
        _rectTransform ??= GetComponent<RectTransform>();
        SetUpSettings();
    }

    public Vector2Int GridPosition
    {
        get => _internalGridPosition;
        set
        {
            if (!_gridFixed) _internalGridPosition = value;
        }
    }

    public bool Selected => _selected;
    public void SetSelected(bool selected)
    {
        if (selected == _selected)
        {
            return;
        }
        _selected = selected;
        AnimateSelect(_selected);
        if (_selected)
        {
            OnSelected?.Invoke(this);
        }
    }

    public bool Confirmed => _confirmed;
    public void SetConfirmed()
    {
        _confirmed = true;
        if (_confirmed)
        {
            AnimateConfirm();
            OnConfirmed?.Invoke(this);
        }
    }

    private void OnEnable()
    {
        _selectableManager ??= GetComponentInParent<ISelectableManager>();
        animationAnimation ??= GetComponentInChildren<SelectionAnimation>();
        _rectTransform ??= GetComponent<RectTransform>();
        SetUpSettings();
    }

    private void AnimateVisual(float endValuePercent, Color startColor, Color endColor, float inDuration, bool withFadeout = false)
    {
        _currentScaleTween.Stop();
        float scalingValue = 1 + endValuePercent*0.01f;
        Vector3 startSize = transform.localScale;
        Vector3 endSize = Vector3.one*scalingValue;
        if (startSize == endSize)
        {
            return;
        }
        _currentScaleTween = Tween.Custom(0.0f, 1.0f, inDuration, (float currentValue) =>
        {
            if (animationAnimation != null)
            {
                animationAnimation.SetColor(Color.Lerp(startColor, endColor, currentValue));
            }
            transform.localScale = Vector3.Lerp(startSize, endSize, currentValue);
        }, ease: animationEase);
        if (withFadeout)
        {
            _currentScaleTween.OnComplete(() =>
            {
                if (animationAnimation != null)
                {
                    animationAnimation.SetColor(startColor);
                }
                transform.localScale = startSize;
            });
        }
    }

    private void AnimateVisual(float endValuePercent, float inDuration)
    {
        _currentScaleTween.Stop();
        float scalingValue = 1 + endValuePercent*0.01f;
        Vector3 startSize = transform.localScale;
        Vector3 endSize = Vector3.one*scalingValue;
        if (startSize == endSize)
        {
            return;
        }
        _currentScaleTween = Tween.Custom(0.0f, 1.0f, inDuration, (float currentValue) =>
        {
            transform.localScale = Vector3.Lerp(startSize, endSize, currentValue);
            if (animationAnimation != null)
            {
                animationAnimation.LerpToDefault(currentValue);
            }
        }, ease: animationEase);
    }

    private void EnableVisualImage(bool enable)
    {
        if (!animationAnimation) return;
        if (animationAnimation.enableNeeded)
        {
            animationAnimation.SetEnabled(enable);
        }
    }

    private void AnimateSelect(bool selected)
    {
        if (selected)
        {
            AnimateVisual(animationStrengthPercent, _manager.SelectedColor, _manager.SelectedColor, animationDuration);
        }
        else
        {
            AnimateVisual(0, animationDuration);
        }
        EnableVisualImage(selected);
    }

    private void AnimateConfirm()
    {
        AnimateVisual(animationStrengthPercent*1.2f, _manager.SelectedColor, _manager.ConfirmedColor, animationDuration*0.5f, true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _selectableManager?.TriggerExternalConfirm(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }
    public void OnSelect(BaseEventData eventData)
    {
        _selectableManager?.TriggerExternalSelect(this);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _selectableManager?.TriggerExternalDeSelect(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // OnDeselect(eventData);
    }

    [SerializeField] private UiSettings _manager;
    void SetUpSettings()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }
}

}