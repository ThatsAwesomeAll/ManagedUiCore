using UnityEngine;

namespace ManagedUi.Tooltip
{
public class TooltipStaticTrigger : MonoBehaviour
{
    private PrimeTween.Tween _delayTween;

    [SerializeField] private TooltipEvent _event;
    private void Awake()
    {
        TooltipManager.ConnectEvent(ref _event);
    }

    public void RemoveTooltip()
    {
        HandleTooltipStop(_event, ref _delayTween);
    }

    public void SetTooltip(string title, string text)
    {
        HandleTooltipShow(_event, ref _delayTween, title, text);
    }
    
    public void SetTooltipWithPosition(string title, string text, RectTransform source)
    {
        _delayTween.Stop();
        _event?.ShowTooltip(title, text, source);
    }

    public static void HandleTooltipStop(TooltipEvent tooltipEvent, ref PrimeTween.Tween delayTween)
    {
        delayTween.Stop();
        delayTween = PrimeTween.Tween.Delay(tooltipEvent.delay).OnComplete(
            () =>
            {
                tooltipEvent?.HideTooltip();
            });
    }
    public static void HandleTooltipShow(TooltipEvent tooltipEvent, ref PrimeTween.Tween delayTween, string title, string text)
    {
        delayTween.Stop();
        tooltipEvent?.ShowTooltip(title, text); 
    }
    
}
}