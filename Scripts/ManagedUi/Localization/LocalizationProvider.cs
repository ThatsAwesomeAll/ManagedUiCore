using System;
using UnityEngine;
#if USE_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace ManagedUi.Localization
{
public static class LocalizationProvider
{
    public static System.Action OnLocalizationChanged;

    static LocalizationProvider()
    {
#if USE_LOCALIZATION
        LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
#endif
    }

    public static string GetTranslatedValue(string key, string tableName)
    {
#if USE_LOCALIZATION
        if (!Application.isPlaying)
        {
            return key;
        }
        if (!IsLocalizedKeyAvailable(tableName, key)) return "KEY N/A: $" + key + "$";
        
        var myLocalizedString = new LocalizedString(tableName, key);
        return myLocalizedString.GetLocalizedString();
#else
        return key;
#endif
    }

    static bool IsLocalizedKeyAvailable(string tableName, string key)
    {
#if USE_LOCALIZATION

        var table = LocalizationSettings.StringDatabase?.GetTable(tableName);
        return table != null && table.GetEntry(key) != null;
#else
        return false;
#endif
    }
    
#if USE_LOCALIZATION
    private static void LocaleChanged(Locale locale)
    {
        OnLocalizationChanged?.Invoke();
    }
#endif



}
}