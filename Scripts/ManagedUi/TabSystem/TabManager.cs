using System;
using UnityEngine;

namespace ManagedUi.TabSystem
{
public class TabManager : MonoBehaviour
{

    public GameObject HeaderPrefab;
    
    public RectTransform Header;
    public RectTransform LeftIndicator;
    public RectTransform RightIndicator;
    public RectTransform Content;
    public RectTransform TabHolder;

    public void OnEnable()
    {
        if (!TabHolder)
        {
            return;
        }

        var tabs = TabHolder.GetComponentsInChildren<Tab>();

        foreach (var tab in tabs)
        {
            var header = Instantiate(HeaderPrefab, Header);
        }
    }

}
}