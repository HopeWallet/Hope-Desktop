using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Org.BouncyCastle.Security;
using UnityEngine;

namespace Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base
{
    /// <summary>
    /// Base class for the different types of SecurePlayerPrefs to derive from.
    /// </summary>
    public abstract class SecurePlayerPrefsBase
    {
        /// <summary>
        /// Initializes the <see cref="SecurePlayerPrefsBase"/> by getting the base player pref value.
        /// </summary>
        protected SecurePlayerPrefsBase()
        {
            EnsureSeedCreation();
        }

        /// <summary>
        /// Hashes the key using a specific HashAlgorithm. (SHA384 for now)
        /// </summary>
        /// <param name="key"> The key to hash. </param>
        /// <returns> The hashed key string. </returns>
        protected static string GetKeyHash(string key) => key.GetSHA384Hash();

        /// <summary>
        /// Hashes the value using a specific HashAlgorithm. (SHA1 for now)
        /// </summary>
        /// <param name="value"> The value to hash. </param>
        /// <returns> The hashed value string. </returns>
        protected static string GetValueHash(string value) => value.GetSHA512Hash();

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

            PlayerPrefs.SetString(seedName, SecureRandom.GetNextBytes(new SecureRandom(), 128).GetBase64String().GetSHA512Hash().Protect());
        }

        /// <summary>
        /// Gets the seed name based on this computer.
        /// </summary>
        /// <returns> The seed name of the PlayerPref. </returns>
        private static string GetSeedName()
        {
            return ("TEST").GetSHA512Hash();
        }
    }
}