using UnityEngine;

namespace ManagedUi.TabSystem
{
[RequireComponent(typeof(RectTransform))]
public class ManagedTab : MonoBehaviour
{
    public string Title = "No Title";
    public int OrderIndex = 0;
    
}
}