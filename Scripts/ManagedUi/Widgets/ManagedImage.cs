using ManagedUi.GridSystem;
using ManagedUi.Selectables;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi.Widgets
{

[ExecuteInEditMode]
public class ManagedImage : Image, IManagedGridLayoutElement, ISelectableAnimator
{


    public bool animationEnabled = false;
    public bool disableOnAnimationEnd = false;

    public ManagedColor basicColor = new ManagedColor(false);
    public ManagedColor selectColor = new ManagedColor(false);
    public ManagedColor confirmColor = new ManagedColor(false);

    public Vector2Int growth = Vector2Int.one;
    public bool ignoreLayout = false;

    private Color _animationSavedColor;

    public UiSettings.ColorName ColorTheme { get => basicColor.Theme; set => base.color = basicColor.SetColorByTheme(value, _manager); }

    public bool FixColor
    {
        get => basicColor.IsFixedColor();
        set
        {
            basicColor.SetFixedColor(value);
            UpdateColor();
        }
    }

    public void UpdateColor()
    {
        base.color = basicColor.GetColor(_manager);
    }

    public void SetAsDefaultBackground()
    {
        basicColor.SetFixedColor(true);
        sprite = _manager.DefaultBackgroundImage();
        type = Type.Sliced;
        pixelsPerUnitMultiplier = _manager.DefaultBackgroundImageSliceFactor;
    }

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation)
    {
        _animationSavedColor = color;
        if (!animationEnabled) return;
        bool tempEnabled = (mode != ISelectableAnimator.Mode.Default) || !disableOnAnimationEnd;
        enabled = tempEnabled;
        gameObject.SetActive(tempEnabled);
    }

    public void LerpTo(ISelectableAnimator.Mode mode, float currentValue)
    {
        if (!animationEnabled) return;
        switch (mode)
        {
            case ISelectableAnimator.Mode.Default:
                LerpColor(basicColor.GetColor(_manager), currentValue);
                break;
            case ISelectableAnimator.Mode.Selected:
                if (selectColor?.UseInAnimation() != null && selectColor.UseInAnimation())
                {
                    LerpColor(selectColor.GetColor(_manager), currentValue);
                }
                else
                {
                    LerpColor(_manager.SelectedColor, currentValue);
                }
                break;
            case ISelectableAnimator.Mode.Confirmed:
                if (confirmColor?.UseInAnimation() != null && confirmColor.UseInAnimation())
                {
                    LerpColor(confirmColor.GetColor(_manager), currentValue);
                }
                else
                {
                    LerpColor(_manager.ConfirmedColor, currentValue);
                }
                break;
        }
    }
    private void LerpColor(Color customColor, float currentValue)
    {
        color = Color.Lerp(_animationSavedColor, customColor, currentValue);
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
        UpdateColor();
    }
#endif

    protected override void Awake()
    {
        SetUp();
    }

    [SerializeField] private UiSettings _manager;
    public void SetUp()
    {
        if (!_manager) _manager = UiSettings.GetSettings();
    }

    public int VerticalLayoutGrowth() => growth.y > 0 ? growth.y : 1;
    public int HorizontalLayoutGrowth() => growth.x > 0 ? growth.x : 1;
    public bool IgnoreLayout() => ignoreLayout;


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
        var animationEnabled = serializedObject.FindProperty("animationEnabled");
        var disableOnAnimationEnd = serializedObject.FindProperty("disableOnAnimationEnd");

        var basicColor = serializedObject.FindProperty("basicColor");
        var selectColor = serializedObject.FindProperty("selectColor");
        var confirmColor = serializedObject.FindProperty("confirmColor");

        var growth = serializedObject.FindProperty("growth");
        var ignoreLayout = serializedObject.FindProperty("ignoreLayout");

        var sprite = serializedObject.FindProperty("m_Sprite");
        EditorGUI.BeginChangeCheck();
        EditorUtils.DrawProperty(basicColor, "Color", "enable automatic animation");
        bool updateRequired = EditorGUI.EndChangeCheck();

        EditorUtils.DrawProperty(growth, "Layout Growth", "Selecte Grow factor for layout group");
        EditorUtils.DrawProperty(ignoreLayout, "Layout Growth ignored", "Ignore layout group growth");
        var animationProps = new List<SerializedProperty>();
        animationProps.Add(animationEnabled);
        animationProps.Add(disableOnAnimationEnd);
        EditorUtils.DrawPropertyList(animationProps, "Animation | Visible Animation Only", "enable automatic animation", 80);
        EditorUtils.DrawProperty(selectColor, "Custom SelectColor", "enable automatic animation");
        EditorUtils.DrawProperty(confirmColor, "Custom SelectColor", "enable automatic animation");

        EditorUtils.DrawProperty(sprite, "Sprite", "Set Sprite");

        if (UIManagerAsset != null)
        {
            EditorUtils.DrawProperty(UIManagerAsset, "Manager Asset", "Dont change this");
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent("NO MANAGER FOUND"), GUILayout.Width(120));
        }

        serializedObject.ApplyModifiedProperties();
        if (updateRequired)
        {
            image.UpdateColor();
        }
        EditorUtils.DrawCustomHeader();
        base.OnInspectorGUI();
    }
}
#endif
}