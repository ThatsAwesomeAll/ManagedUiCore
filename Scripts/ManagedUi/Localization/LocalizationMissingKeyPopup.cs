#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ManagedUi.Localization
{
public class LocalizationMissingKeyPopup : EditorWindow
{
    private string key = "";
    private string tablename = "";
    private Action<string[]> onConfirmCallback;
    private string[] languages;
    List<string> newEntries = new List<string>();

    public static void ShowPopup(string key, string tablename, string[] languages, Action<string[]> onConfirm)
    {
        LocalizationMissingKeyPopup window = GetWindow<LocalizationMissingKeyPopup>("Translate key: "+key);
        window.minSize = new Vector2(500, 400);
        window.onConfirmCallback = onConfirm;
        window.languages = (string[])languages.Clone(); // Clone to avoid modifying the original array
        window.key = key;
        window.tablename = tablename;
        foreach (string lang in languages)
        {
            window.newEntries.Add(key);
        } 
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Translation for Missing [Table: "+ tablename + "]Key: " + key, EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (languages != null && languages.Length > 0)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                if (newEntries.Count <= i)
                {
                    newEntries.Add(key);
                }
                newEntries[i] = EditorGUILayout.TextField(languages[i],  newEntries[i]);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No languages provided.");
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        bool close = false;
        if (GUILayout.Button("Confirm"))
        {
            close = true;
            onConfirmCallback?.Invoke(newEntries.ToArray()); // Ensure the updated values are returned
        }
        if (GUILayout.Button("Cancel"))
        {
            close = true;
        }
        GUILayout.EndHorizontal();
        if (close)
        {
            Close();
        }
    }
}
}
#endif