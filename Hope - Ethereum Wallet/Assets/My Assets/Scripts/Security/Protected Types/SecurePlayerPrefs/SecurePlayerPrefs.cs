using Hope.Security.Encryption.DPAPI;
using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using UnityEngine;

/// <summary>
/// Class that is used to securely and obscurely save data to the PlayerPrefs with a seemingly random name and encrypted data.
/// </summary>
public sealed class SecurePlayerPrefs : SecurePlayerPrefsBase
{
    /// <summary>
    /// Initializes the <see cref="SecurePlayerPrefs"/> by making sure we have the base seed pref initialized.
    /// </summary>
    public SecurePlayerPrefs()
    {
        EnsureSeedCreation();
    }

    /// <summary>
    /// Sets a string value in the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetString(string key, string value) => InternalSetString(key, value);

    /// <summary>
    /// Gets a string from the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The string value returned from the <see cref="PlayerPrefs"/> with the given key. </returns>
    public static string GetString(string key) => InternalGetString(key);

    /// <summary>
    /// Sets an int value in the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetInt(string key, int value) => InternalSetString(key, value.ToString());

    /// <summary>
    /// Gets an int from the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The int value returned from the <see cref="PlayerPrefs"/> with the given key. </returns>
    public static int GetInt(string key) => int.Parse(InternalGetString(key));

    /// <summary>
    /// Sets a float value in the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetFloat(string key, float value) => InternalSetString(key, value.ToString());

    /// <summary>
    /// Gets a float from the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The float value returned from the <see cref="PlayerPrefs"/> with the given key. </returns>
    public static float GetFloat(string key) => float.Parse(InternalGetString(key));

    /// <summary>
    /// Deletes a key from the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key of the pref to delete. </param>
    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(GetSecureKey(key));

    /// <summary>
    /// Checks if the <see cref="PlayerPrefs"/> contains a key.
    /// </summary>
    /// <param name="key"> The key to check for. </param>
    /// <returns> True if the key is contained in the <see cref="PlayerPrefs"/>. </returns>
    public static bool HasKey(string key) => PlayerPrefs.HasKey(GetSecureKey(key));

    /// <summary>
    /// Gets the secure key used to be the actual key for storing data in the <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="key"> The key that is used to access the pref. </param>
    /// <returns> The secure, random text version of the key. </returns>
    private static string GetSecureKey(string key) => GetKeyHash(string.Concat(GetSeedValue().Unprotect(), key));

    /// <summary>
    /// Sets a string to the <see cref="PlayerPrefs"/> after hashing the string values.
    /// </summary>
    /// <param name="key"> The key of the value in the <see cref="PlayerPrefs"/>. </param>
    /// <param name="value"> The value returned from the key in the <see cref="PlayerPrefs"/>. </param>
    private static void InternalSetString(string key, string value)
    {
        string secureKey = GetSecureKey(key);

        PlayerPrefs.SetString(secureKey, value.DPEncrypt(GetValueHash(secureKey)).Protect());
    }

    /// <summary>
    /// Gets a string from the <see cref="PlayerPrefs"/> after hashing the string values.
    /// </summary>
    /// <param name="key"> The key of the value in the <see cref="PlayerPrefs"/>. </param>
    /// <returns> The value returned from the key in the <see cref="PlayerPrefs"/>. </returns>
    private static string InternalGetString(string key)
    {
        string secureKey = GetSecureKey(key);

        return PlayerPrefs.GetString(secureKey).Unprotect().DPDecrypt(GetValueHash(secureKey));
    }
}