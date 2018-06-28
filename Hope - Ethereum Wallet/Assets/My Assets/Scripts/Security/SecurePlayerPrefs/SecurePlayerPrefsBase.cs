using Hope.Security.Encryption;
using UnityEngine;

namespace Hope.Security.SecurePlayerPrefs.Base
{

    /// <summary>
    /// Base class for the different types of SecurePlayerPrefs to derive from.
    /// </summary>
    public class SecurePlayerPrefsBase
    {

        protected const string SECURE_PREF_SEED_NAME = "9bd1f75eb75c8ffad8f4b4c67c8f14db32cc3d4177b942334abd47f9e02e35b371d599cb4796185d7410e808f046e119";

        /// <summary>
        /// Ensures the base seed for all secure key generation is created.
        /// </summary>
        protected static void EnsureSeedCreation()
        {
            if (PlayerPrefs.HasKey(SECURE_PREF_SEED_NAME))
                return;

            PlayerPrefs.SetString(SECURE_PREF_SEED_NAME, PasswordUtils.GenerateRandomPassword().DPEncrypt());
        }
    }
}