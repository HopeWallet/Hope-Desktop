using System;
using UnityEditor;
using UnityEditor.Callbacks;
using Random = UnityEngine.Random;
using UnityEngine;

/// <summary>
/// Class which randomizes the name of the prefs that store the token info saved in the wallet.
/// </summary>
public class TokenPrefRandomizer
{

    private static AppSettingsInstaller AppSettings;

    private static string OldTokenPrefName;

    /// <summary>
    /// Replaces the pref name for saved tokens in the wallet.
    /// </summary>
    [PostProcessScene(2)]
    public static void ReplaceValues()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        AppSettings = Resources.Load("AppSettings") as AppSettingsInstaller;
        OldTokenPrefName = AppSettings.tokenContractSettings.tokenPrefName;
        AppSettings.tokenContractSettings.tokenPrefName = PasswordUtils.GenerateRandomPassword() + RandomUtils.GenerateRandomHexLetter();
    }

    /// <summary>
    /// Restores the orignial token pref name.
    /// </summary>
    /// <param name="target"> The target playform of the build. </param>
    /// <param name="result"> The result of the build. </param>
    [PostProcessBuild(2)]
    public static void RestoreValues(BuildTarget target, string result) => AppSettings.tokenContractSettings.tokenPrefName = OldTokenPrefName;

}
