using ManagedUi.ResourcesLoader;
using ManagedUi.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace ManagedUi.Tooltip
{

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class TooltipManager : MonoBehaviour
{

    [SerializeField] private TooltipEvent _tooltipEvent;
    [SerializeField] private ToolTip _toolTip;

    private PrimeTween.Tween toolTipEffect;
    private GraphicRaycaster _raycaster;

    private void Awake()
    {
        ConnectEvent(ref _tooltipEvent);
    }

    private void OnEnable()
    {
        _tooltipEvent.onShow += ShowTooltip;
        _tooltipEvent.onShowWithFixedPosition += ShowTooltipWithPosition;
        _tooltipEvent.onHide += HideTooltip;
        _raycaster ??= GetComponent<GraphicRaycaster>();
        if (_raycaster && _raycaster.enabled)
        {
            Debug.LogError("Raycaster Disabled for tooltip Canvas. This is not supposed to happen." + this.gameObject.name);
            _raycaster.enabled = false;
        }
    }


    private void OnDisable()
    {
        _tooltipEvent.onShow -= ShowTooltip;
        _tooltipEvent.onShowWithFixedPosition -= ShowTooltipWithPosition;
        _tooltipEvent.onHide -= HideTooltip;
    }

    private void ShowTooltip(string title, string text)
    {
        if (!_toolTip) return;
        
        _toolTip.gameObject.SetActive(true);
        _toolTip.transform.localScale = Vector3.one*_tooltipEvent.animationStartSize;
        toolTipEffect.Stop();
        toolTipEffect = PrimeTween.Tween.Scale(_toolTip.transform, Vector3.one, _tooltipEvent.inDuration, useUnscaledTime: true);
        _toolTip._disablePosition = false;
        _toolTip.SetText(text, title);
    }
    
    private void ShowTooltipWithPosition(string title, string text, RectTransform source)
    {
        if (!_toolTip) return;

        _toolTip.gameObject.SetActive(true);
        _toolTip.transform.localScale = Vector3.one*_tooltipEvent.animationStartSize;
        toolTipEffect.Stop();
        toolTipEffect = PrimeTween.Tween.Scale(_toolTip.transform, Vector3.one, _tooltipEvent.inDuration, useUnscaledTime: true);
        _toolTip._disablePosition = false; 
        _toolTip.SetText(text, title, source);
    }
    
    private void HideTooltip(TooltipEvent.TooltipTriggerSender obj)
    {
        if (_toolTip._currentlyHovered) return;
        if (obj == TooltipEvent.TooltipTriggerSender.Default)
        {
            _toolTip._disablePosition = true;
        }
        if (obj != TooltipEvent.TooltipTriggerSender.TooltipInternal)
        {
            toolTipEffect.Stop();
        }
        toolTipEffect = PrimeTween.Tween.Scale(_toolTip.transform, Vector3.one*_tooltipEvent.animationStartSize, _tooltipEvent.outDuration, useUnscaledTime: true).OnComplete(
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