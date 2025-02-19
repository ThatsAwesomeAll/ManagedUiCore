using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
[RequireComponent(typeof(ManagedImage))]
[RequireComponent(typeof(SelectableParent))]
public class SimpleButton : MonoBehaviour, IGridElement
{

    private const string C_ButtonTextObjectName = "ButtonText";
    
    public string _buttonText = "No TEXT";
    public bool getTextFromName = true;
    public bool autoFormat = true;
    public int growFactor = 1;
    public ManagedImage Image => _image;
    
    private SelectableParent _selectable;
    public SelectableParent Selectable => _selectable;
    private ManagedImage _image;
    private TextMeshProUGUI _text;

    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            SetText();
        }
    }

    protected void Awake()
    {
        SetUp();
    }

    private void OnEnable()
    {
        if (!_image)
        {
            _image = GetComponent<ManagedImage>();
            _image.colorTheme = UiSettings.ColorName.Main;
            _image.SetAsDefaultBackground();
        }
        _text ??= GetComponentInChildren<TextMeshProUGUI>();
        if (!_text)
        {
            var textChild = new GameObject(C_ButtonTextObjectName);
            textChild.transform.SetParent(transform, false);
            _text = textChild.AddComponent<TextMeshProUGUI>();
        }
        if (getTextFromName)
        {
            _buttonText = name;
        }
        _selectable = GetComponent<SelectableParent>();
        SetText();
    }

    private void SetText()
    {
        _text.text = _buttonText;
        if (autoFormat)
        {
            _manager.SetTextAutoFormat(_text, UiSettings.TextStyle.Highlight, _image.colorTheme);
        }
    }
    
    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }

    public int VerticalLayoutGrowth() => growFactor;
    public int HorizontalLayoutGrowth() => growFactor;
    public bool IgnoreLayout() => false;
}

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SimpleButton))]
    public class SimpleButtonEditor : Editor
    {
        private SimpleButton image;

        private void OnEnable()
        {
            image = (SimpleButton)target;
            image.SetUp();
        }

        void DrawProperty(SerializedProperty property, string content, string tooltip)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField(new GUIContent(content, tooltip), GUILayout.Width(120));
            if (property != null)
            {
                EditorGUILayout.PropertyField(property, new GUIContent("", tooltip));
            }

            GUILayout.EndHorizontal();
        }

        void DrawCustomHeader()
        {
            GUILayout.Space(2);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("_manager");
            var buttonText = serializedObject.FindProperty("_buttonText");
            var autoFormat = serializedObject.FindProperty("autoFormat");
            var getTextFromName = serializedObject.FindProperty("getTextFromName");
            var growFactor = serializedObject.FindProperty("growFactor");

            DrawProperty(buttonText, "Text", "Button Text");
            DrawProperty(getTextFromName, "Text From Name", "Enables retrieving text from object name");
            DrawProperty(autoFormat, "Auto Format", "Enable Formating form Settings");
            DrawProperty(growFactor, "GrowLayout", "Grow in flexible layout");
            

            if (UIManagerAsset != null)
            {
                DrawProperty(UIManagerAsset, "Manager Asset", "Dont change this");
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("NO MANAGER FOUND"), GUILayout.Width(120));
            }

            serializedObject.ApplyModifiedProperties();
            DrawCustomHeader();
        }
    }
#endif
}