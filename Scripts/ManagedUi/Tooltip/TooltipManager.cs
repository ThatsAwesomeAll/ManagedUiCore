using ManagedUi.ResourcesLoader;
using ManagedUi.Widgets;
using UnityEngine;

namespace ManagedUi.Tooltip
{

[ExecuteInEditMode]
public class TooltipManager : MonoBehaviour
{

    [SerializeField] private TooltipEvent _tooltipEvent;
    [SerializeField] private ToolTip _toolTip;

    private PrimeTween.Tween toolTipEffect;

    private void Awake()
    {
        ConnectEvent(ref _tooltipEvent);
    }

    private void OnEnable()
    {
        _tooltipEvent.onShow += ShowTooltip;
        _tooltipEvent.onHide += HideTooltip;
    }

    private void OnDisable()
    {
        _tooltipEvent.onShow -= ShowTooltip;
        _tooltipEvent.onHide -= HideTooltip;
    }

    private void ShowTooltip(string title, string text)
    {
        _toolTip.gameObject.SetActive(true);
        _toolTip.SetText(text, title);
        _toolTip.transform.localScale = Vector3.one*_tooltipEvent.animationStartSize;
        toolTipEffect.Stop();
        toolTipEffect = PrimeTween.Tween.Scale(_toolTip.transform, Vector3.one, _tooltipEvent.inDuration);
    }
    private void HideTooltip(int obj)
    {
        if (_toolTip._currentlyHovered) return;
        toolTipEffect.Stop();
        toolTipEffect = PrimeTween.Tween.Scale(_toolTip.transform, Vector3.one*_tooltipEvent.animationStartSize, _tooltipEvent.outDuration).OnComplete(
            () =>
            {
                _toolTip._currentlyHovered = false;
                _toolTip.gameObject.SetActive(false);
            });
    }



    public static void ConnectEvent(ref TooltipEvent settings)
    {
        if (settings != null)
        {
            return;
        }
        settings = SettingsLoader.GetTooltipEvent();
    }

}
}