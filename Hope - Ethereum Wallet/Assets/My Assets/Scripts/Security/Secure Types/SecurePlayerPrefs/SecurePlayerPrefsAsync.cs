using Hope.Security.Encryption.DPAPI;
using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class used for saving data to SecurePlayerPrefs asynchronously.
/// Useful in the case where a lot of data needs to be saved.
/// </summary>
public sealed class SecurePlayerPrefsAsync : SecurePlayerPrefsBase
{
    /// <summary>
    /// Initializes the SecurePlayerPrefsAsync.
    /// </summary>
    /// <param name="settings"> The Settings to use with the SecurePlayerPrefsAsync. </param>
    public SecurePlayerPrefsAsync(Settings settings) : base(settings)
    {
    }

    /// <summary>
    /// Sets a string value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onStringSet"> Optional action to call when the string value has been set. </param>
    public static void SetString(string key, string value, Action onStringSet = null) => StartAsyncSetPref(key, value, onStringSet);

    /// <summary>
    /// Sets an int value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onIntSet"> Optional action to call when the int value has been set. </param>
    public static void SetInt(string key, int value, Action onIntSet = null) => StartAsyncSetPref(key, value.ToString(), onIntSet);

    /// <summary>
    /// Sets a float value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onFloatSet"> Optional action to call when the float value has been set. </param>
    public static void SetFloat(string key, float value, Action onFloatSet = null) => StartAsyncSetPref(key, value.ToString(), onFloatSet);

    /// <summary>
    /// Starts the AsyncSetPref by getting the base encrypted key and starting the InternalSetString.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onValueSet"> Action to call once the value has been set. </param>
    private static void StartAsyncSetPref(string key, string value, Action onValueSet)
    {
        string baseKeyEncrypted = GetSeedValue();
        AsyncTaskScheduler.Schedule(() => InternalSetString(baseKeyEncrypted, key, value, onValueSet));
    }

    /// <summary>
    /// Sets a string value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// Gets the secure key, secure entropy, and encrypted value all asynchronously.
    /// </summary>
    /// <param name="baseKeyEncrypted"> The encrypted base key to use to get the secure key of this SecurePlayerPref. </param>
    /// <param name="key"> The key to use to set the PlayerPref. </param>
    /// <param name="value"> The value to use for the PlayerPref. </param>
    /// <param name="onValueSet"> Action called once the PlayerPref has been set successfully. </param>
    /// <returns> The task for creating the hashed key/values and setting the PlayerPref. </returns>
    private static async Task InternalSetString(string baseKeyEncrypted, string key, string value, Action onValueSet)
    {
        string secureKey = await GetSecureKey(baseKeyEncrypted, key).ConfigureAwait(false);
        string secureEntropy = await Task.Run(() => GetValueHash(secureKey)).ConfigureAwait(false);
        string encryptedValue = await Task.Run(() => value.DPEncrypt(secureEntropy).Protect()).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() =>
        {
            PlayerPrefs.SetString(secureKey, encryptedValue);
            onValueSet?.Invoke();
        });
    }

    /// <summary>
    /// Gets the encrypted and hashed key asynchronously.
    /// </summary>
    /// <param name="key"> The unencrypted key to use to derive the encrypted and hashed key. </param>
    /// <returns> The secure key to use to set/get the PlayerPref. </returns>
    private static Task<string> GetSecureKey(string baseKeyEncrypted, string key)
    {
        return Task.Run(() => GetKeyHash(string.Concat(baseKeyEncrypted.Unprotect(), key)));
    }
}