using ManagedUi.GridSystem;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{
    
    [ExecuteInEditMode]
    public class ManagedImage : Image, IGridElement
    {
        [Header("Style")]
        public bool onHoverEffect = true;
        public bool fixColor = false;
        public UiSettings.ColorName colorTheme;
        public Color imageColor;
        public Vector2Int growth = Vector2Int.one;
        
        [SerializeField] private UiSettings _manager;

        protected override void Awake()
        {
            SetUp();
        }

        public void SetUp()
        {
            if (!_manager) _manager = UiSettings.GetSettings();
        }
        
        #if UNITY_EDITOR
        private void Update()
        {
            if (!_manager)
            {
                return;
            }
            if (Application.isPlaying)
            {
                return;
            }
            if (fixColor)
            {
                SetColorByTheme(colorTheme);
            }
        }
        #endif

        public void SetColorByTheme(UiSettings.ColorName currentEnumValue)
        {
            if (!_manager) return;
            var colorTemp = _manager.GetImageColorByEnum(currentEnumValue);
            color = colorTemp;
        }

        public void SetColorByFixed(Color colorTypeColorValue)
        {
            color = colorTypeColorValue;
        }

        public void SetAsDefaultBackground()
        {
            fixColor = true;
            sprite = _manager.DefaultBackgroundImage();
            type = UnityEngine.UI.Image.Type.Sliced;
            pixelsPerUnitMultiplier = _manager.DefaultBackgroundImageSliceFactor;
        }
        
        public int VerticalLayoutGrowth() => growth.y > 0 ? growth.y : 1;
        public int HorizontalLayoutGrowth() => growth.x > 0 ? growth.x : 1;
        public bool IgnoreLayout() => false;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ManagedImage))]
    public class ManagedImageEditor : Editor
    {
        private ManagedImage image;

        private void OnEnable()
        {
            image = (ManagedImage)target;
            image.SetUp();
        }


        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("_manager");
            var colorType = serializedObject.FindProperty("imageColor");
            var fixColor = serializedObject.FindProperty("fixColor");
            var colorTheme = serializedObject.FindProperty("colorTheme");
            var growth = serializedObject.FindProperty("growth");

            EditorUtils.DrawProperty(fixColor, "Color fixed", "Fix your color by Theme");
            if (fixColor.boolValue)
            {
                EditorUtils.DrawProperty(colorTheme, "Color", "Select Color");
                int enumIndex = colorTheme.enumValueIndex;
                UiSettings.ColorName currentEnumValue = (UiSettings.ColorName)enumIndex;
                image.SetColorByTheme(currentEnumValue);
            }
            else
            {
                Color temp = colorType.colorValue;
                EditorUtils.DrawProperty(colorType, "Color", "Select Color");
                if (temp != colorType.colorValue)
                {
                    image.SetColorByFixed(colorType.colorValue);
                }
            }
            EditorUtils.DrawProperty(growth, "Layout Growth", "Selecte Grow factor for layout group");

            if (UIManagerAsset != null)
            {
                EditorUtils.DrawProperty(UIManagerAsset, "Manager Asset", "Dont change this");
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("NO MANAGER FOUND"), GUILayout.Width(120));
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtils.DrawCustomHeader();
            base.OnInspectorGUI();
        }
    }
#endif
}