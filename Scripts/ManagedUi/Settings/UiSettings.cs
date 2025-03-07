using ManagedUi.ResourcesLoader;
using System;
using TMPro;
using UnityEngine;
using TextSettings = ManagedUi.Settings.TextSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ManagedUi
{

[CreateAssetMenu(fileName = "UiManager", menuName = "UiManager/UiSettings")]
public class UiSettings : ScriptableObject
{

    public Action OnSettingsChanged;

    [SerializeField] ColorTheme ImageColors = new ColorTheme(ColorMode.Default);
    [SerializeField] ColorTheme TextColors = new ColorTheme(ColorMode.DefaultText);

    [Header("Default Image settings")]
    [SerializeField] private ImageSettings _imageSettings;

    public float DefaultBackgroundImageSliceFactor => _imageSettings.DefaultBackgroundImageSliceFactor;
    public float DefaultSelectionImageSliceFactor => _imageSettings.DefaultSelectionImageSliceFactor;
    public Sprite DefaultImage() => _imageSettings?.DefaultImage();
    public Sprite DefaultBackgroundImage() => _imageSettings?.DefaultBackgroundImage();
    public Sprite DefaultSelectionImage() => _imageSettings?.DefaultSelectionImage();
    public Sprite DefaultShadowImage() => _imageSettings?.DefaultShadowImage();
    public Sprite DefaultBoarderImage()  => _imageSettings?.DefaultBoarderImage();
    public float DefaultBoarderImageSliceFactor => _imageSettings.DefaultBoarderImageSliceFactor;

    [Header("Text Settings")]
    [SerializeField] private TextSettings _textSettings;
    public TextSettings.FontStyleSettings FontStyles => _textSettings.DefaultFontStyleSettings;
    public void SetTextAutoFormat(TextMeshProUGUI text, TextSettings.TextStyle style)
    {
        _textSettings.SetTextAutoFormat(text, style);
    }
    public TMP_SpriteAsset SpriteAsset => _textSettings?.GetTextSprites();
    
    
    [Header("Selection Animation")]
    [SerializeField] private float defaultSelectionDuration = 0.3f;
    [SerializeField] private float defaultConfirmedDuration = 0.1f;
    [SerializeField] private float defaultSelectionStrength = 5f;

    public float DefaultSelectionDuration => defaultSelectionDuration;
    public float DefaultConfirmedDuration => defaultConfirmedDuration;
    public float DefaultSelectionStrength => defaultSelectionStrength;

    public Color SelectedColor => ImageColors.ColorAccent;
    public Color ConfirmedColor => ImageColors.ColorAccentLighter;


    [ContextMenu("Set Default Colors")]
    public void SetDefaultColor()
    {
        ImageColors = new ColorTheme(ColorMode.Default);
        TextColors = new ColorTheme(ColorMode.DefaultText);
    }

    private static Color GetImageColorByEnum(ColorName colorTheme, ColorTheme colorPalette)
    {
        return colorTheme switch
        {
            ColorName.Accent => colorPalette.ColorAccent,
            ColorName.AccentLighter => colorPalette.ColorAccentLighter,
            ColorName.Light => colorPalette.ColorLight,
            ColorName.Lighter => colorPalette.ColorLighter,
            ColorName.Dark => colorPalette.ColorDark,
            ColorName.Darker => colorPalette.ColorDarker,
            ColorName.Background => colorPalette.ColorBackground,
            ColorName.BackgroundDarker => colorPalette.ColorBackgroundDarker,
            _ => Color.black
        };
    }
    public Color GetImageColorByEnum(ColorName colorTheme)
    {
        return GetImageColorByEnum(colorTheme, ImageColors);
    }

    public Color GetTextColorByEnum(ColorName colorTheme)
    {
        return GetImageColorByEnum(colorTheme, TextColors);
    }


    #region SettingDefinition

    [Serializable]
    public enum ColorMode
    {
        Default,
        DefaultText,
    }

    public enum ColorName
    {
        Accent,
        AccentLighter,
        Light,
        Lighter,
        Dark,
        Darker,
        Background,
        BackgroundDarker
    }

    [Serializable]
    public struct ColorTheme
    {
        public Color ColorAccent;
        public Color ColorAccentLighter;
        public Color ColorLight;
        public Color ColorLighter;
        public Color ColorDark;
        public Color ColorDarker;
        public Color ColorBackground;
        public Color ColorBackgroundDarker;

        public ColorTheme(ColorMode mode)
        {
            if (mode == ColorMode.DefaultText)
            {
                ColorAccent = new Color(0.1f, 0.1f, 0.1f);
                ColorAccentLighter = new Color(0.1f, 0.1f, 0.1f);
                ColorLight = new Color(0.8f, 0.8f, 0.8f);
                ColorLighter = new Color(0.8f, 0.8f, 0.8f);
                ColorDark = new Color(0.8f, 0.8f, 0.8f);
                ColorDarker = new Color(0.8f, 0.8f, 0.8f);
                ColorBackground = new Color(0.8f, 0.8f, 0.8f);
                ColorBackgroundDarker = new Color(0.8f, 0.8f, 0.8f);
                return;
            }
            ColorAccent = new Color(1.0f, 0.71f, 0.01f);
            ColorAccentLighter = new Color(1.0f, 0.91f, 0.01f);
            ColorLight = new Color(0.15f, 0.32f, 0.48f);
            ColorLighter = new Color(0.2f, 0.4f, 0.6f);
            ColorDark = new Color(0.01f, 0.19f, 0.27f);
            ColorDarker = new Color(0.011f, 0.1f, 0.25f);
            ColorBackground = new Color(0.2f, 0.3f, 0.3f);
            ColorBackgroundDarker = new Color(0.1f, 0.1f, 0.1f);
        }
    }

    #endregion

    public static void ConnectSettings(ref UiSettings settings)
    {
        if (settings != null)
        {
            return;
        }
        settings = SettingsLoader.GetSettings();
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(UiSettings))]
public class UiSettingsEditor : Editor
{
    private UiSettings settings;

    private void OnEnable()
    {
        settings = (UiSettings)target;
    }


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        bool updateRequired = EditorGUI.EndChangeCheck();
        if (updateRequired)
        {
            settings.OnSettingsChanged?.Invoke();
        }
    }
}
#endif
}