using ManagedUi.GridSystem;
using ManagedUi.SystemInterfaces;
using System;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TabContentContainer))]
[RequireComponent(typeof(GridSelection))]
[RequireComponent(typeof(UiInputManager))]
public class TabManager : MonoBehaviour
{
    private const string C_contentName = "ContentContainer";
    public TabContentContainer Content;
    private RectTransform _rectTransform;
    private GridSelection _gridSelection;

    private ManagedTab _currentTab;
    public ManagedTab currentTab => _currentTab;
    public Action<ManagedTab> OnTabChanged;

    public void OnEnable()
    {
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;            
        }
        
        _gridSelection = GetComponent<GridSelection>();
        Content ??= GetComponent<TabContentContainer>();
        Content.onSeletedTabChanged += UpdateGrid;
        _currentTab = Content.CurrentTab;
        _gridSelection.SetupGrid(true);
    }
    
    public void OnDisable()
    {
        Content.onSeletedTabChanged -= UpdateGrid;
    }
    
    private void UpdateGrid(ManagedTab obj)
    {
        _currentTab = obj;
        _gridSelection?.SetupGrid(false);
        OnTabChanged?.Invoke(obj);
    }
}
}