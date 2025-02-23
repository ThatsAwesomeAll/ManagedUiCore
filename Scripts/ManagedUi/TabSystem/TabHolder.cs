using ManagedUi.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
[RequireComponent(typeof(ManagedImage))]
[RequireComponent(typeof(RectTransform))]
public class TabHolder : MonoBehaviour
{
    public List<ManagedTab> _tabs = new List<ManagedTab>();

    private ManagedTab _currentTab;
    private ManagedImage _image;
    private RectTransform _rectTransform;

    private void OnEnable()
    {
        _tabs = GetComponentsInChildren<ManagedTab>(true).ToList();
        SetupBackground();
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;            
        }
    }
    private void SetupBackground()
    {
        if (_image)
        {
            return;
        }
        _image ??= GetComponent<ManagedImage>();
        _image.growth = new Vector2Int(10, 10);
        _image.SetAsDefaultBackground();
        _image.ColorTheme = UiSettings.ColorName.Lighter;
    }
    
    public ManagedTab GetCurrentTab()
    {
        if (!_currentTab)
        {
            var currentIndex = int.MinValue;
            foreach (var tab in _tabs)
            {
                if (tab.OrderIndex > currentIndex)
                {
                    _currentTab = tab;
                }
            }
        }
        foreach (var tab in _tabs)
        {
            tab.gameObject.SetActive(tab == _currentTab);
        } 
        return _currentTab;
    }
    
    public List<ManagedTab> Tabs
    {
        get
        {
            List<ManagedTab> sortedList = new List<ManagedTab>();
            if (_tabs != null)
            {
                sortedList = _tabs.OrderBy(tab => tab.OrderIndex).ToList();
            }
            int lastIndex = int.MinValue;
            foreach (var tab in sortedList)
            {
                if (lastIndex == tab.OrderIndex)
                {
                    tab.OrderIndex++;
                }
                lastIndex = tab.OrderIndex;
            }
            return sortedList;
        }
    }

    public void SelectTab(ManagedTab managedTab)
    {
        if (_currentTab)
        {
            _currentTab.gameObject.SetActive(false);
        }
        _currentTab = managedTab;
        managedTab.gameObject.SetActive(true);
    }
}
}