using UnityEngine;
using UnityEngine.Serialization;

namespace ManagedUi.TabSystem
{
[ExecuteInEditMode]
public class TabManager : MonoBehaviour
{
    const string C_tabHeaderName = "TabHeader";

    [SerializeField] TabHeader header;
    public RectTransform Content;
    public RectTransform TabHolder;

    public void OnEnable()
    {
        SetupTabHeader();

        if (!TabHolder)
        {
            return;
        }
        var tabs = TabHolder.GetComponentsInChildren<Tab>();
    }
    private void SetupTabHeader()
    {
        header ??= GetComponentInChildren<TabHeader>();
        header.SetupSize(100f);
        if (header != null)
        {
            return;
        }
        var headerChild = new GameObject(C_tabHeaderName);
        headerChild.transform.SetParent(transform, false);
        header = headerChild.AddComponent<TabHeader>();
        header.transform.SetAsFirstSibling();
        header.SetupSize(100f);
    }

}
}