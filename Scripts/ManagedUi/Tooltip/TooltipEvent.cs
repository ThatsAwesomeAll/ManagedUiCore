using System;
using UnityEngine;

namespace ManagedUi.Tooltip
{
public class TooltipEvent : ScriptableObject
{
    public Action<string, string> onShow;
    public Action<int> onHide;

    public float delay = 0.5f;
    
    public void ShowTooltip(string title, string text)
    {
        onShow?.Invoke(title,text);
    }

    public void HideTooltip()
    {
        onHide?.Invoke(0);
    }
    
}
}