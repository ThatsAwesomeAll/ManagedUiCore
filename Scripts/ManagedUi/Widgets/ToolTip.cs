using ManagedUi.Settings;
using ManagedUi.Tooltip;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ManagedUi.Widgets
{
[ExecuteInEditMode]
[RequireComponent(typeof(ContentSizeFitter))]
[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(ManagedImage))]
public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    private RectTransform _rectTransform;
    private Canvas _canvas;


    public int characterLimitTitle = 30;
    public int characterLimitText = 50;
    public bool _currentlyHovered = false;
    public bool _disablePosition = false;

    private void ActivateText(string text, ManagedText textfield)
    {
        if (String.IsNullOrEmpty(text))
        {
            textfield.gameObject.SetActive(false);
        }
        else
        {
            textfield.gameObject.SetActive(true);
            textfield.SetTextWithTranslation(text);
        }
        UpdateLayout();
    }

    public void SetText(string text, string title)
    {
        ActivateText(title, _title);
        ActivateText(text, _text);
    }

    public void SetText(string text, string title, RectTransform source)
    {
        SetText(text, title);
        _disablePosition = true;
        var target = source.transform.position;
        target.x += source.pivot.x*source.rect.width;
        target.y += source.pivot.y*source.rect.height;
        SetPosition(source.transform.position);
    }

    private void SetPosition(Vector3 targetPosition)
    {
        if (targetPosition.x + _rectTransform.rect.width > _canvas.pixelRect.width)
        {
            targetPosition.x = _canvas.pixelRect.width - _rectTransform.rect.width;
        }
        if (targetPosition.x < 0f)
        {
            targetPosition.x = 0f;
        }
        if (targetPosition.y - _rectTransform.rect.height < 0f)
        {
            targetPosition.y = _rectTransform.rect.height;
        }
        if (targetPosition.y > _canvas.pixelRect.height)
        {
            targetPosition.y = _canvas.pixelRect.height;
        }
        transform.position = targetPosition;
    }

    private void OnEnable()
    {
        _rectTransform ??= GetComponent<RectTransform>();
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
        _canvas = GetComponentInParent<Canvas>();
        SetUpAllText();
        SetupAllImages();
        _currentlyHovered = false;
        _disablePosition = false;
    }

    private void OnDisable()
    {
        _currentlyHovered = false;
        _disablePosition = true;
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
            StyleDefaultUtils.StyleBoarder(image, _manager);
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
        if (!Application.isPlaying)
        {
            return;
        }
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_currentlyHovered || _disablePosition)
        {
            return;
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        SetPosition(mousePos);
    }


    private void UpdateLayout()
    {
        int headerLength = _title.Text.Length;
        int textLength = _text.Text.Length;
        _layoutElement.enabled = (headerLength > characterLimitTitle || textLength > characterLimitText);
    }

    void Awake()
    {
        SetUp();
        TooltipManager.ConnectEvent(ref _event);
    }
    [SerializeField] private UiSettings _manager;
    [SerializeField] private TooltipEvent _event;
    public void SetUp()
    {
        UiSettings.ConnectSettings(ref _manager);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _currentlyHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _currentlyHovered = false;
        _disablePosition = true;
        _event?.HideTooltip();

    }
}
}