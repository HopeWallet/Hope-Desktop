using Hope.Security;
using Hope.Security.Encryption;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Class that is used to securely and obscurely save data to the PlayerPrefs with a seemingly random name and encrypted data.
/// </summary>
public static class SecurePlayerPrefs
{

    private const string SECURE_PREF_SEED_NAME = "9bd1f75eb75c8ffad8f4b4c67c8f14db32cc3d4177b942334abd47f9e02e35b371d599cb4796185d7410e808f046e119";

    /// <summary>
    /// Initializes the SecurePlayerPrefs by making sure we have the base seed pref initialized.
    /// </summary>
    static SecurePlayerPrefs()
    {
        EnsureSeedCreation();
    }

    public static void GetStringAsync(string key, Action<string> onStringReceived) => InternalGetStringAsync(key, onStringReceived);

    private static async void InternalGetStringAsync(string key, Action<string> onStringReceived)
    {
        string baseKeyEncrypted = PlayerPrefs.GetString(SECURE_PREF_SEED_NAME);

        string secureKey = await Task.Run(() => string.Concat(baseKeyEncrypted.DPDecrypt(), key).GetSha384Hash());
        string secureEntropy = await Task.Run(() => secureKey.GetMd5Hash());

        string encryptedValue = PlayerPrefs.GetString(secureKey);

        onStringReceived?.Invoke(await Task.Run(() => encryptedValue.DPDecrypt(secureEntropy)));
    }

    /// <summary>
    /// Sets a string value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetString(string key, string value) => InternalSetString(key, value.ToString());

    /// <summary>
    /// Gets a string from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The string value returned from the PlayerPrefs with the given key. </returns>
    public static string GetString(string key) => InternalGetString(key);

    /// <summary>
    /// Sets an int value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetInt(string key, int value) => InternalSetString(key, value.ToString());

    /// <summary>
    /// Gets an int from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The int value returned from the PlayerPrefs with the given key. </returns>
    public static int GetInt(string key) => int.Parse(InternalGetString(key));

    /// <summary>
    /// Sets a float value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetFloat(string key, float value) => InternalSetString(key, value.ToString());

    /// <summary>
    /// Gets a float from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The float value returned from the PlayerPrefs with the given key. </returns>
    public static float GetFloat(string key) => float.Parse(InternalGetString(key));

    /// <summary>
    /// Deletes a key from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref to delete. </param>
    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(GetSecureKey(key));

    /// <summary>
    /// Checks if the PlayerPrefs contains a key.
    /// </summary>
    /// <param name="key"> The key to check for. </param>
    /// <returns> True if the key is contained in the PlayerPrefs. </returns>
    public static bool HasKey(string key) => PlayerPrefs.HasKey(GetSecureKey(key));

    /// <summary>
    /// Gets the secure key used to be the actual key for storing data in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key that is used to access the pref. </param>
    /// <returns> The secure, random text version of the key. </returns>
    private static string GetSecureKey(string key) => string.Concat(PlayerPrefs.GetString(SECURE_PREF_SEED_NAME).DPDecrypt(), key).GetSha384Hash();

    /// <summary>
    /// Sets a string to the PlayerPrefs after hashing the string values.
    /// </summary>
    /// <param name="key"> The key of the value in the PlayerPrefs. </param>
    /// <param name="value"> The value returned from the key in the PlayerPrefs. </param>
    private static void InternalSetString(string key, string value)
    {
        string secureKey = GetSecureKey(key);

        PlayerPrefs.SetString(secureKey, value.DPEncrypt(secureKey.GetMd5Hash()));
    }

    /// <summary>
    /// Gets a string from the PlayerPrefs after hashing the string values.
    /// </summary>
    /// <param name="key"> The key of the value in the PlayerPrefs. </param>
    /// <returns> The value returned from the key in the PlayerPrefs. </returns>
    private static string InternalGetString(string key)
    {
        string secureKey = GetSecureKey(key);

        return PlayerPrefs.GetString(secureKey).DPDecrypt(secureKey.GetMd5Hash());
    }

    /// <summary>
    /// Ensures the base seed for all secure key generation is created.
    /// </summary>
    private static void EnsureSeedCreation()
    {
        if (PlayerPrefs.HasKey(SECURE_PREF_SEED_NAME))
            return;

        PlayerPrefs.SetString(SECURE_PREF_SEED_NAME, PasswordUtils.GenerateRandomPassword().DPEncrypt());
    }

}