using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if USE_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
#endif
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
        if (!IsLocalizedKeyAvailable(tableName, key))
        {
#if UNITY_EDITOR
            List<string> languages = new List<string>();
            foreach (var localization in LocalizationSettings.AvailableLocales.Locales)
            {
                languages.Add(localization.Identifier.ToString());
            }
            // LocalizationMissingKeyPopup.ShowPopup(key, tableName, languages.ToArray(),(newentries)=> OnConfirm(newentries, tableName, key, languages));
#endif
            return "KEY N/A: " + key;
        }

        var myLocalizedString = new LocalizedString(tableName, key);
        return myLocalizedString.GetLocalizedString();
#else
        return key;
#endif
    }

#if USE_LOCALIZATION && UNITY_EDITOR
    private static void OnConfirm(string[] newEntries, string tableName, string key, List<string> languages)
    {
        int iEntry = 0;
        foreach (var entry in newEntries)
        {
            AddEntryToLocalizationTable(tableName, key, languages[iEntry], entry);
            iEntry++;
        }
    }
    public static void AddEntryToLocalizationTable(string tableName, string key, string language, string value)
    {
        // return;
        var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);
        if (collection == null)
        {
            Debug.LogError($"String Table '{tableName}' not found.");
            return;
        }

        // Add entry if it doesn't exist
        if (collection.SharedData.GetEntry(key) == null)
        {
            collection.SharedData.AddKey(key);
        }
        // Find the specific language table
        StringTable targetTable = collection.StringTables.FirstOrDefault(table =>
        {
            Debug.Log(table.LocaleIdentifier);
            return table.LocaleIdentifier.ToString().Contains(language);
        });
        
        if (targetTable == null)
        {
            Debug.LogError($"Locale '{language}' not found in table '{tableName}'.");
            return;
        }

        // Add or update entry
        targetTable.AddEntry(key, value);
        EditorUtility.SetDirty(targetTable); // Mark table as changed
    }
#endif

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