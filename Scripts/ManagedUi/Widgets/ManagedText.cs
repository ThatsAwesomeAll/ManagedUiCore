using ManagedUi.Localization;
using ManagedUi.Selectables;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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

    [SerializeField] private ManagedColor basicColor = new ManagedColor(UiSettings.ColorName.Background);
    [SerializeField] private ManagedColor selectColor = new ManagedColor(UiSettings.ColorName.Background);
    [SerializeField] private ManagedColor confirmColor = new ManagedColor(UiSettings.ColorName.Background);

    public ManagedColor BasicColor
    {
        get => basicColor;
        set
        {
            basicColor = value;
            UpdateColor();
        }
    }

    public ManagedColor SelectColor { get => selectColor; set => selectColor = value; }
    public ManagedColor ConfirmColor { get => selectColor; set => selectColor = value; }

    private Color _animationSavedColor;
    private Vector3 _savedSize;
    [SerializeField] private TextMeshProUGUI _text;
    public LocalizationType.Table localizationType = LocalizationType.Table.UIMenu;

    private ColorAnimation _colorAnimation;

    [SerializeField] private string _manualText;
    private UiSettings.ColorName _backgroundTheme;
    private UiSettings.TextStyle _textStyle;


    public void UpdateColor()
    {
        if (_text != null)
        {
            _text.color = basicColor.GetColor(_manager, true);
        }
    }


    private void OnEnable()
    {
        _text = GetComponent<TextMeshProUGUI>();
        LocalizationProvider.OnLocalizationChanged -= UpdateOnLocalChanged;
        LocalizationProvider.OnLocalizationChanged += UpdateOnLocalChanged;
        _colorAnimation = new ColorAnimation(_manager,
            basicColor, selectColor, confirmColor);
        _manager.OnSettingsChanged += UpdateOnSettingsChanged;
    }

    private void UpdateOnSettingsChanged()
    {
        Format(_backgroundTheme, _textStyle);
    }

    private void OnDisable()
    {
        LocalizationProvider.OnLocalizationChanged -= UpdateOnLocalChanged;
        _manager.OnSettingsChanged -= UpdateOnSettingsChanged;
    }

    private void UpdateOnLocalChanged()
    {
        SetTextWithTranslation(_manualText);
        _text.SetAllDirty();
    }

    public void SetTextWithTranslation(string text, bool localization = true, LocalizationType.Table table = LocalizationType.Table.UIMenu)
    {
        if (!_text) return;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        _text.text = text;
        _manualText = text;
        if (!localization)
            return;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        _text.text = LocalizationProvider.GetTranslatedValue(text, LocalizationType.GetTableFileName(table));
    }

    public void SetBasicColorTheme(UiSettings.ColorName textColor)
    {
        basicColor.SetFixedColor(true);
        basicColor.SetColorByTheme(textColor, _manager, true);
        UpdateColor();
    }

    public void Format(UiSettings.ColorName theme, UiSettings.TextStyle style = UiSettings.TextStyle.Text)
    {
        _backgroundTheme = theme;
        _textStyle = style;
        if (!_text) return;
        _text.color = basicColor.SetColorByTheme(theme, _manager, true);
        _manager.SetTextAutoFormat(_text, style);
    }

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        if (_text) _colorAnimation?.SetEnabled(_text.color);
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        if (_text) _text.color = _colorAnimation.LerpTo(mode, currentValue, true);
    }

    void Awake()
    {
        SetUp();
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        UiSettings.ConnectSettings(ref _manager);
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ManagedText))]
public class ManagedTextEditor : Editor
{
    private ManagedText text;
    private bool foldout;

    private void OnEnable()
    {
        text = (ManagedText)target;
        text.SetUp();
    }


    public override void OnInspectorGUI()
    {

        var UIManagerAsset = serializedObject.FindProperty("_manager");
        var localizationType = serializedObject.FindProperty("localizationType");
        var _manualText = serializedObject.FindProperty("_manualText");
        
        var animationEnabled = serializedObject.FindProperty("animationEnabled");

        var basicColor = serializedObject.FindProperty("basicColor");
        var selectColor = serializedObject.FindProperty("selectColor");
        var confirmColor = serializedObject.FindProperty("confirmColor");


        EditorGUI.BeginChangeCheck();
        EditorUtils.DrawProperty(_manualText, "Manual Text", "enable automatic animation");
        EditorUtils.DrawProperty(localizationType, "Localization", "enable automatic animation");
        EditorUtils.DrawProperty(basicColor, "Color", "enable automatic animation");
        bool updateRequired = EditorGUI.EndChangeCheck();
        EditorUtils.DrawProperty(animationEnabled, "Animation", "enable automatic animation");
        EditorUtils.DrawProperty(selectColor, "Custom SelectColor", "enable automatic animation");
        EditorUtils.DrawProperty(confirmColor, "Custom ConfirmColor", "enable automatic animation");


        if (UIManagerAsset != null)
        {
            EditorUtils.DrawProperty(UIManagerAsset, "Manager Asset", "Dont change this");
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent("NO MANAGER FOUND"), GUILayout.Width(120));
        }

        serializedObject.ApplyModifiedProperties();
        if (updateRequired)
        {
            text.SetTextWithTranslation(_manualText.stringValue, true,(LocalizationType.Table) localizationType.enumValueIndex);
            text.UpdateColor();
            text.Format(text.BasicColor.Theme);
        }
        EditorUtils.DrawCustomHeader();
        foldout = EditorGUILayout.Foldout(foldout, "Advanced Settings");
        if (foldout)
        {
            base.OnInspectorGUI();
        }
    }
}
#endif
}