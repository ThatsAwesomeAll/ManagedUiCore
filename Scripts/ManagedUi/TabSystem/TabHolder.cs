using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
public class TabHolder : MonoBehaviour
{
    public List<ManagedTab> _tabs = new List<ManagedTab>();

    private ManagedTab _currentTab;
    
    private void OnEnable()
    {
        _tabs = GetComponentsInChildren<ManagedTab>(true).ToList();
        foreach (var tab in _tabs)
        {
            tab.gameObject.SetActive(false);
        }
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
            return sortedList;
        }
    }

}
}