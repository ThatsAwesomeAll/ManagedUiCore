using UnityEngine;
using UnityEngine.EventSystems;

namespace ManagedUi.Tooltip
{

[ExecuteInEditMode]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private string _title = "SOMMMEHASDF";
    [SerializeField] private string _text = "aösdlkfjöaskdjföljk öasjdfölk ajsdfa sdf\n asdjöfja kösdflkjas";

    private PrimeTween.Tween _delayTween;

    [SerializeField] private TooltipEvent _event;
    private void Awake()
    {
        TooltipManager.ConnectEvent(ref _event);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipStaticTrigger.HandleTooltipShow(_event, ref _delayTween, _title, _text);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipStaticTrigger.HandleTooltipStop(_event, ref _delayTween);
    }
}
}