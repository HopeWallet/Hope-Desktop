using Hope.Security;
using Hope.Security.Encryption;
using UnityEngine;

/// <summary>
/// Class that is used to securely and obscurely save data to the PlayerPrefs with a seemingly random name and encrypted data.
/// </summary>
public static class SecurePlayerPrefs
{

    private const string SECURE_PREF_SEED_NAME = "i2E.z4cF^lVo_m-+cw,p@cj;)hAqQBFDp3d{Zi+l";

    /// <summary>
    /// Initializes the SecurePlayerPrefs by making sure we have the base seed pref initialized.
    /// </summary>
    static SecurePlayerPrefs()
    {
        EnsureSeedCreation();
    }

    /// <summary>
    /// Sets a string value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetString(string key, string value) => PlayerPrefs.SetString(GetSecureKey(key), value.DPEncrypt());

    /// <summary>
    /// Gets a string from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The string value returned from the PlayerPrefs with the given key. </returns>
    public static string GetString(string key) => PlayerPrefs.GetString(GetSecureKey(key)).DPDecrypt();

    /// <summary>
    /// Sets an int value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetInt(string key, int value) => SetString(key, value.ToString());

    /// <summary>
    /// Gets an int from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The int value returned from the PlayerPrefs with the given key. </returns>
    public static int GetInt(string key) => int.Parse(PlayerPrefs.GetString(GetSecureKey(key)).DPDecrypt());

    /// <summary>
    /// Sets a float value in the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <param name="value"> The value of the pref. </param>
    public static void SetFloat(string key, float value) => SetString(key, value.ToString());

    /// <summary>
    /// Gets a float from the PlayerPrefs.
    /// </summary>
    /// <param name="key"> The key of the pref. </param>
    /// <returns> The float value returned from the PlayerPrefs with the given key. </returns>
    public static float GetFloat(string key) => float.Parse(PlayerPrefs.GetString(GetSecureKey(key)).DPDecrypt());

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
    private static string GetSecureKey(string key) => string.Concat(PlayerPrefs.GetString(SECURE_PREF_SEED_NAME).DPDecrypt(), key).GetSha512Hash();

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