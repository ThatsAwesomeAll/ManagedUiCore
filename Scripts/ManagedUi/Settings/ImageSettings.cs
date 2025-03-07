using UnityEngine;

namespace ManagedUi
{

[CreateAssetMenu(fileName = "UiManager", menuName = "UiManager/ImageSettings")]
public class ImageSettings : ScriptableObject
{
    [Header("Default Image settings")]
    [SerializeField] Sprite _defaultImage;
    [SerializeField] Sprite _defaultShadowImage;

    [SerializeField] Sprite _defaultSelectionImage;
    [SerializeField] private float _defaultSelectionImageSliceFactor = 0.3f;

    [SerializeField] Sprite _defaultBackgroundImage;
    [SerializeField] private float _defaultBackgroundImageSliceFactor = 0.3f;
    
    [SerializeField] Sprite _defaultBoarderImage;
    [SerializeField] private float _defaultBoarderImageSliceFactor = 3.0f;

    public float DefaultBackgroundImageSliceFactor => _defaultBackgroundImageSliceFactor;
    public float DefaultSelectionImageSliceFactor => _defaultSelectionImageSliceFactor;
    public float DefaultBoarderImageSliceFactor => _defaultBoarderImageSliceFactor;
    public Sprite DefaultImage() => _defaultImage;
    public Sprite DefaultBackgroundImage() => _defaultBackgroundImage;
    public Sprite DefaultSelectionImage() => _defaultSelectionImage;
    public Sprite DefaultShadowImage() => _defaultShadowImage;
    public Sprite DefaultBoarderImage() => _defaultBoarderImage;
}
}