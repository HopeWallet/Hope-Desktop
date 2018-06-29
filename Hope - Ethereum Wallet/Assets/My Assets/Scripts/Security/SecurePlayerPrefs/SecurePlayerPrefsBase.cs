using Hope.Security.Encryption;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace Hope.Security.SecurePlayerPrefs.Base
{

    /// <summary>
    /// Base class for the different types of SecurePlayerPrefs to derive from.
    /// </summary>
    public abstract class SecurePlayerPrefsBase
    {

        /// <summary>
        /// Hashes the key using a specific HashAlgorithm.
        /// </summary>
        /// <param name="input"> The key to hash. </param>
        /// <returns> The hashed key string. </returns>
        protected static string GetKeyHash(string input) => input.GetSHA384Hash();

        /// <summary>
        /// Hashes the value using a specific HashAlgorithm.
        /// </summary>
        /// <param name="input"> The value to hash. </param>
        /// <returns> The hashed value string. </returns>
        protected static string GetValueHash(string input) => input.GetSHA1Hash();

        protected static string GetSeedValue() => PlayerPrefs.GetString(GetSeedName());

        /// <summary>
        /// Ensures the base seed for all secure key generation is created.
        /// </summary>
        protected static void EnsureSeedCreation()
        {
            string seedName = GetSeedName();

            if (PlayerPrefs.HasKey(seedName))
                return;

            PlayerPrefs.SetString(seedName, PasswordUtils.GenerateRandomPassword().GetSHA512Hash().DPEncrypt());
        }

        private static string GetSeedName()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up)
                    return GetKeyHash(Encoding.UTF8.GetBytes(nic.Id).Concat(nic.GetPhysicalAddress().GetAddressBytes()).ToArray().GetHexString());

            return null;
        }
    }
}