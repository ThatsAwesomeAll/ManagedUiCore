using System;
using TMPro;
using UnityEngine;

namespace ManagedUi
{

[CreateAssetMenu(fileName = "UiManager", menuName = "UiManager/UiSettings")]
public class UiSettings : ScriptableObject
{

    [SerializeField] ColorTheme ImageColors = new ColorTheme(ColorMode.Default);
    [SerializeField] ColorTheme TextColors = new ColorTheme(ColorMode.DefaultText);
    [Header("Default Image settings")]
    [SerializeField] Sprite _defaultImage;

    [SerializeField] Sprite _defaultSelectionImage;
    [SerializeField] private float _defaultSelectionImageSliceFactor = 0.3f;

    [SerializeField] Sprite _defaultBackgroundImage;
    [SerializeField] private float _defaultBackgroundImageSliceFactor = 0.3f;

    public float DefaultBackgroundImageSliceFactor => _defaultBackgroundImageSliceFactor;
    public float DefaultSelectionImageSliceFactor => _defaultSelectionImageSliceFactor;
    public Sprite DefaultImage() => _defaultImage;
    public Sprite DefaultBackgroundImage() => _defaultBackgroundImage;
    public Sprite DefaultSelectionImage() => _defaultSelectionImage;
        
    [SerializeField] private FontStyleSettings DefaultFontStyleSettings;

    [Header("Selection Animation")]
    [SerializeField] private float defaultSelectionDuration = 0.3f;
    [SerializeField] private float defaultConfirmedDuration = 0.1f;
    [SerializeField] private float defaultSelectionStrength = 5f;
    [SerializeField] private AnimationCurve defaultTextSelectionCurve = new AnimationCurve();

    public float DefaultSelectionDuration => defaultSelectionDuration;
    public float DefaultConfirmedDuration => defaultConfirmedDuration;
    public float DefaultSelectionStrength => defaultSelectionStrength;
    public AnimationCurve DefaultTextSelectionCurve => defaultTextSelectionCurve;
    
    public FontStyleSettings FontStyles => DefaultFontStyleSettings;
    public Color SelectedColor => ImageColors.ColorAccent;
    public Color ConfirmedColor => ImageColors.ColorAccentLighter;


    [ContextMenu("Set Default Colors")]
    public void SetDefaultColor()
    {
        ImageColors = new ColorTheme(ColorMode.Default);
        TextColors = new ColorTheme(ColorMode.DefaultText);
    }

    public static Color GetImageColorByEnum(ColorName colorTheme, ColorTheme colorPalette)
    {
        return colorTheme switch
        {
            ColorName.Accent => colorPalette.ColorAccent,
            ColorName.AccentLighter => colorPalette.ColorAccentLighter,
            ColorName.Main => colorPalette.ColorMain,
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

    private static void ScaleRectTrans(RectTransform transform)
    {
        transform.anchorMin = new Vector2(0, 0);
        transform.anchorMax = new Vector2(1, 1);
        transform.offsetMin = Vector2.zero;
        transform.offsetMax = Vector2.zero;
        transform.anchoredPosition = new Vector2(0, 0);
        transform.pivot = new Vector2(0.5f, 0.5f);
    }


    public void SetTextAutoFormat(TextMeshProUGUI text, TextStyle style, ColorName background)
    {
        text.color = GetImageColorByEnum(background, TextColors);
        ScaleRectTrans(text.rectTransform);
        text.enableAutoSizing = true;
        var textSize = style switch
        {

            TextStyle.Header => FontStyles.HeaderStyle.textMinMax,
            TextStyle.Highlight => FontStyles.HighlightStyle.textMinMax,
            TextStyle.Text => FontStyles.TextStyle.textMinMax,
            TextStyle.SubText => FontStyles.SubTextStyle.textMinMax,
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
        text.fontSizeMin = textSize.x;
        text.fontSizeMax = textSize.y;
        text.alignment = TextAlignmentOptions.Center;
    }

    public void SetTextColor(TextMeshProUGUI text, ColorName background)
    {
        text.color = GetImageColorByEnum(background, TextColors);
    }
    
    #region SettingDefinition
    [Serializable]
    public struct FontStyle
    {
        public FontStyle(Vector2 text)
        {
            textMinMax = text;
        }
        public Vector2 textMinMax;
    }

    public enum TextStyle
    {
        Header,
        Highlight,
        Text,
        SubText
    }

    [Serializable]
    public struct FontStyleSettings
    {
        public FontStyle HeaderStyle;
        public FontStyle HighlightStyle;
        public FontStyle TextStyle;
        public FontStyle SubTextStyle;
        public FontStyleSettings(bool dummy)
        {
            HeaderStyle = new FontStyle(new Vector2(30, 100));
            HighlightStyle = new FontStyle(new Vector2(30, 80));
            TextStyle = new FontStyle(new Vector2(20, 40));
            SubTextStyle = new FontStyle(new Vector2(10, 30));
        }
    }

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
        Main,
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
        public Color ColorMain;
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
                ColorMain = new Color(0.1f, 0.1f, 0.1f);
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
            ColorMain = new Color(0.98f, 0.51f, 0.0f);
            ColorLight = new Color(0.15f, 0.32f, 0.48f);
            ColorLighter = new Color(0.2f, 0.4f, 0.6f);
            ColorDark = new Color(0.01f, 0.19f, 0.27f);
            ColorDarker = new Color(0.011f, 0.1f, 0.25f);
            ColorBackground = new Color(0.2f, 0.3f, 0.3f);
            ColorBackgroundDarker = new Color(0.1f, 0.1f, 0.1f);
        }
    }

    #endregion


    public static UiSettings GetSettings()
    {
        UiSettings matchingAssets = Resources.Load<UiSettings>("ManagedUi/DefaultUiSettings");
        return matchingAssets;
    }
}
}