﻿using Hope.Security.HashGeneration;
using Hope.Utils.Random;
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
            dataEncryptor = new DataEncryptor(settings.securePlayerPrefDataEntropy, GetSeedName(), RandomBytes.SHA256.GetBytes(GetSeedName(), 64).GetBase64String());

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

            PlayerPrefs.SetString(seedName, dataEncryptor.Encrypt(RandomBytes.SHA256.GetBytes(128).GetSHA512Hash().GetHexString()));
        }

        /// <summary>
        /// Gets the seed name based on this computer.
        /// </summary>
        /// <returns> The seed name of the PlayerPref. </returns>
        private static string GetSeedName()
        {
            return settings.securePlayerPrefSeed.GetSHA512Hash();
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