using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using ManagedUi.Settings;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
[RequireComponent(typeof(ManagedImage))]
[RequireComponent(typeof(SelectableParent))]
public class SimpleButton : MonoBehaviour, IManagedGridLayoutElement
{

    private const string C_ButtonTextObjectName = "ButtonText";
    private const string C_ShadowImageObjectName = "Shadow";

    public string _buttonText = "No TEXT";
    public bool getTextFromName = true;
    public bool autoFormat = true;
    public int growFactor = 1;
    public ManagedImage Image => _image;

    [SerializeField] private SelectableParent _selectable;
    public SelectableParent Selectable => _selectable;
    [SerializeField] private ManagedImage _image;
    [SerializeField] private ManagedText _text;
    private ManagedImage _shadow;

    public void SetShadow(bool enabled)
    {
        if (_shadow)
        {
            _shadow.enabled = enabled;
        }
    }

    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            SetText();
        }
    }

    public void RefreshButton()
    {
        SetText();
        if (_manager && autoFormat)
        {
            _image.sprite = _manager.DefaultBackgroundImage();
        }
    }

    protected void Awake()
    {
        SetUp();
    }

    private void OnDisable()
    {
        _manager.OnSettingsChanged -= RefreshButton;
    }

    private void OnEnable()
    {
        if (!_image)
        {
            _image = GetComponent<ManagedImage>();
            _image.SetDefaultBackgroundImage();
            StyleDefaultUtils.ActiveDefaultButtonAnimation(_image);
        }
        _text ??= GetComponentInChildren<ManagedText>();
        if (!_text)
        {
            var textChild = new GameObject(C_ButtonTextObjectName);
            textChild.transform.SetParent(transform, false);
            _text = textChild.AddComponent<ManagedText>();
            StyleDefaultUtils.ActiveDefaultButtonAnimation(_text);
        }
        var allImages = GetComponentsInChildren<ManagedImage>();
        foreach (var image in allImages)
        {
            _shadow = image.name switch
            {
                C_ShadowImageObjectName => image,
                _ => _shadow
            };
        }
        if (!_shadow)
        {
            var shadowChild = new GameObject(C_ShadowImageObjectName);
            shadowChild.transform.SetParent(transform, false);
            _shadow = shadowChild.AddComponent<ManagedImage>();
            StyleDefaultUtils.StyleShadow(_shadow, _manager);
            _shadow.enabled = false;
        }

        _selectable = GetComponent<SelectableParent>();
        StartCoroutine(DelayTextOnEnable());
        SetText();
        _manager.OnSettingsChanged += RefreshButton;
    }

    IEnumerator DelayTextOnEnable()
    {
        yield return new WaitForEndOfFrame();
        SetText();
    }

    private void SetText()
    {
        if (getTextFromName)
        {
            _buttonText = name;
        }
        _text.SetTextWithTranslation(_buttonText);
        if (autoFormat)
        {
            _text.Format(_image.ColorTheme, TextSettings.TextStyle.Header);
        }
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        UiSettings.ConnectSettings(ref _manager);
    }

    public int VerticalLayoutGrowth() => growFactor;
    public int HorizontalLayoutGrowth() => growFactor;
    public bool IgnoreLayout() => false;

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SimpleButton))]
public class SimpleButtonEditor : Editor
{
    private SimpleButton button;
    private bool foldout;

    private void OnEnable()
    {
        button = (SimpleButton)target;
        button.SetUp();
    }

    public override void OnInspectorGUI()
    {
        var UIManagerAsset = serializedObject.FindProperty("_manager");
        var buttonText = serializedObject.FindProperty("_buttonText");
        var autoFormat = serializedObject.FindProperty("autoFormat");
        var getTextFromName = serializedObject.FindProperty("getTextFromName");
        var growFactor = serializedObject.FindProperty("growFactor");

        EditorGUI.BeginChangeCheck();
        EditorUtils.DrawProperty(buttonText, "Text", "Button Text");
        EditorUtils.DrawProperty(getTextFromName, "Text From Name", "Enables retrieving text from object name");
        EditorUtils.DrawProperty(autoFormat, "Auto Format", "Enable Formating form Settings");
        EditorUtils.DrawProperty(growFactor, "GrowLayout", "Grow in flexible layout");

        bool updateRequired = EditorGUI.EndChangeCheck();

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
            button.RefreshButton();
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