using ManagedUi.GridSystem;
using ManagedUi.Widgets;
using System;
using UnityEngine;

namespace ManagedUi.TabSystem
{

[ExecuteInEditMode]
[RequireComponent(typeof(GrowGridLayout))]
[RequireComponent(typeof(RectTransform))]
public class TabContentContainer : MonoBehaviour
{
    private const string C_contentName = "Content";
    const string C_tabHeaderName = "TabHeader";
 
    private GrowGridLayout _layout;
    private RectTransform _rectTransform;
    
    [SerializeField] RectTransform _content;
    [SerializeField] TabHeader _header;
    public TabHeader Header => _header;


    private void OnEnable()
    {
        if (!_rectTransform)
        {
            _rectTransform = GetComponent<RectTransform>();
            // Stretch horizontally, but anchor to the top
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        if (!_layout)
        {
            _layout = GetComponent<GrowGridLayout>();
            _layout.direction = GrowGridLayout.GrowDirection.Rows;
        }

        SetupContent();
        SetupTabHeader();
            
    }

    private void SetupContent()
    {
        if (!_content)
        {
            for (int i = 0; i < _rectTransform.childCount; i++)
            {
                var child = _rectTransform.GetChild(i);
                if (child.name == "Content")
                {
                    _content = child as RectTransform;
                }
            }
            if (!_content)
            {
                var buttonChild = new GameObject(C_contentName);
                buttonChild.transform.SetParent(transform, false);
                var button = buttonChild.AddComponent<ManagedImage>();
                button.colorTheme = UiSettings.ColorName.Dark;
                button.fixColor = true;
                button.growth = new Vector2Int(10, 10);
            }
        }
    }
    private void SetupTabHeader()
    {
        _header ??= GetComponentInChildren<TabHeader>();
        if (_header != null)
        {
            return;
        }
        var headerChild = new GameObject(C_tabHeaderName);
        headerChild.transform.SetParent(transform, false);
        _header = headerChild.AddComponent<TabHeader>();
        _header.transform.SetAsFirstSibling();
    }

}
}