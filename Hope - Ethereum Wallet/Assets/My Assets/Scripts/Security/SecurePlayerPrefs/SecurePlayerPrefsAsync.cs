using Hope.Security.Encryption;
using Hope.Security.SecurePlayerPrefs.Base;
using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class used for saving data to SecurePlayerPrefs asynchronously.
/// Useful in the case where a lot of data needs to be saved.
/// </summary>
public class SecurePlayerPrefsAsync : SecurePlayerPrefsBase
{
    
    /// <summary>
    /// Initializes the SecurePlayerPrefsAsync by ensuring we have the base seed created.
    /// </summary>
    static SecurePlayerPrefsAsync()
    {
        EnsureSeedCreation();
    }

    /// <summary>
    /// Gets a string from the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the value located in the PlayerPrefs. </param>
    /// <param name="onStringReceived"> Action to call when the string has been received. </param>
    public static void GetString(string key, Action<string> onStringReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, onStringReceived));

    /// <summary>
    /// Gets an int from the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the int value located in the PlayerPrefs. </param>
    /// <param name="onIntReceived"> Action to call when the int value has been received. </param>
    public static void GetInt(string key, Action<int> onIntReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, str => onIntReceived?.Invoke(int.Parse(str))));

    /// <summary>
    /// Gets a float from the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the float value located in the PlayerPrefs. </param>
    /// <param name="onFloatReceived"> Action to call when the float value has been received. </param>
    public static void GetFloat(string key, Action<float> onFloatReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, str => onFloatReceived?.Invoke(float.Parse(str))));

    /// <summary>
    /// Sets a string value to the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onStringSet"> Optional action to call when the string value has been set. </param>
    public static void SetString(string key, string value, Action onStringSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value, onStringSet));

    /// <summary>
    /// Sets an int value to the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onIntSet"> Optional action to call when the int value has been set. </param>
    public static void SetInt(string key, int value, Action onIntSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onIntSet));

    /// <summary>
    /// Sets a float value to the SecurePlayerPrefs asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onFloatSet"> Optional action to call when the float value has been set. </param>
    public static void SetFloat(string key, float value, Action onFloatSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onFloatSet));

    private static async Task InternalSetString(string key, string value, Action onValueSet)
    {
        string secureKey = await GetSecureKey(key);
        string secureEntropy = await Task.Run(() => GetValueHash(secureKey));
        string encryptedValue = await Task.Run(() => value.DPEncrypt(secureEntropy).DPEncrypt());

        PlayerPrefs.SetString(secureKey, encryptedValue);
        onValueSet?.Invoke();
    }

    private static async Task InternalGetString(string key, Action<string> onStringReceived)
    {
        string secureKey = await GetSecureKey(key);
        string secureEntropy = await Task.Run(() => GetValueHash(secureKey));
        string encryptedValue = PlayerPrefs.GetString(secureKey);

        onStringReceived?.Invoke(await Task.Run(() => encryptedValue.DPDecrypt().DPDecrypt(secureEntropy)));
    }

    private static async Task<string> GetSecureKey(string key)
    {
        string baseKeyEncrypted = GetSeedValue();

        return await Task.Run(() => GetKeyHash(string.Concat(baseKeyEncrypted.DPDecrypt(), key)));
    }
}