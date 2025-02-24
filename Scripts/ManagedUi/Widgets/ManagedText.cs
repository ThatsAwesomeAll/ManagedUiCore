using ManagedUi.Selectables;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{
public class ManagedText : TextMeshProUGUI, ISelectableAnimator
{
    [Header("Style")]
    public bool animationEnabled = false;

    [SerializeField] private bool _fixColor = true;
    [SerializeField] private UiSettings.ColorName _colorTheme = UiSettings.ColorName.Main;

    private Color _saveCustomColor;
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
                SetColorByFixed(_saveCustomColor);
            }
        }
    }

    public void SetColorByTheme(UiSettings.ColorName currentEnumValue)
    {
        if (!_manager) return;
        var colorTemp = _manager.GetTextColorByEnum(currentEnumValue);
        _saveCustomColor = color;
        _colorTheme = currentEnumValue;
        color = colorTemp;
    }

    public void SetColorByFixed(Color colorTypeColorValue)
    {
        _saveCustomColor = color;
        color = colorTypeColorValue;
    }

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        _animationSavedColor = color;
        if (animationEnabled) gameObject.SetActive(enabled);
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpColor(_saveCustomColor, ColorTheme, currentValue);
                break;
            case ISelectableAnimator.Mode.Selected:
                LerpColor(_manager.SelectedColor, currentValue);
                break;
            case ISelectableAnimator.Mode.Confirmed:
                LerpColor(_manager.ConfirmedColor, currentValue);
                break;
        }
    }
    private void LerpColor(Color customColor, UiSettings.ColorName theme, float currentValue)
    {
        if (_fixColor)
        {
            color = Color.Lerp(_animationSavedColor, _manager.GetTextColorByEnum(theme), currentValue);
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
    
    protected override void Awake()
    {
        SetUp();
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ManagedText))]
public class ManagedTextEditor : Editor
{
    private ManagedText text;

    private void OnEnable()
    {
        text = (ManagedText)target;
        text.SetUp();
    }


    public override void OnInspectorGUI()
    {
        
        var UIManagerAsset = serializedObject.FindProperty("_manager");
        var animationEnabled = serializedObject.FindProperty("animationEnabled");
        var colorType = serializedObject.FindProperty("imageColor");
        var fixColor = serializedObject.FindProperty("_fixColor");
        var colorTheme = serializedObject.FindProperty("_colorTheme");

        EditorUtils.DrawProperty(fixColor, "Color fixed", "Fix your color by Theme");
        EditorUtils.DrawProperty(animationEnabled, "Auto Animation", "enable automatic animation");
        if (fixColor.boolValue)
        {
            EditorUtils.DrawProperty(colorTheme, "Color", "Select Color");
            int enumIndex = colorTheme.enumValueIndex;
            UiSettings.ColorName currentEnumValue = (UiSettings.ColorName)enumIndex;
            text.SetColorByTheme(currentEnumValue);
        }
        else
        {
            Color temp = colorType.colorValue;
            EditorUtils.DrawProperty(colorType, "Color", "Select Color");
            if (temp != colorType.colorValue)
            {
                text.SetColorByFixed(colorType.colorValue);
            }
        }

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