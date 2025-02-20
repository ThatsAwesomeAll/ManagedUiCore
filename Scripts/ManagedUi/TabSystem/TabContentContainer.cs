using ManagedUi.GridSystem;
using ManagedUi.SystemInterfaces;
using ManagedUi.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManagedUi.TabSystem
{

[ExecuteInEditMode]
[RequireComponent(typeof(GrowGridLayout))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridSelection))]
[RequireComponent(typeof(UiInputManager))]
public class TabContentContainer : MonoBehaviour
{
    private const string C_contentName = "Content";
    const string C_tabHeaderName = "TabHeader";

    private GrowGridLayout _layout;
    private RectTransform _rectTransform;

    [SerializeField] RectTransform _content;
    [SerializeField] TabHeader _header;
    private GridSelection _gridSelection;
    private ManagedTab currentTab;
    public TabHeader Header => _header;


    private void OnEnable()
    {
        _gridSelection = GetComponent<GridSelection>();
        SetUpRectTransform();
        SetupGridLayout();
        SetupContent();
        SetupTabHeader();
    }

    private void SetUpRectTransform()
    {
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
            // Stretch horizontally, but anchor to the top
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
    private void SetupGridLayout()
    {
        if (!_layout)
        {
            _layout = GetComponent<GrowGridLayout>();
            _layout.direction = GrowGridLayout.GrowDirection.Rows;
            _layout.spacing = new Vector2(0, 10);
        }
    }

    private void SetupContent()
    {
        if (!_content)
        {
            for (int i = 0; i < _rectTransform.childCount; i++)
            {
                var child = _rectTransform.GetChild(i);
                if (child.name == C_contentName)
                {
                    _content = child as RectTransform;
                }
            }
            if (!_content)
            {
                var buttonChild = new GameObject(C_contentName);
                buttonChild.transform.SetParent(transform, false);
                var backgroundImage = buttonChild.AddComponent<ManagedImage>();
                backgroundImage.colorTheme = UiSettings.ColorName.Dark;
                backgroundImage.growth = new Vector2Int(10, 10);
                backgroundImage.SetAsDefaultBackground();
            }
        }
    }
    private void SetupTabHeader()
    {
        _header ??= GetComponentInChildren<TabHeader>();
        if (_header != null)
        {
            return;
        }
        var headerChild = new GameObject(C_tabHeaderName);
        headerChild.transform.SetParent(transform, false);
        _header = headerChild.AddComponent<TabHeader>();
        _header.transform.SetAsFirstSibling();
    }

    public void SetHeader(List<ManagedTab> tabContainerTabs)
    {
        _header.AddTabs(tabContainerTabs);
    }
    
    public void ShowContent(ManagedTab tab)
    {
        if(currentTab) DestroyImmediate(currentTab.gameObject);
        currentTab = Instantiate(tab, _content);
        currentTab.gameObject.SetActive(true);
        _gridSelection.SetupGrid();
    }
}
}