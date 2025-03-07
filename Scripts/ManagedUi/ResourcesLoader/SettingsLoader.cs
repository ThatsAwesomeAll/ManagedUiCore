using ManagedUi.Tooltip;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.ResourcesLoader
{
public static class SettingsLoader
{
    public static UiSettings GetSettings()
    {
        UiSettings matchingAssets = Resources.Load<UiSettings>("ManagedUi/DefaultUiSettings");
        if (matchingAssets == null)
        {
            matchingAssets = ScriptableObject.CreateInstance<UiSettings>();
            matchingAssets.name = "DefaultUiSettings";
            SaveScriptableObject(matchingAssets, "Assets/Resources/ManagedUi/DefaultUiSettings.asset");
        }
        return matchingAssets;
    }
    
    public static TooltipEvent GetTooltipEvent()
    {
        TooltipEvent matchingAssets = Resources.Load<TooltipEvent>("ManagedUi/TooltipEvent");
        if (matchingAssets == null)
        {
            matchingAssets = ScriptableObject.CreateInstance<TooltipEvent>();
            matchingAssets.name = "DefaultUiSettings";
            SaveScriptableObject(matchingAssets, "Assets/Resources/ManagedUi/TooltipEvent.asset");
        }
        return matchingAssets;
    }

    private static void SaveScriptableObject(ScriptableObject obj, string path)
    {
#if UNITY_EDITOR
        // Ensure the directory exists
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!EditorUtility.DisplayDialog(
                "Default Settings for UI are created", // Title
                "Do you want to proceed?",             // Message
                "Yes",                                 // Yes button
                "No"                                   // No button
            ))
        {
            return;
        }
        // Save the asset
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Saved ScriptableObject at: {path}");
#else
        Debug.LogError("No UISettings found! UI will be missing default values");
#endif
    } 
}
}