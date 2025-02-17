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

    public static void DrawCustomHeader()
    {
        GUILayout.Space(2);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    } 
}

#endif
}