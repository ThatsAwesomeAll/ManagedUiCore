using UnityEngine;
using UnityEngine.Serialization;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class TabManager : MonoBehaviour
{
    private const string C_contentName = "ContentContainer";
    public TabContentContainer Content;
    private RectTransform _rectTransform;

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
        
        Content ??= GetComponentInChildren<TabContentContainer>();
        if (!Content)
        {
            var buttonChild = new GameObject(C_contentName);
            buttonChild.transform.SetParent(transform, false);
            Content = buttonChild.AddComponent<TabContentContainer>();
        }
    }
}
}