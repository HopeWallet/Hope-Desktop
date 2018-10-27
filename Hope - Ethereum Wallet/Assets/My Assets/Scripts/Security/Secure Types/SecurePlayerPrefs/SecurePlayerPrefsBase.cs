using Hope.Random.Bytes;
using Hope.Random.Strings;
using Hope.Security.HashGeneration;
using System;
using UnityEngine;

namespace Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base
{
    /// <summary>
    /// Base class for the different types of SecurePlayerPrefs to derive from.
    /// </summary>
    public abstract class SecurePlayerPrefsBase
    {
        protected static DataEncryptor dataEncryptor;

        private static Settings settings;

        /// <summary>
        /// Initializes the <see cref="SecurePlayerPrefsBase"/> by getting the base player pref value.
        /// </summary>
        /// <param name="prefSettings"> The Settings for the SecurePlayerPrefs. </param>
        protected SecurePlayerPrefsBase(Settings prefSettings)
        {
            settings = prefSettings;
            dataEncryptor = new DataEncryptor(settings.securePlayerPrefDataEntropy, GetSeedName(), RandomBytes.Secure.SHA3.GetBytes(GetSeedName(), 64).GetBase64String());

            EnsureSeedCreation();
        }

        /// <summary>
        /// Hashes the key using a specific HashAlgorithm. (SHA384 for now)
        /// </summary>
        /// <param name="key"> The key to hash. </param>
        /// <returns> The hashed key string. </returns>
        protected static string GetKeyHash(string key) => key.Keccak_256();

        /// <summary>
        /// Hashes the value using a specific HashAlgorithm. (SHA1 for now)
        /// </summary>
        /// <param name="value"> The value to hash. </param>
        /// <returns> The hashed value string. </returns>
        protected static string GetValueHash(string value) => value.SHA3_512();

        /// <summary>
        /// Gets the seed value from the player prefs.
        /// </summary>
        /// <returns> The seed value to use to derive all other SecurePlayerPrefs. </returns>
        protected static string GetSeedValue() => PlayerPrefs.GetString(GetSeedName());

        /// <summary>
        /// Ensures the base seed for all secure key generation is created.
        /// </summary>
        private void EnsureSeedCreation()
        {
            string seedName = GetSeedName();

            if (PlayerPrefs.HasKey(seedName))
                return;

            PlayerPrefs.SetString(seedName, dataEncryptor.Encrypt(RandomBytes.Secure.Blake2.GetBytes(128).Keccak_512().GetHexString()));
        }

        /// <summary>
        /// Gets the seed name based on this computer.
        /// </summary>
        /// <returns> The seed name of the PlayerPref. </returns>
        private static string GetSeedName()
        {
            return (Environment.MachineName.SHA3_256()
                   + Environment.OSVersion.Platform.ToString().SHA3_256()
                   + RandomString.Secure.SHA3.GetString(Environment.ProcessorCount.ToString(), 16).SHA3_256()
                   + settings.securePlayerPrefSeed.SHA3_256()).SHA3_512();
        }

        /// <summary>
        /// Class which holds the SecurePlayerPrefs settings.
        /// </summary>
        [Serializable]
        public sealed class Settings
        {
            [RandomizeText] public string securePlayerPrefSeed;
            [RandomizeText] public string securePlayerPrefDataEntropy;
        }
    }
}