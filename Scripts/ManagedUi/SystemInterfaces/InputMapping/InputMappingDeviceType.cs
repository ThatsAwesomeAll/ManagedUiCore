using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.SystemInterfaces.InputMapping
{
[CreateAssetMenu(fileName = "InputMapping", menuName = "UiManager/InputMappingDevice")]
public class InputMappingDeviceType : ScriptableObject
{

    [SerializeField] private InputMappingManager.ControlSchema _deviceSchema;
    public InputMappingManager.ControlSchema DeviceSchema => _deviceSchema;
    public string DeviceName => _deviceSchema.ToString();

    [SerializeField] private List<InputMapping> InputUiMapping = new List<InputMapping>();

    [SerializeField] private Color _deviceColor;
    public Color DeviceColor => _deviceColor;

    public InputActionAsset DefaultAssetToCreateDummy;

    public InputMapping.InputVisualization GetInputVisualization(InputBinding binding)
    {
        foreach (var uiInputMapping in InputUiMapping.Where(uiInputMapping => binding.path == uiInputMapping.inputToUiMapping.buttonRawPath))
        {
            return uiInputMapping.inputToUiMapping.visualization;
        }
        return default;
    }
#if UNITY_EDITOR
    [ContextMenu("Process Input Asset")]
    public void ProcessInputAsset()
    {
        if (!DefaultAssetToCreateDummy) return;

        foreach (var actionMap in DefaultAssetToCreateDummy.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                foreach (var binding in action.bindings)
                {
                    string bindingPath = binding.effectivePath;
                    if (FilterForGamepad(bindingPath) || FilterForKeyboard(bindingPath))
                    {
                        InputMapping newMapping = CreateNewInputMapping(bindingPath);
                        if (SaveInputMappingAsset(newMapping))
                        {
                            InputUiMapping.Add(newMapping);
                        }
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private bool SaveInputMappingAsset(InputMapping inputMapping)
    {
        // Get the path of this ScriptableObject asset
        string assetPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Failed to determine the asset path for InputMappingDeviceType.");
            return false;
        }

        // Get the directory of the asset
        string directory = System.IO.Path.GetDirectoryName(assetPath);

        // Define the subfolder path
        string subfolderPath = System.IO.Path.Combine(directory, DeviceName);

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(subfolderPath))
        {
            AssetDatabase.CreateFolder(directory, DeviceName);
        }

        // Define the full path for the new asset
        string assetFilePath = System.IO.Path.Combine(subfolderPath, inputMapping.name + ".asset");
        // Check if the asset already exists
        if (AssetDatabase.LoadAssetAtPath<InputMapping>(assetFilePath) != null)
        {
            Debug.Log($"Asset '{inputMapping.name}.asset' already exists at {assetFilePath}. Skipping creation.");
            return false;
        }
        // Save the asset
        AssetDatabase.CreateAsset(inputMapping, assetFilePath);
        return true;
    }
#endif

    private bool FilterForKeyboard(string bindingPath)
    {
        if (!bindingPath.Contains("Keyboard") && !bindingPath.Contains("Mouse"))
            return false;
        return _deviceSchema == InputMappingManager.ControlSchema.Keyboard;
    }
    private bool FilterForGamepad(string bindingPath)
    {
        if (!bindingPath.Contains("Gamepad"))
            return false;
        return _deviceSchema != InputMappingManager.ControlSchema.Keyboard;
    }

    private InputMapping CreateNewInputMapping(string devicePath)
    {
        InputMapping newMapping = CreateInstance<InputMapping>();
        var cleanedPath = CleanBindingPath(devicePath);
        newMapping.inputToUiMapping = new InputMapping.InputUiDefinition
        {
            buttonRawPath = cleanedPath
        };
        newMapping.name = cleanedPath;
        return newMapping;
    }
    private string CleanBindingPath(string path)
    {
        string[] parts = path.Split('/');
        return parts.Length > 0 ? parts[^1] : path; // ^1 gets the last element
    }


}
}