using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SelectableParent))]
public class Card : MonoBehaviour
{
    
    [Header("Content")]
    public string Title;
    public string Text;
    public Sprite Image;
    public UiSettings.ColorTheme BackgroundColor;
    public UiSettings.ColorTheme TitleColor;
    public UiSettings.ColorTheme TextColor;
    
    [Header("UI Elements")]
    [SerializeField] ManagedImage _image;
    [SerializeField] ManagedImage _background;
    [SerializeField] SelectionAnimation _selectionAnimation;
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _text;
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
            _image.sprite = Image;
        }
        else
        {
            _image.sprite = _manager.DefaultImage();
        }
    }
    
    private void SetupSelectionAnimation()
    {
        var allAnimations = GetComponentsInChildren<SelectionAnimation>();
        foreach (var animation in allAnimations)
        {
            if (animation.name == "SelectionAnimation")
            {
                _selectionAnimation ??= animation;
            }
        }
        if (_selectionAnimation != null)
            return;
        
        var animationChild = new GameObject( "SelectionAnimation");
        animationChild.transform.SetParent(_background.transform, false); 
        var animationTemp = animationChild.AddComponent<SelectionAnimation>();
        animationTemp.images.Add(_background);
        _selectionAnimation = animationTemp;
    }

    private void SetUpAllText()
    {
        var allText = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in allText)
        {
            switch (text.name)
            {
                case "Title":
                    _title ??= text;
                    break;
                case "Text":
                    _text ??= text;
                    break;
            }
        }
        if (!_title)
        {
            _title = CreateText("Title", "No Title", UiSettings.TextStyle.Highlight);
            _title.transform.SetAsLastSibling();
        }
        if (!_text)
        {
            _text = CreateText("Text", "No Content", UiSettings.TextStyle.Text);
            _text.transform.SetAsLastSibling();
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
                    _image ??= image;
                    break;
            }
        }
        if (!_background)
        {
            _background = CreateImage("Background", transform);
            _background.fixColor = true;
            _background.colorTheme = UiSettings.ColorName.Background;
            var layout = _background.AddComponent<GrowGridLayout>();
            layout.spacing = new Vector2(0, 5);
            layout.padding.top = layout.padding.left = layout.padding.right = 20;
            layout.padding.bottom = 10;
            _background.rectTransform.anchorMax = Vector2.one;
            _background.rectTransform.anchorMin = Vector2.zero;
            _background.rectTransform.offsetMin = Vector2.zero;
            _background.rectTransform.offsetMax = Vector2.zero;
        }
        if (!_image)
        {
            _image = CreateImage("Image", _background.transform);
            _image.transform.SetAsFirstSibling();
            _image.growth = Vector2Int.one * 5;
        }
    }
   
    private TextMeshProUGUI CreateText(string textName, string defaultText, UiSettings.TextStyle style)
    {
        var textChild = new GameObject(textName);
        textChild.transform.SetParent(_background.transform, false);
        var text = textChild.AddComponent<TextMeshProUGUI>();
        text.text = defaultText;
        _manager.SetTextAutoFormat(text, style, UiSettings.ColorName.Dark);
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
        if (!_manager) _manager = UiSettings.GetSettings();
    }
}

}