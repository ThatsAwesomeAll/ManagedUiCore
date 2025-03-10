using System;
using UnityEngine;

namespace ManagedUi.Tooltip
{
public class TooltipEvent : ScriptableObject
{
    public Action<string, string> onShow;
    public Action<string, string, RectTransform> onShowWithFixedPosition;
    public Action<TooltipTriggerSender> onHide;

    public float delay = 0.5f;
    public float animationStartSize = 0.5f;
    public float inDuration = 0.2f;
    public float outDuration = 0.2f;

    public enum TooltipTriggerSender
    {
        Default,
        TooltipInternal
    }

    public void ShowTooltip(string title, string text)
    {
        onShow?.Invoke(title, text);
    }

    public void ShowTooltip(string title, string text, RectTransform source)
    {
        onShowWithFixedPosition?.Invoke(title, text, source);
    }

    public void HideTooltip()
    {
        onHide?.Invoke(TooltipTriggerSender.Default);
    }

}
}