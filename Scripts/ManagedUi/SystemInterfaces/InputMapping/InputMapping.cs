using System;
using UnityEngine;

namespace ManagedUi.SystemInterfaces.InputMapping
{

[CreateAssetMenu(fileName = "InputMapping", menuName = "UiManager/InputMapping")]
public class InputMapping : ScriptableObject
{

    [Serializable]
    public struct InputUiDefinition
    {
        public string buttonRawPath;
        public InputVisualization visualization;
    }
    
    [Serializable]
    public struct InputVisualization
    {
        public int spriteId;
        public Sprite isolatedSprite;
    }
    
    public InputUiDefinition inputToUiMapping;
}



}