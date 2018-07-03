using Hope.Security.Encryption.DPAPI;
using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class used for saving data to SecurePlayerPrefs asynchronously.
/// Useful in the case where a lot of data needs to be saved.
/// </summary>
public sealed class SecurePlayerPrefsAsync : SecurePlayerPrefsBase, IUpdater
{

    /// <summary>
    /// Initializes the SecurePlayerPrefsAsync by ensuring we have the base seed created.
    /// </summary>
    public SecurePlayerPrefsAsync(UpdateManager updateManager)
    {
        updateManager.AddUpdater(this);
        EnsureSeedCreation();
    }

    public void UpdaterUpdate()
    {
    }

    /// <summary>
    /// Gets a string from the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the value located in the <see cref="PlayerPrefs"/>. </param>
    /// <param name="onStringReceived"> Action to call when the string has been received. </param>
    public static void GetString(string key, Action<string> onStringReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, onStringReceived));

    /// <summary>
    /// Gets an int from the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the int value located in the <see cref="PlayerPrefs"/>. </param>
    /// <param name="onIntReceived"> Action to call when the int value has been received. </param>
    public static void GetInt(string key, Action<int> onIntReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, str => onIntReceived?.Invoke(int.Parse(str))));

    /// <summary>
    /// Gets a float from the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the float value located in the <see cref="PlayerPrefs"/>. </param>
    /// <param name="onFloatReceived"> Action to call when the float value has been received. </param>
    public static void GetFloat(string key, Action<float> onFloatReceived) => AsyncTaskScheduler.Schedule(() => InternalGetString(key, str => onFloatReceived?.Invoke(float.Parse(str))));

    /// <summary>
    /// Sets a string value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onStringSet"> Optional action to call when the string value has been set. </param>
    public static void SetString(string key, string value, Action onStringSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value, onStringSet));

    /// <summary>
    /// Sets an int value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onIntSet"> Optional action to call when the int value has been set. </param>
    public static void SetInt(string key, int value, Action onIntSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onIntSet));

    /// <summary>
    /// Sets a float value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key of the PlayerPref to set. </param>
    /// <param name="value"> The value of the PlayerPref to set. </param>
    /// <param name="onFloatSet"> Optional action to call when the float value has been set. </param>
    public static void SetFloat(string key, float value, Action onFloatSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onFloatSet));

    /// <summary>
    /// Sets a string value to the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// Gets the secure key, secure entropy, and encrypted value all asynchronously.
    /// </summary>
    /// <param name="key"> The key to use to set the PlayerPref. </param>
    /// <param name="value"> The value to use for the PlayerPref. </param>
    /// <param name="onValueSet"> Action called once the PlayerPref has been set successfully. </param>
    /// <returns> The task for creating the hashed key/values and setting the PlayerPref. </returns>
    private static async Task InternalSetString(string key, string value, Action onValueSet)
    {
        string secureKey = await GetSecureKey(key).ConfigureAwait(false);
        string secureEntropy = await Task.Run(() => GetValueHash(secureKey)).ConfigureAwait(false);
        string encryptedValue = await Task.Run(() => value.DPEncrypt(secureEntropy).Protect()).ConfigureAwait(false);

        PlayerPrefs.SetString(secureKey, encryptedValue);
        onValueSet?.Invoke();
    }

    /// <summary>
    /// Gets a string value from the <see cref="SecurePlayerPrefs"/> asynchronously.
    /// </summary>
    /// <param name="key"> The key to use to get the value from the PlayerPrefs. </param>
    /// <param name="onStringReceived"> Action called once the string value has been received. </param>
    /// <returns> The task for creating the hashed key and retrieving the string from the PlayerPrefs. </returns>
    private static async Task InternalGetString(string key, Action<string> onStringReceived)
    {
        string secureKey = await GetSecureKey(key).ConfigureAwait(false);
        string secureEntropy = await Task.Run(() => GetValueHash(secureKey)).ConfigureAwait(false);
        string encryptedValue = PlayerPrefs.GetString(secureKey);

        onStringReceived?.Invoke(await Task.Run(() => encryptedValue.Unprotect().DPDecrypt(secureEntropy)).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the encrypted and hashed key asynchronously.
    /// </summary>
    /// <param name="key"> The unencrypted key to use to derive the encrypted and hashed key. </param>
    /// <returns> The secure key to use to set/get the PlayerPref. </returns>
    private static Task<string> GetSecureKey(string key)
    {
        string baseKeyEncrypted = GetSeedValue();

        return Task.Run(() => GetKeyHash(string.Concat(baseKeyEncrypted.Unprotect(), key)));
    }
}