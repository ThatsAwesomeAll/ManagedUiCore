using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
public class TabHolder : MonoBehaviour
{
    public List<ManagedTab> Tabs = new List<ManagedTab>();

    private ManagedTab _currentTab;
    
    private void OnEnable()
    {
        Tabs = GetComponentsInChildren<ManagedTab>(true).ToList();
        foreach (var tab in Tabs)
        {
            tab.gameObject.SetActive(false);
        }
    }
    public ManagedTab GetCurrentTab()
    {
        if (!_currentTab)
        {
            var currentIndex = int.MinValue;
            foreach (var tab in Tabs)
            {
                if (tab.OrderIndex > currentIndex)
                {
                    _currentTab = tab;
                }
            }
        }
        return _currentTab;
    }
}
}