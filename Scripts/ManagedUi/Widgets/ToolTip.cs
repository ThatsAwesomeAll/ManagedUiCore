using ManagedUi.Settings;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ManagedUi.Widgets
{
[ExecuteInEditMode]
[RequireComponent(typeof(ContentSizeFitter))]
[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(ManagedImage))]
public class ToolTip : MonoBehaviour
{

    private const string C_GameObjectTitle = "Title";
    private const string C_GameObjectBoarder = "Boarder";
    private const string C_GameObjectText = "Text";
    
    [SerializeField] private VerticalLayoutGroup _layoutGroup;
    [SerializeField] private ContentSizeFitter _contentFitter;
    [SerializeField] private LayoutElement _layoutElement;
    
    [SerializeField] private ManagedText _text;
    [SerializeField] private ManagedText _title;
    
    [SerializeField] private ManagedImage _boarder;
    [SerializeField] private ManagedImage _background;

    public int characterLimitTitle = 30;
    public int characterLimitText = 50;
    public void SetText(string text, string title)
    {
        _text.SetTextWithTranslation(text);
        if (String.IsNullOrEmpty(title))
        {
            return;
        }
        _title.SetTextWithTranslation(title);
    }

    private void OnEnable()
    {
        if (!_layoutGroup)
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
            _layoutGroup.childControlHeight = true;
            _layoutGroup.childControlWidth = true;
            _layoutGroup.childForceExpandHeight = false;
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childAlignment = TextAnchor.UpperLeft;
            _layoutGroup.spacing = 0;
            _layoutGroup.padding = new RectOffset(20, 20, 10, 10);
        }
        if (!_contentFitter)
        {
            _contentFitter = GetComponent<ContentSizeFitter>();
            _contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        if (!_layoutElement)
        {
            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.preferredWidth = 400;
        }
        SetUpAllText();
        SetupAllImages();
    }
    
    private void SetupAllImages()
    {
        var allText = GetComponentsInChildren<ManagedImage>();
        foreach (var text in allText)
        {
            switch (text.name)
            {
                case C_GameObjectBoarder:
                    _boarder ??= text;
                    break;
            }
        }
        if (!_boarder)
        {
            var textChild = new GameObject(C_GameObjectBoarder);
            textChild.transform.SetParent(transform, false);
            var image = textChild.AddComponent<ManagedImage>(); 
            var layout = textChild.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            StyleDefaultUtils.SetFullScreen(image.rectTransform);
            StyleDefaultUtils.StyleBoarder(image,_manager);
        }
        if (!_background)
        {
            _background = GetComponent<ManagedImage>();
            _background.SetDefaultBackgroundImage();
        }
    }

    private void SetUpAllText()
    {
        var allText = GetComponentsInChildren<ManagedText>();
        foreach (var text in allText)
        {
            switch (text.name)
            {
                case C_GameObjectTitle:
                    _title ??= text;
                    break;
                case C_GameObjectText:
                    _text ??= text;
                    break;
            }
        }
        if (!_title)
        {
            _title = CreateText(C_GameObjectTitle, "Title", TextSettings.TextStyle.Highlight);
            _title.transform.SetAsLastSibling();
        }
        if (!_text)
        {
            _text = CreateText(C_GameObjectText, "Tooltip long text and even longer and may take multi line", TextSettings.TextStyle.Text);
            _text.transform.SetAsLastSibling();
        }
    }
    private ManagedText CreateText(string textName, string defaultText, TextSettings.TextStyle style)
    {
        var textChild = new GameObject(textName);
        textChild.transform.SetParent(transform, false);
        var text = textChild.AddComponent<ManagedText>();
        text.SetTextWithTranslation(defaultText);
        text.Format(UiSettings.ColorName.Dark, style);
        return text;
    } 
    private void Update()
    {
        int headerLength = _title.Text.Length;
        int textLength = _text.Text.Length;

        _layoutElement.enabled = (headerLength > characterLimitTitle || textLength > characterLimitText);
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
}