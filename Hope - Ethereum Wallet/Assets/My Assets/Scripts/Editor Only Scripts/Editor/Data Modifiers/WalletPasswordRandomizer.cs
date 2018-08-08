using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Class which randomizes the names of the password derivation strings and the order of the operations.
/// </summary>
public static class WalletPasswordRandomizer
{

    private static PlayerPrefPassword PasswordObj;

    private static int[] SavedOps;
    private static string[] SavedKeys;

    /// <summary>
    /// Replaces the values of the arrays.
    /// </summary>
    [PostProcessScene(2)]
    public static void ReplaceValues()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        ReplacePrefNames();
        ReplaceOpValues();
        SaveChanges();
    }

    /// <summary>
    /// Restores the original values to the arrays.
    /// </summary>
    /// <param name="target"> The target playform of the build. </param>
    /// <param name="result"> The result of the build. </param>
    [PostProcessBuild(2)]
    public static void RestoreValues(BuildTarget target, string result)
    {
        Array.Copy(SavedOps, PasswordObj.ops, SavedOps.Length);
        Array.Copy(SavedKeys, PasswordObj.keys, SavedKeys.Length);

        SaveChanges();
    }

    /// <summary>
    /// Replaces the operation values by randomizing the array.
    /// </summary>
    private static void ReplaceOpValues()
    {
        var ops = PasswordObj.ops;

        SavedOps = new int[ops.Length];
        Array.Copy(ops, SavedOps, ops.Length);

        System.Random random = new System.Random();
        PasswordObj.ops = ops.OrderBy(x => random.Next()).ToArray();
    }

    /// <summary>
    /// Replaces the PlayerPref names by changing the name of each key to a random password.
    /// </summary>
    private static void ReplacePrefNames()
    {
        PasswordObj = GetWalletPasswordObj();
        var keys = PasswordObj.keys;

        SavedKeys = new string[keys.Length];
        Array.Copy(keys, SavedKeys, keys.Length);

        for (int i = 0; i < keys.Length; i++)
            keys[i] = PasswordUtils.GenerateRandomPassword();
    }

    /// <summary>
    /// Saves all changes to the assets so the new values are applied to the ScriptableObject.
    /// </summary>
    private static void SaveChanges()
    {
        EditorUtility.SetDirty(PasswordObj);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Loads the WalletPassword object from the Resources folder.
    /// </summary>
    /// <returns> The SafePassword object we will change the values for. </returns>
    private static PlayerPrefPassword GetWalletPasswordObj() => Resources.Load("PasswordBase") as PlayerPrefPassword;

}