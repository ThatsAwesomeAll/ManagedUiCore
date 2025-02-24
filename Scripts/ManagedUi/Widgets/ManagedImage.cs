using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{

[Serializable]
class ManagedColor
{
    [SerializeField] private bool _fixedColor;
    [SerializeField] private bool _useInAnimation;
    [SerializeField] private Color _customColor;
    [SerializeField] private UiSettings.ColorName _theme;

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
    }

    public ManagedColor(UiSettings.ColorName theme)
    {
        _theme = theme;
        _fixedColor = true;
    }
}


[ExecuteInEditMode]
public class ManagedImage : Image, IManagedGridLayoutElement, ISelectableAnimator
{


    public bool animationEnabled = false;
    public bool disableOnAnimationEnd = false;

    [SerializeField] private bool _fixColor = false;
    [SerializeField] private UiSettings.ColorName _colorTheme = UiSettings.ColorName.Background;
    [SerializeField] private Color _customColorSave = Color.white;

    [SerializeField] private ManagedColor selectColor;
    [SerializeField] private ManagedColor confirmColor;

    public Vector2Int growth = Vector2Int.one;
    public bool ignoreLayout = false;

    private Color _animationSavedColor;

    public UiSettings.ColorName ColorTheme
    {
        get => _colorTheme;
        set
        {
            _colorTheme = value;
            SetColorByTheme(_colorTheme);
        }
    }

    public bool FixColor
    {
        get => _fixColor;
        set
        {
            _fixColor = value;
            if (_fixColor)
            {
                SetColorByTheme(_colorTheme);
            }
            else
            {
                SetColorByFixed(_customColorSave);
            }
        }
    }

    public void SetColorByTheme(UiSettings.ColorName currentEnumValue)
    {
        if (!_manager) return;
        var colorTemp = _manager.GetImageColorByEnum(currentEnumValue);
        _colorTheme = currentEnumValue;
        base.color = colorTemp;
    }

    public void SetColorByFixed(Color colorTypeColorValue)
    {
        base.color = colorTypeColorValue;
    }

    public void SetAsDefaultBackground()
    {
        _fixColor = true;
        sprite = _manager.DefaultBackgroundImage();
        type = UnityEngine.UI.Image.Type.Sliced;
        pixelsPerUnitMultiplier = _manager.DefaultBackgroundImageSliceFactor;
    }

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        _animationSavedColor = color;
        if (!animationEnabled) return;
        bool tempEnabled = (mode != ISelectableAnimator.Mode.Default) || !disableOnAnimationEnd;
        this.enabled = tempEnabled;
        gameObject.SetActive(tempEnabled);
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpColor(_customColorSave, ColorTheme, currentValue);
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

#if UNITY_EDITOR
    private void Update()
    {
        if (!_manager)
        {
            return;
        }
        if (Application.isPlaying)
        {
            return;
        }
        if (_fixColor)
        {
            SetColorByTheme(_colorTheme);
        }
    }
#endif

    protected override void Awake()
    {
        SetUp();
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }

    public int VerticalLayoutGrowth() => growth.y > 0 ? growth.y : 1;
    public int HorizontalLayoutGrowth() => growth.x > 0 ? growth.x : 1;
    public bool IgnoreLayout() => ignoreLayout;

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ManagedImage))]
public class ManagedImageEditor : Editor
{
    private ManagedImage image;

    private void OnEnable()
    {
        image = (ManagedImage)target;
        image.SetUp();
    }


    public override void OnInspectorGUI()
    {
        var UIManagerAsset = serializedObject.FindProperty("_manager");
        var animationEnabled = serializedObject.FindProperty("animationEnabled");
        var disableOnAnimationEnd = serializedObject.FindProperty("disableOnAnimationEnd");
        var selectColor = serializedObject.FindProperty("selectColor");
        var confirmColor = serializedObject.FindProperty("confirmColor");

        var fixColor = serializedObject.FindProperty("_fixColor");
        var customColor = serializedObject.FindProperty("_customColorSave");
        var colorTheme = serializedObject.FindProperty("_colorTheme");

        var growth = serializedObject.FindProperty("growth");
        var ignoreLayout = serializedObject.FindProperty("ignoreLayout");

        var sprite = serializedObject.FindProperty("m_Sprite");


        EditorUtils.DrawProperty(fixColor, "Color fixed", "Fix your color by Theme");
        if (fixColor.boolValue)
        {
            EditorUtils.DrawProperty(colorTheme, "Color", "Select Color");
            int enumIndex = colorTheme.enumValueIndex;
            UiSettings.ColorName currentEnumValue = (UiSettings.ColorName)enumIndex;
            image.SetColorByTheme(currentEnumValue);
        }
        else
        {
            EditorUtils.DrawProperty(customColor, "Color", "Select Color");
            image.SetColorByFixed(customColor.colorValue);
        }
        EditorUtils.DrawProperty(growth, "Layout Growth", "Selecte Grow factor for layout group");
        EditorUtils.DrawProperty(ignoreLayout, "Layout Growth ignored", "Ignore layout group growth");
        var animationProps = new List<SerializedProperty>();
        animationProps.Add(animationEnabled);
        animationProps.Add(disableOnAnimationEnd);
        EditorUtils.DrawPropertyList(animationProps, "Animation | Visible Animation Only", "enable automatic animation", 180);
        EditorUtils.DrawProperty(selectColor, "Custom SelectColor", "enable automatic animation");
        EditorUtils.DrawProperty(confirmColor, "Custom SelectColor", "enable automatic animation");

        EditorUtils.DrawProperty(sprite, "Sprite", "Set Sprite");

        if (UIManagerAsset != null)
        {
            EditorUtils.DrawProperty(UIManagerAsset, "Manager Asset", "Dont change this");
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent("NO MANAGER FOUND"), GUILayout.Width(120));
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtils.DrawCustomHeader();
        base.OnInspectorGUI();
    }
}
#endif
}