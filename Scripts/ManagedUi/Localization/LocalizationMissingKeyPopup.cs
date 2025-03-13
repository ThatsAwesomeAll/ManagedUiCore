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
        LocalizationMissingKeyPopup window = CreateInstance<LocalizationMissingKeyPopup>();
        window.titleContent = new GUIContent("Translate key: " + key); // Optional: Give each instance a title
        window.minSize = new Vector2(500, 400);
        window.onConfirmCallback = onConfirm;
        window.languages = (string[])languages.Clone(); // Clone to avoid modifying the original array
        window.key = key;
        window.tablename = tablename;
        window.ShowPopup();
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
        bool confirmed = false;
        if (GUILayout.Button("Confirm"))
        {
            confirmed = true;
            close = true;
        }
        if (GUILayout.Button("Cancel"))
        {
            confirmed = false;
            close = true;
        }
        GUILayout.EndHorizontal();
        if (close)
        {
            Close();
             if (confirmed) onConfirmCallback?.Invoke(newEntries.ToArray()); // Ensure the updated values are returned
        }
    }
}
}
#endif