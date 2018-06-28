using UnityEngine;

public static class SecurePlayerPrefs
{

    private const string SECURE_PREF_SEED_NAME = "0x0000000000000000000000000000000000000000";

    static SecurePlayerPrefs()
    {
        EnsureSeedCreation();
    }

    public static void SetString(string key, string value) => PlayerPrefs.SetString(GetSecureKey(key), value);

    public static string GetString(string key) => PlayerPrefs.GetString(GetSecureKey(key));

    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(GetSecureKey(key));

    public static bool HasKey(string key) => PlayerPrefs.HasKey(GetSecureKey(key));

    private static string GetSecureKey(string key) => RandomUtils.GenerateSeededRandomString(PlayerPrefs.GetString(SECURE_PREF_SEED_NAME).DPDecrypt() + key);

    private static void EnsureSeedCreation()
    {
        if (PlayerPrefs.HasKey(SECURE_PREF_SEED_NAME))
            return;

        PlayerPrefs.SetString(SECURE_PREF_SEED_NAME, PasswordUtils.GenerateRandomPassword().DPEncrypt());
    }

}