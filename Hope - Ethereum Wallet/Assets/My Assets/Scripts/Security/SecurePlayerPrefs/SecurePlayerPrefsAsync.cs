using Hope.Security.Encryption;
using Hope.Security.SecurePlayerPrefs.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SecurePlayerPrefsAsync : SecurePlayerPrefsBase
{
    
    static SecurePlayerPrefsAsync()
    {
        EnsureSeedCreation();
    }

    public static void GetString(string key, Action<string> onStringReceived) => InternalGetString(key, onStringReceived);

    public static void GetInt(string key, Action<int> onIntReceived) => InternalGetString(key, str => onIntReceived?.Invoke(int.Parse(str)));

    public static void GetFloat(string key, Action<float> onFloatReceived) => InternalGetString(key, str => onFloatReceived?.Invoke(float.Parse(str)));

    public static void SetString(string key, string value, Action onStringSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value, onStringSet));

    public static void SetInt(string key, int value, Action onIntSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onIntSet));

    public static void SetFloat(string key, float value, Action onFloatSet = null) => AsyncTaskScheduler.Schedule(() => InternalSetString(key, value.ToString(), onFloatSet));

    private static async Task InternalSetString(string key, string value, Action onValueSet)
    {
        string secureKey = await GetSecureKey(key);
        string secureEntropy = await Task.Run(() => ValueHash(secureKey));
        string encryptedValue = await Task.Run(() => value.DPEncrypt(secureEntropy));

        PlayerPrefs.SetString(secureKey, encryptedValue);
        onValueSet?.Invoke();
    }

    private static async void InternalGetString(string key, Action<string> onStringReceived)
    {
        string secureKey = await GetSecureKey(key);
        string secureEntropy = await Task.Run(() => ValueHash(secureKey));
        string encryptedValue = PlayerPrefs.GetString(secureKey);

        onStringReceived?.Invoke(await Task.Run(() => encryptedValue.DPDecrypt(secureEntropy)));
    }

    private static async Task<string> GetSecureKey(string key)
    {
        string baseKeyEncrypted = PlayerPrefs.GetString(SECURE_PREF_SEED_NAME);

        return await Task.Run(() => KeyHash(string.Concat(baseKeyEncrypted.DPDecrypt(), key)));
    }
}