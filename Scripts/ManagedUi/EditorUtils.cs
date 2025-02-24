using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace ManagedUi
{
public class EditorUtils
{

    public static void DrawProperty(SerializedProperty property, string content, string tooltip)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUILayout.LabelField(new GUIContent(content, tooltip), GUILayout.Width(120));
        if (property != null)
        {
            EditorGUILayout.PropertyField(property, new GUIContent("", tooltip));
        }
        GUILayout.EndHorizontal();
    }
    public static void DrawPropertyList(List<SerializedProperty> property, string content, string tooltip, float width = 120)
    {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUILayout.LabelField(new GUIContent(content, tooltip), GUILayout.Width(width));

        foreach (var prop in property.Where(prop => property != null))
        {
            EditorGUILayout.PropertyField(prop, new GUIContent("", tooltip));
        }
        GUILayout.EndHorizontal();
    }

    public static void DrawCustomHeader()
    {
        GUILayout.Space(2);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}

#endif
}