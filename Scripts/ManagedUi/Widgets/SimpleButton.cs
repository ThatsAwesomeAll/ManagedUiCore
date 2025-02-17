using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
[RequireComponent(typeof(ManagedImage))]
public class SimpleButton : MonoBehaviour
{

    public string _buttonText = "No TEXT";
    public bool autoFormat;
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
        _image ??= GetComponent<ManagedImage>();
        _text ??= GetComponentInChildren<TextMeshProUGUI>();
        if (!_text)
        {
            var textChild = new GameObject("ButtonText");
            textChild.transform.SetParent(transform, false);
            _text = textChild.AddComponent<TextMeshProUGUI>();
        }
    }

    private void SetText()
    {
        _text.text = _buttonText;
        if (autoFormat)
        {
            _manager.SetTextAutoFormat(_text, UiSettings.TextStyle.Header, _image.colorTheme);
        }
    }
    
    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }

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
            var image = serializedObject.FindProperty("_image");

            DrawProperty(buttonText, "Text", "Button Text");
            DrawProperty(autoFormat, "Auto Format", "Enable Formating form Settings");
            DrawProperty(image, "Image", "Background Image");
            

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