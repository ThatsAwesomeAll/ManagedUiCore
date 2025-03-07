using System;
using TMPro;
using UnityEngine;

namespace ManagedUi.Settings
{

[CreateAssetMenu(fileName = "UiManager", menuName = "UiManager/TextSettings")]
public class TextSettings : ScriptableObject
{

    [Header("Font")]
    public FontStyleSettings DefaultFontStyleSettings = new FontStyleSettings(true);
    [SerializeField] private TMP_SpriteAsset _textSprites;
    public TMP_SpriteAsset GetTextSprites() => _textSprites;
    
    [Header("Animation")]
    [SerializeField] private AnimationCurve defaultTextSelectionCurve = new AnimationCurve();
    public AnimationCurve DefaultTextSelectionCurve => defaultTextSelectionCurve;


    public void SetTextAutoFormat(TextMeshProUGUI text, TextStyle style)
    {
        StyleDefaultUtils.ScaleRectTrans(text.rectTransform);
        text.enableAutoSizing = true;
        var fontStyle = style switch
        {

            TextStyle.Header => DefaultFontStyleSettings.HighlightStyle,
            TextStyle.Highlight => DefaultFontStyleSettings.HighlightStyle,
            TextStyle.Text => DefaultFontStyleSettings.TextStyle,
            TextStyle.SubText => DefaultFontStyleSettings.SubTextStyle,
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
        text.fontSizeMin = fontStyle.textMinMax.x;
        text.fontSizeMax = fontStyle.textMinMax.y;
        if (fontStyle.font)
        {
            text.font = fontStyle.font;
        }
        text.alignment = TextAlignmentOptions.Center;
    }


    #region Definitions

    [Serializable]
    public struct FontStyle
    {
        public FontStyle(Vector2 text)
        {
            textMinMax = text;
            font = null;
        }
        public Vector2 textMinMax;
        public TMP_FontAsset font;
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

    #endregion

}
}