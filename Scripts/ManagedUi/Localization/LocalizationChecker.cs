#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace ManagedUi.Localization
{
[InitializeOnLoad]
public class LocalizationDefineChecker
{
    static LocalizationDefineChecker()
    {
        const string defineSymbol = "USE_LOCALIZATION";

        bool localizationExists = AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.GetTypes().Any(type => type.Namespace == "UnityEngine.Localization"));

        string defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);
        if (localizationExists && !defines.Contains(defineSymbol))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defines + ";" + defineSymbol);
        }
        else if (!localizationExists && defines.Contains(defineSymbol))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, defines.Replace(defineSymbol, ""));
        }
    }
}
}
#endif