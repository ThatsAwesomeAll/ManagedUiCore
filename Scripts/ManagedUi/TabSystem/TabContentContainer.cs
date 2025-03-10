using ManagedUi.GridSystem;
using ManagedUi.SystemInterfaces;
using ManagedUi.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ManagedUi.TabSystem
{

[ExecuteInEditMode]
[RequireComponent(typeof(GrowGridLayout))]
public class TabContentContainer : MonoBehaviour
{
    private const string C_contentName = "Content";
    const string C_tabHeaderName = "TabHeader";

    private GrowGridLayout _layout;

    [SerializeField] private TabHolder tabHolder;
    [SerializeField] private TabHeader header;
    public Action<ManagedTab> onSeletedTabChanged;

    private ManagedTab _currentTab;
    public ManagedTab CurrentTab => _currentTab;

    private void OnEnable()
    {
        SetupGridLayout();
        SetupTabHeader();
        SetupTabHolder();

        header.AddTabs(tabHolder.Tabs);
        header.OnTabSelected += SelectTab;
        _currentTab = tabHolder.GetCurrentTab();
        header.SetCurrentTab(_currentTab);
        onSeletedTabChanged?.Invoke(_currentTab);
    }

    private void OnDisable()
    {
        if (header)
        {
            header.OnTabSelected -= SelectTab;
        }
    }

    private void SelectTab(ManagedTab obj)
    {
        tabHolder?.SelectTab(obj);
        onSeletedTabChanged?.Invoke(obj);
    }


    private void SetupTabHolder()
    {
        tabHolder ??= GetComponentInChildren<TabHolder>();
        if (tabHolder != null)
        {
            return;
        }
        var headerChild = new GameObject(C_contentName);
        headerChild.transform.SetParent(transform, false);
        tabHolder = headerChild.AddComponent<TabHolder>();
        tabHolder.transform.SetAsLastSibling();
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

    private void SetupTabHeader()
    {
        header ??= GetComponentInChildren<TabHeader>();
        if (header != null)
        {
            return;
        }
        var headerChild = new GameObject(C_tabHeaderName);
        headerChild.transform.SetParent(transform, false);
        header = headerChild.AddComponent<TabHeader>();
        header.transform.SetAsFirstSibling();
    }

}
}