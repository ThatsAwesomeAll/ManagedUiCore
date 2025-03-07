using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using ManagedUi.Settings;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SelectableParent))]
public class Card : MonoBehaviour
{

    [Header("Content")]
    public string Title = "Card Title";
    public string Text = "This should be some text. Try things out. Get wrapped. Long Long Longer.";
    public Sprite Image;
    public UiSettings.ColorName BackgroundTheme = UiSettings.ColorName.Dark;
    public UiSettings.ColorName TitleColor = UiSettings.ColorName.Dark;
    public UiSettings.ColorName TextColor = UiSettings.ColorName.Dark;

    [Header("UI Elements")]
    [SerializeField] ManagedImage _imageHolder;
    [SerializeField] ManagedImage _background;
    [SerializeField] ManagedImage _selectionImage;
    [SerializeField] ManagedText _titleTextBox;
    [SerializeField] ManagedText _textTextBox;
    [SerializeField] RectTransform _rect;

 
    protected void Awake()
    {
        if (!_rect)
        {
            _rect = GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(300, 500);
        }
        SetUp();
    }

    private void OnEnable()
    {
        SetUp();
        SetUpImages();
        SetUpAllText();
        SetupSelectionAnimation();
        SetContent();
    }

    private void SetContent()
    {
        if (Image)
        {
            _imageHolder.sprite = Image;
        }
        else
        {
            _imageHolder.sprite = _manager.DefaultImage();
        }
    }
    
    public void RefreshContent()
    {
        SetContent();
        _textTextBox.SetTextWithTranslation(Text);
        _titleTextBox.SetTextWithTranslation(Title);
        _background.BasicColor.SetColorByTheme(BackgroundTheme,_manager);
        _textTextBox.SetBasicColorTheme(TextColor);
        _titleTextBox.SetBasicColorTheme(TitleColor);
    }

    private void SetupSelectionAnimation()
    {
        var allAnimations = GetComponentsInChildren<ManagedImage>();
        foreach (var animation in allAnimations)
        {
            if (animation.name == "SelectionAnimation")
            {
                _selectionImage ??= animation;
            }
        }
        if (_selectionImage != null)
            return;

        var animationChild = new GameObject("SelectionAnimation");
        animationChild.transform.SetParent(_background.transform, false);
        var animationTemp = animationChild.AddComponent<ManagedImage>();
        animationTemp.sprite = _manager.DefaultSelectionImage();
        StyleDefaultUtils.StyleSelectionMarker(animationTemp);
        _selectionImage = animationTemp;
        _selectionImage.enabled = false;
        _selectionImage.ConfirmColor = new ManagedColor(UiSettings.ColorName.Lighter);
    }

    private void SetUpAllText()
    {
        var allText = GetComponentsInChildren<ManagedText>();
        foreach (var text in allText)
        {
            switch (text.name)
            {
                case "Title":
                    _titleTextBox ??= text;
                    break;
                case "Text":
                    _textTextBox ??= text;
                    break;
            }
        }
        if (!_titleTextBox)
        {
            _titleTextBox = CreateText("Title", Title, TextSettings.TextStyle.Highlight, TitleColor);
            _titleTextBox.transform.SetAsLastSibling();
        }
        if (!_textTextBox)
        {
            _textTextBox = CreateText("Text", Text, TextSettings.TextStyle.Text, TextColor);
            _textTextBox.transform.SetAsLastSibling();
        }
    }

    private void SetUpImages()
    {
        var allImages = GetComponentsInChildren<ManagedImage>();
        foreach (var image in allImages)
        {
            switch (image.name)
            {
                case "Background":
                    _background ??= image;
                    break;
                case "Image":
                    _imageHolder ??= image;
                    break;
            }
        }
        if (!_background)
        {
            _background = CreateImage("Background", transform);
            _background.FixColor = true;
            _background.ColorTheme = UiSettings.ColorName.Light;
            var layout = _background.gameObject.AddComponent<GrowGridLayout>();
            layout.spacing = new Vector2(0, 5);
            layout.padding.top = layout.padding.left = layout.padding.right = 20;
            layout.padding.bottom = 10;
            StyleDefaultUtils.SetFullScreen(_background.rectTransform);
            _background.SetDefaultBackgroundImage();
            StyleDefaultUtils.ActiveDefaultButtonAnimation(_background);
        }
        if (!_imageHolder)
        {
            _imageHolder = CreateImage("Image", _background.transform);
            _imageHolder.transform.SetAsFirstSibling();
            _imageHolder.growth = Vector2Int.one*5;
        }
    }

    private ManagedText CreateText(string textName, string defaultText, TextSettings.TextStyle style, UiSettings.ColorName background)
    {
        var textChild = new GameObject(textName);
        textChild.transform.SetParent(_background.transform, false);
        var text = textChild.AddComponent<ManagedText>();
        text.SetTextWithTranslation(defaultText);
        text.Format(background, style);
        return text;
    }
    private ManagedImage CreateImage(string imageName, Transform parent)
    {
        var textChild = new GameObject(imageName);
        textChild.transform.SetParent(parent, false);
        return textChild.AddComponent<ManagedImage>();
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        UiSettings.ConnectSettings(ref _manager);
    }

}
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private Card card;

    private void OnEnable()
    {
        card = (Card) target;
        card.SetUp();
    }


    public override void OnInspectorGUI()
    {

        var UIManagerAsset = serializedObject.FindProperty("_manager");
        
        var _title = serializedObject.FindProperty("Title");
        var _text = serializedObject.FindProperty("Text");
        var Image = serializedObject.FindProperty("Image");
        var _backgroundTheme = serializedObject.FindProperty("BackgroundTheme");
        var _textColor = serializedObject.FindProperty("TextColor");
        var _titleColor = serializedObject.FindProperty("TitleColor");


        EditorGUI.BeginChangeCheck();
        EditorUtils.DrawProperty(_title, "Title", "enable automatic animation");
        EditorUtils.DrawProperty(_text, "Text", "enable automatic animation");
        
        EditorUtils.DrawProperty(Image, "Image", "enable automatic animation");
        EditorUtils.DrawProperty(_backgroundTheme, "Color", "enable automatic animation");
        EditorUtils.DrawProperty(_textColor, "Title Theme", "enable automatic animation");
        EditorUtils.DrawProperty(_titleColor, "Text Theme", "enable automatic animation");


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
            card.RefreshContent();
        }
        EditorUtils.DrawCustomHeader();
    }
}
#endif
}