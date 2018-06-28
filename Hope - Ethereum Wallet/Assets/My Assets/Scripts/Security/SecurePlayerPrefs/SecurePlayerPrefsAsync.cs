using Hope.Security;
using Hope.Security.Encryption;
using Hope.Security.SecurePlayerPrefs.Base;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class SecurePlayerPrefsAsync : SecurePlayerPrefsBase
{

    static SecurePlayerPrefsAsync()
    {
        EnsureSeedCreation();
    }

    public static void GetStringAsync(string key, Action<string> onStringReceived) => InternalGetStringAsync(key, onStringReceived);

    public static void GetIntAsync(string key, Action<int> onIntReceived) => InternalGetStringAsync(key, str => onIntReceived?.Invoke(int.Parse(str)));

    public static void GetFloatAsync(string key, Action<float> onFloatReceived) => InternalGetStringAsync(key, str => onFloatReceived?.Invoke(float.Parse(str)));

    public static void SetStringAsync(string key, string value, Action onStringSet = null) => InternalSetStringAsync(key, value, onStringSet);

    public static void SetIntAsync(string key, int value, Action onIntSet = null) => InternalSetStringAsync(key, value.ToString(), onIntSet);

    public static void SetFloatAsync(string key, float value, Action onFloatSet = null) => InternalSetStringAsync(key, value.ToString(), onFloatSet);

    private static async void InternalSetStringAsync(string key, string value, Action onStringSet)
    {
        string secureKey = await GetSecureKeyAsync(key);
        string secureEntropy = await Task.Run(() => secureKey.GetMD5Hash());
        string encryptedValue = await Task.Run(() => value.DPEncrypt(secureEntropy));

        PlayerPrefs.SetString(secureKey, encryptedValue);
        onStringSet?.Invoke();
    }

    private static async void InternalGetStringAsync(string key, Action<string> onStringReceived)
    {
        string secureKey = await GetSecureKeyAsync(key);
        string secureEntropy = await Task.Run(() => secureKey.GetMD5Hash());
        string encryptedValue = PlayerPrefs.GetString(secureKey);

        onStringReceived?.Invoke(await Task.Run(() => encryptedValue.DPDecrypt(secureEntropy)));
    }

    private static async Task<string> GetSecureKeyAsync(string key)
    {
        string baseKeyEncrypted = PlayerPrefs.GetString(SECURE_PREF_SEED_NAME);

        return await Task.Run(() => string.Concat(baseKeyEncrypted.DPDecrypt(), key).GetSHA384Hash());
    }

}