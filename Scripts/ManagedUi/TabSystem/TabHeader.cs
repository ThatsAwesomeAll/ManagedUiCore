using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using ManagedUi.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private ManagedImage _image;

    public Action<ManagedTab> OnTabSelected;

    private ManagedTab currentTab;
    private Dictionary<ManagedTab, SimpleButton> _currentTabButtons = new Dictionary<ManagedTab, SimpleButton>();

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
    }

    private void SetupBackground()
    {
        if (_image) return;

        _image = GetComponent<ManagedImage>();
        _image.ColorTheme = UiSettings.ColorName.Lighter;
        _image.SetDefaultBackgroundImage();
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
            _right = CreateControlButton(C_buttonNameRight, "$Right$");
        }
        _right.Selectable.OnConfirmed -= SelectNext;
        _right.Selectable.OnConfirmed += SelectNext;
        if (!_left)
        {
            _left = CreateControlButton(C_buttonNameLeft, "$Left$");
        }
        _left.Selectable.OnConfirmed -= SelectPrev;
        _left.Selectable.OnConfirmed += SelectPrev;
    }

    private void SelectNext(SelectableParent _)
    {
        SelectNextTab(false);
    }
    private void SelectPrev(SelectableParent _)
    {
        SelectNextTab(true);
    }
    
    private void SelectNextTab(bool forward)
    {
        if (_currentTabButtons.Count == 0) return;
        ManagedTab selection;
        if (!currentTab)
        {
            selection = _currentTabButtons.Keys.First();
        }
        else
        {
            int targetOrderIndex = ComputeNextOrderIndex(forward);
            selection = _currentTabButtons.First(x => x.Key.OrderIndex == targetOrderIndex).Key;
        }
        TabSelected(selection);
    }

    private int ComputeNextOrderIndex(bool forward)
    {
        List<int> orderIndexes = _currentTabButtons.Keys.Select(tab => tab.OrderIndex).ToList();
        orderIndexes.Sort();
        int currentOrderIndex = orderIndexes.FindIndex(x => x == currentTab.OrderIndex);
        int newIndex = 0;
        if (forward)
        {
            if (currentOrderIndex + 1 < orderIndexes.Count)
            {
                newIndex = currentOrderIndex + 1;
            }
            else
            {
                newIndex = 0;
            }
        }
        else
        {
            if (currentOrderIndex > 0)
            {
                newIndex = currentOrderIndex - 1;
            }
            else
            {
                newIndex = orderIndexes.Count - 1;
            }
        }
        return orderIndexes[newIndex];
    }

    private SimpleButton CreateControlButton(string buttonObjectName, string defaultText)
    {
        var buttonChild = new GameObject(buttonObjectName);
        buttonChild.transform.SetParent(transform, false);
        var button = buttonChild.AddComponent<SimpleButton>();
        button.getTextFromName = false;
        button.Image.FixColor = true;
        button.Image.ColorTheme = UiSettings.ColorName.Dark;
        button.ButtonText = defaultText;
        button.Image.animationEnabled = true;
        button.Image.SelectColor = new ManagedColor(UiSettings.ColorName.Darker);
        button.Image.ConfirmColor = new ManagedColor(UiSettings.ColorName.Accent);
        return button;
    }

    public void AddTabs(List<ManagedTab> tabs)
    {
        ClearTabs(tabs);
        _left.transform.SetAsFirstSibling();
        _currentTabButtons.Clear();

        int index = _left.transform.GetSiblingIndex();
        foreach (var tab in tabs)
        {
            SimpleButton button = FindTabInChildren(tab);
            if (!button)
            {
                button = CreateControlButton(tab.Title, tab.Title);

            }
            SetupTab(button, tab);
            button.transform.SetSiblingIndex(index + 1);
            index = button.transform.GetSiblingIndex();
            _currentTabButtons.Add(tab, button);
        }
        _right.transform.SetAsLastSibling();
    }
    private SimpleButton FindTabInChildren(ManagedTab tab)
    {
        var foundElement = transform.Find(tab.Title);
        return !foundElement ? null : foundElement.GetComponent<SimpleButton>();
    }

    private void SetupTab(SimpleButton button, ManagedTab tab)
    {
        button.Image.ColorTheme = UiSettings.ColorName.Light;
        button.Image.SelectColor = new ManagedColor(UiSettings.ColorName.Lighter);
        button.Image.ConfirmColor = new ManagedColor(UiSettings.ColorName.Accent);
        button.transform.SetSiblingIndex(tab.OrderIndex);
        button.Image.growth = new Vector2Int(3, 3);
        button.SetUp();
        button.Selectable.OnConfirmed += parent =>
        {
            TabSelected(tab);
        };
    }

    private void TabSelected(ManagedTab tab)
    {
        if (currentTab)
        {
            _currentTabButtons[currentTab].SetShadow(false);
        }
        currentTab = tab;
        _currentTabButtons[currentTab].SetShadow(true);
        OnTabSelected?.Invoke(tab);
    }

    private void ClearTabs(List<ManagedTab> tabs)
    {
        int currentChildIndex = 0;
        while(currentChildIndex < transform.childCount)
        {
            var child = transform.GetChild(currentChildIndex);
            if (child.name == C_buttonNameLeft || child.name == C_buttonNameRight)
            {
                currentChildIndex++;
                continue;
            }
            if (tabs.Any(x => x.Title == child.name))
            {
                currentChildIndex++;
                continue;
            }
            DestroyImmediate(transform.GetChild(currentChildIndex).gameObject);
        }
        SetUpControlButton();
    }

    public void SetCurrentTab(ManagedTab managedTab)
    {
        currentTab = managedTab;
        TabSelected(managedTab);
    }
}
}