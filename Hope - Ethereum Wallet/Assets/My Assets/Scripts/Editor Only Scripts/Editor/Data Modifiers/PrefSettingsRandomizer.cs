using Org.BouncyCastle.Security;
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

    private static readonly Dictionary<FieldInfo, object> FieldsToRandomize = new Dictionary<FieldInfo, object>();
    private static readonly Dictionary<FieldInfo, object> PreviousFieldValues = new Dictionary<FieldInfo, object>();

    /// <summary>
    /// Replaces the pref name for saved tokens in the wallet.
    /// </summary>
    [PostProcessScene(3)]
    public static void ReplaceValues()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        AppSettings = Resources.Load("AppSettings") as AppSettingsInstaller;

        PopulateDictionaries();
        RandomizeFields();
    }

    /// <summary>
    /// Restores the orignial token pref name.
    /// </summary>
    /// <param name="target"> The target playform of the build. </param>
    /// <param name="result"> The result of the build. </param>
    [PostProcessBuild(3)]
    public static void RestoreValues(BuildTarget target, string result)
    {
        FieldsToRandomize.ForEach(pair => pair.Key.SetValue(pair.Value, PreviousFieldValues[pair.Key]));
    }

    /// <summary>
    /// Randomizes the field values.
    /// </summary>
    private static void RandomizeFields()
    {
        SecureRandom secureRandom = new SecureRandom();
        FieldsToRandomize.ForEach(pair => pair.Key.SetValue(pair.Value, SecureRandom.GetNextBytes(secureRandom, 16).GetBase64String()));
    }

    /// <summary>
    /// Populates the dictionaries with valid fields which contain the <see cref="RandomizeTextAttribute"/>.
    /// </summary>
    private static void PopulateDictionaries()
    {
        foreach (var settingsField in typeof(AppSettingsInstaller).GetFields())
        {
            var settingsObj = settingsField.GetValue(AppSettings);
            foreach (var field in settingsObj.GetType().GetFields())
            {
                if (Attribute.IsDefined(field, typeof(RandomizeTextAttribute)))
                {
                    FieldsToRandomize.Add(field, settingsObj);
                    PreviousFieldValues.Add(field, field.GetValue(settingsObj));
                }
            }
        }
    }
}