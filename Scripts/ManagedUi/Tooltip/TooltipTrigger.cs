using UnityEngine;
using UnityEngine.EventSystems;

namespace ManagedUi.Tooltip
{

[ExecuteInEditMode]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string title = "SOMMMEHASDF";
    public string text = "aösdlkfjöaskdjföljk öasjdfölk ajsdfa sdf\n asdjöfja kösdflkjas";

    private PrimeTween.Tween _delayTween;

    [SerializeField] private TooltipEvent _event;
    private void Awake()
    {
        TooltipManager.ConnectEvent(ref _event);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _delayTween.Stop();
        _event?.ShowTooltip(title, text);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _delayTween = PrimeTween.Tween.Delay(_event.delay).OnComplete(
            () =>
            {
                _event?.HideTooltip();
            }
        );
    }


}
}