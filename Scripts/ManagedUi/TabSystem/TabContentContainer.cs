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
public class TabContentContainer : MonoBehaviour
{
    private const string C_contentName = "Content";
    const string C_tabHeaderName = "TabHeader";

    private GrowGridLayout _layout;

    [SerializeField] TabHolder _tabHolder;
    [SerializeField] TabHeader _header;
    private ManagedTab currentTab;
    public TabHeader Header => _header;


    private void OnEnable()
    {
        SetupGridLayout();
        SetupTabHeader();
        SetupTabHolder();

        _header.AddTabs(_tabHolder.Tabs);
        _header.OnTabSelected += SelectTab;
        currentTab = _tabHolder.GetCurrentTab();
        _header.SetCurrentTab(currentTab);
    }

    private void OnDisable()
    {
        if (_header)
        {
            _header.OnTabSelected -= SelectTab;
        }
    }

    private void SelectTab(ManagedTab obj)
    {
        _tabHolder?.SelectTab(obj);
    }


    private void SetupTabHolder()
    {
        _tabHolder ??= GetComponentInChildren<TabHolder>();
        if (_tabHolder != null)
        {
            return;
        }
        var headerChild = new GameObject(C_contentName);
        headerChild.transform.SetParent(transform, false);
        _tabHolder = headerChild.AddComponent<TabHolder>();
        _tabHolder.transform.SetAsLastSibling();
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

}
}