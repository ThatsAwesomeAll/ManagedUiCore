using ManagedUi.GridSystem;
using ManagedUi.Selectables;
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
    public string Title = "Card Title";

    public string Text = "This should be some text. Try things out. Get wrapped. Long Long Longer.";
    public Sprite Image;
    public UiSettings.ColorName BackgroundTheme = UiSettings.ColorName.Background;
    public UiSettings.ColorName TitleColor = UiSettings.ColorName.BackgroundDarker;
    public UiSettings.ColorName TextColor = UiSettings.ColorName.Background;

    [Header("UI Elements")]
    [SerializeField] ManagedImage _image;

    [SerializeField] ManagedImage _background;
    [SerializeField] ManagedImage _selectionImage;
    [SerializeField] ManagedText _title;
    [SerializeField] ManagedText _text;
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
        _selectionImage.confirmColor = new ManagedColor( UiSettings.ColorName.Lighter);
    }

    private void SetUpAllText()
    {
        var allText = GetComponentsInChildren<ManagedText>();
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
            _title = CreateText("Title", Title, UiSettings.TextStyle.Highlight, TitleColor);
            _title.transform.SetAsLastSibling();
        }
        if (!_text)
        {
            _text = CreateText("Text", Text, UiSettings.TextStyle.Text, TextColor);
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
            _background.FixColor = true;
            _background.ColorTheme = BackgroundTheme;
            var layout = _background.AddComponent<GrowGridLayout>();
            layout.spacing = new Vector2(0, 5);
            layout.padding.top = layout.padding.left = layout.padding.right = 20;
            layout.padding.bottom = 10;
            StyleDefaultUtils.SetFullScreen(_background.rectTransform);
            _background.animationEnabled = true;
            _background.selectColor = new ManagedColor(UiSettings.ColorName.Background);
            _background.confirmColor = new ManagedColor(UiSettings.ColorName.BackgroundDarker);
        }
        if (!_image)
        {
            _image = CreateImage("Image", _background.transform);
            _image.transform.SetAsFirstSibling();
            _image.growth = Vector2Int.one*5;
        }
    }

    private ManagedText CreateText(string textName, string defaultText, UiSettings.TextStyle style, UiSettings.ColorName background)
    {
        var textChild = new GameObject(textName);
        textChild.transform.SetParent(_background.transform, false);
        var text = textChild.AddComponent<ManagedText>();
        text.SetTextWithTranslation(defaultText);
        text.Format(background);
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