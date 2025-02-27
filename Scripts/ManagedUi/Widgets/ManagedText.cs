using ManagedUi.Localization;
using ManagedUi.Selectables;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class ManagedText : MonoBehaviour, ISelectableAnimator
{
    [Header("Style")]
    public bool animationEnabled = false;

    [SerializeField] private bool _fixColor = true;
    [SerializeField] private UiSettings.ColorName _colorTheme = UiSettings.ColorName.Main;
    [SerializeField] private Color _customColorSave = Color.white;

    public ManagedColor selectColor = new ManagedColor(false);
    public ManagedColor confirmColor = new ManagedColor(false);

    private Color _animationSavedColor;
    private Vector3 _savedSize;
    private TextMeshProUGUI _text;

    private string _save_original_text;

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

    private void OnEnable()
    {
        _text = GetComponent<TextMeshProUGUI>();
        LocalizationProvider.OnLocalizationChanged -= UpdateOnLocalChanged;
        LocalizationProvider.OnLocalizationChanged += UpdateOnLocalChanged;
    }

    private void OnDisable()
    {
        LocalizationProvider.OnLocalizationChanged -= UpdateOnLocalChanged;
    }

    private void UpdateOnLocalChanged()
    {
        SetTextWithTranslation(_save_original_text);
        _text.SetAllDirty();
    }

    public void SetTextWithTranslation(string text, bool localization = true, LocalizationType.Table table = LocalizationType.Table.UIMenu)
    {
        _text.text = text;
        _save_original_text = text;
        if (localization)
        {
            _text.text = LocalizationProvider.GetTranslatedValue(text, LocalizationType.GetTableFileName(table));
        }
    }

    public void Format(UiSettings.ColorName theme)
    {
        _manager.SetTextAutoFormat(_text, UiSettings.TextStyle.Highlight, theme);
    }

    public void SetColorByTheme(UiSettings.ColorName currentEnumValue)
    {
        if (!_manager) return;
        var colorTemp = _manager.GetTextColorByEnum(currentEnumValue);
        _colorTheme = currentEnumValue;
        _text.color = colorTemp;
    }

    public void SetColorByFixed(Color colorTypeColorValue)
    {
        _text.color = colorTypeColorValue;
    }

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        _animationSavedColor = _text.color;
        this.enabled = enableAnimation;
        gameObject.SetActive(enableAnimation);
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpSize(_savedSize, Vector3.one, currentValue);
                LerpColor(_customColorSave, ColorTheme, currentValue);
                break;
            case ISelectableAnimator.Mode.Selected:
                LerpSize(_savedSize, Vector3.one, currentValue);
                if (selectColor?.UseInAnimation() != null && selectColor.UseInAnimation())
                {
                    LerpColor(selectColor.GetColor(_manager), currentValue);
                    break;
                }
                LerpColor(_manager.SelectedColor, currentValue);
                break;
            case ISelectableAnimator.Mode.Confirmed:
                LerpSize(_savedSize, Vector3.one, currentValue);
                if (confirmColor?.UseInAnimation() != null && confirmColor.UseInAnimation())
                {
                    LerpColor(confirmColor.GetColor(_manager), currentValue);
                    break;
                }
                LerpColor(_manager.ConfirmedColor, currentValue);
                break;
        }
    }

    private void LerpSize(Vector3 startSize, Vector3 targetSize, float currentValue)
    {
        transform.localScale = Vector3.Lerp(startSize, targetSize, currentValue);
    }

    private void LerpColor(Color customColor, UiSettings.ColorName theme, float currentValue)
    {
        if (_fixColor)
        {
            _text.color = Color.Lerp(_animationSavedColor, _manager.GetTextColorByEnum(theme), currentValue);
        }
        else
        {
            _text.color = Color.Lerp(_animationSavedColor, customColor, currentValue);
        }
    }
    private void LerpColor(Color customColor, float currentValue)
    {
        _text.color = Color.Lerp(_animationSavedColor, customColor, currentValue);
    }

    void Awake()
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
        var selectColor = serializedObject.FindProperty("selectColor");
        var confirmColor = serializedObject.FindProperty("confirmColor");


        var fixColor = serializedObject.FindProperty("_fixColor");
        var customColor = serializedObject.FindProperty("_customColorSave");
        var colorTheme = serializedObject.FindProperty("_colorTheme");

        EditorUtils.DrawProperty(fixColor, "Color fixed", "Fix your color by Theme");
        if (fixColor.boolValue)
        {
            EditorUtils.DrawProperty(colorTheme, "Color", "Select Color");
            int enumIndex = colorTheme.enumValueIndex;
            UiSettings.ColorName currentEnumValue = (UiSettings.ColorName)enumIndex;
            text.SetColorByTheme(currentEnumValue);
        }
        else
        {
            EditorUtils.DrawProperty(customColor, "Color", "Select Color");
            text.SetColorByFixed(customColor.colorValue);
        }
        EditorUtils.DrawProperty(animationEnabled, "Animation", "enable automatic animation");
        EditorUtils.DrawProperty(selectColor, "Custom SelectColor", "enable automatic animation");
        EditorUtils.DrawProperty(confirmColor, "Custom SelectColor", "enable automatic animation");


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