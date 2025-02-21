using ManagedUi.GridSystem;
using ManagedUi.SystemInterfaces;
using UnityEngine;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TabContentContainer))]
[RequireComponent(typeof(GridSelection))]
[RequireComponent(typeof(UiInputManager))]
public class TabManager : MonoBehaviour
{
    private const string C_contentName = "ContentContainer";
    public TabContentContainer Content;
    private RectTransform _rectTransform;
    private GridSelection _gridSelection;

    public void OnEnable()
    {
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;            
        }
        
        _gridSelection = GetComponent<GridSelection>();
        Content ??= GetComponent<TabContentContainer>();
        
        _gridSelection.SetupGrid();
    }
}
}