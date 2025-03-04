using PrimeTween;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ManagedUi.Selectables
{
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class SelectableParent : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation")]
    [SerializeField] List<ISelectableAnimator> _selectionAnimators = new List<ISelectableAnimator>();

    public bool _customAnimation = false;
    [SerializeField] private float _animationDuration = 0.1f;
    [SerializeField] private float _animationConfirmedDuration = 0.1f;
    [SerializeField] private float _animationStrengthPercent = 5f;
    [SerializeField] private Ease _animationEase = Ease.Linear;

    public float AnimationDuration
    {
        get
        {
            if (!_customAnimation && _manager)
            {
                return _manager.DefaultSelectionDuration;
            }
            return _animationDuration;
        }
    }    
    
    public float AnimationConfirmedDuration
    {
        get
        {
            if (!_customAnimation && _manager)
            {
                return _manager.DefaultConfirmedDuration;
            }
            return _animationConfirmedDuration;
        }
    }

    public float AnimationStrengthPercent
    {
        get
        {
            if (!_customAnimation && _manager)
            {
                return _manager.DefaultSelectionStrength;
            }
            return _animationStrengthPercent;
        }
    }

    public Ease AnimationEase
    {
        get
        {
            if (!_customAnimation)
            {
                return Ease.Default;
            }
            return _animationEase;
        }
    }

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
        _selectionAnimators = GetComponentsInChildren<ISelectableAnimator>(true).ToList();
        var selectionAnimator = GetComponent<ISelectableAnimator>();
        if (selectionAnimator != null)
        {
            _selectionAnimators.Add(selectionAnimator);
        }
        _rectTransform ??= GetComponent<RectTransform>();
        SetUpSettings();
    }

    private void AnimateVisual(float endSizeScalingPercent, float inDuration, ISelectableAnimator.Mode mode = default, ISelectableAnimator.Mode fadeoutMode = default)
    {
        _currentScaleTween.Stop();
        float scalingValue = 1 + endSizeScalingPercent*0.01f;
        Vector3 startSize = transform.localScale;
        Vector3 endSize = Vector3.one*scalingValue;

        _currentScaleTween = Tween.Custom(0.0f, 1.0f, inDuration, (float currentValue) =>
        {
            transform.localScale = Vector3.Lerp(startSize, endSize, currentValue);
            foreach (var animators in _selectionAnimators)
            {
                animators.LerpTo(mode, currentValue);
            }
        }, ease: AnimationEase);
        if (fadeoutMode == mode)
        {
            return;
        }
        _currentScaleTween.OnComplete(() =>
        {
            foreach (var animators in _selectionAnimators)
            {
                animators.SetEnabled(fadeoutMode);
                animators.LerpTo(fadeoutMode, 1);
            }
            transform.localScale = startSize;
        });
    }

    private void EnableVisualImage(ISelectableAnimator.Mode mode)
    {
        foreach (var animators in _selectionAnimators)
        {
            animators.SetEnabled(mode);
        }
    }

    private void AnimateSelect(bool selected)
    {
        var mode = selected ? ISelectableAnimator.Mode.Selected : ISelectableAnimator.Mode.Default;
        EnableVisualImage(mode);
        if (selected)
        {
            AnimateVisual(AnimationStrengthPercent, AnimationDuration, ISelectableAnimator.Mode.Selected,ISelectableAnimator.Mode.Selected);
        }
        else
        {
            AnimateVisual(0, AnimationDuration);
        }
    }

    private void AnimateConfirm()
    {
        EnableVisualImage(ISelectableAnimator.Mode.Confirmed);
        AnimateVisual(AnimationStrengthPercent, AnimationConfirmedDuration, ISelectableAnimator.Mode.Confirmed, ISelectableAnimator.Mode.Selected);
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
        if (_selectableManager != null)
        {
            _selectableManager.TriggerExternalSelect(this);
        }
        else
        {
            AnimateSelect(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_selectableManager != null)
        {
            _selectableManager?.TriggerExternalDeSelect(this);
        }
        else
        {
            AnimateSelect(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_selectableManager == null)
        {
            AnimateSelect(false);
        }

    }

    [SerializeField] private UiSettings _manager;
    void SetUpSettings()
    {
        UiSettings.ConnectSettings(ref _manager);
    }
}

}