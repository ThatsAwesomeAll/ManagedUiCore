using ManagedUi.GridSystem;
using ManagedUi.Widgets;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
[RequireComponent(typeof(ManagedImage))]
[RequireComponent(typeof(GrowGridLayout))]
[RequireComponent(typeof(RectTransform))]
public class TabHeader : MonoBehaviour
{
    private const string C_buttonNameRight = "RightControlButton";
    private const string C_buttonNameLeft = "LeftControlButton";

    [SerializeField] private SimpleButton _right;
    [SerializeField] private SimpleButton _left;

    private RectTransform _rectTransform;
    private GrowGridLayout _grid;
    private ManagedImage _image;

    public void OnEnable()
    {
        SetupSize(100f);
        SetupGrid();
        SetupBackground();
        SetUpControlButton();
    }

    private void SetupGrid()
    {
        if (_grid) return;

        _grid = GetComponent<GrowGridLayout>();
        _grid.direction = GrowGridLayout.GrowDirection.Column;
        _grid.spacing.x = 20f;
        _grid.padding.left = 10;
        _grid.padding.top = 10;
        _grid.padding.right = 10;
        _grid.padding.bottom = 10;
    }

    private void SetupBackground()
    {
        if (_image) return;

        _image = GetComponent<ManagedImage>();
        _image.fixColor = true;
        _image.colorTheme = UiSettings.ColorName.Light;
    }

    private void SetupSize(float height)
    {
        if (_rectTransform) return;
        _rectTransform = GetComponent<RectTransform>();
        // Stretch horizontally, but anchor to the top
        _rectTransform.anchorMin = new Vector2(0, 1);
        _rectTransform.anchorMax = new Vector2(1, 1);

        // Set height to 100 pixels
        _rectTransform.sizeDelta = new Vector2(0, height);

        // Make sure it's aligned to the top without any offset
        _rectTransform.pivot = new Vector2(0.5f, 1);
        _rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    private void SetUpControlButton()
    {
        var allButtons = GetComponentsInChildren<SimpleButton>();
        foreach (var button in allButtons)
        {
            switch (button.name)
            {
                case C_buttonNameRight:
                    _right ??= button;
                    break;
                case C_buttonNameLeft:
                    _left ??= button;
                    break;
            }
        }
        if (!_right)
        {
            _right = CreateControlButton(C_buttonNameRight, "Right");
            _right.Selectable.OnConfirmed += parent =>
            {
                Debug.Log("Right Control Confirmed");
            };
        }
        if (!_left)
        {
            _left = CreateControlButton(C_buttonNameLeft, "Left");
            _left.transform.SetAsLastSibling();
            _left.Selectable.OnConfirmed += parent =>
            {
                Debug.Log("Left Control Confirmed");
            };
        }
    }

    private SimpleButton CreateControlButton(string buttonObjectName, string defaultText)
    {
        var buttonChild = new GameObject(buttonObjectName);
        buttonChild.transform.SetParent(transform, false);
        var button = buttonChild.AddComponent<SimpleButton>();
        button.getTextFromName = false;
        button.Image.SetColorByTheme(UiSettings.ColorName.Darker);
        button.ButtonText = defaultText;
        return button;
    }


}
}