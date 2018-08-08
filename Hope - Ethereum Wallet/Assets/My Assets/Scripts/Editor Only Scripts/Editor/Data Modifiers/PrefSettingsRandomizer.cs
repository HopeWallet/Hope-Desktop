using Hope.Utils.Misc;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Class which randomizes the name of the prefs that store the token info saved in the wallet.
/// </summary>
public static class PrefSettingsRandomizer
{
    private static AppSettingsInstaller AppSettings;

    private static string OldTokenPrefName;

    private static readonly Dictionary<FieldInfo, object> FieldsToRandomize = new Dictionary<FieldInfo, object>();

    /// <summary>
    /// Replaces the pref name for saved tokens in the wallet.
    /// </summary>
    [PostProcessScene(3)]
    public static void ReplaceValues()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        AppSettings = Resources.Load("AppSettings") as AppSettingsInstaller;

        //OldTokenPrefName = AppSettings.tokenContractSettings.tokenPrefName;
        //AppSettings.tokenContractSettings.tokenPrefName = PasswordUtils.GenerateRandomPassword() + RandomUtils.GenerateRandomHexLetter();
    }

    /// <summary>
    /// Restores the orignial token pref name.
    /// </summary>
    /// <param name="target"> The target playform of the build. </param>
    /// <param name="result"> The result of the build. </param>
    [PostProcessBuild(3)]
    public static void RestoreValues(BuildTarget target, string result) => AppSettings.tokenContractSettings.tokenPrefName = OldTokenPrefName;

    private static void RandomizeFields()
    {
        var type = typeof(AppSettingsInstaller);

        foreach (var settingsField in type.GetFields())
        {
            var settingsObj = settingsField.GetValue(AppSettings);
            foreach (var subField in settingsObj.GetType().GetFields())
            {
                if (Attribute.IsDefined(subField, typeof(RandomizeTextAttribute)))
                    FieldsToRandomize.Add(subField, settingsObj);
            }
        }

        //fieldsToRandomize[0].Item2.SetValue(fieldsToRandomize[0].Item1, "TEST");
    }
}