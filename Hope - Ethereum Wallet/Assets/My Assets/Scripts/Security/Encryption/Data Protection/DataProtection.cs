using Hope.Security.HashGeneration;
using Org.BouncyCastle.Security;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which interacts with the ProtectedData and ProtectedMemory class to protect data.
    /// </summary>
    public static class DataProtection
    {

        private static readonly byte[] Entropy;

        /// <summary>
        /// Initializes the entropy to use to encrypt the data.
        /// </summary>
        static DataProtection()
        {
            SecureRandom secureRandom = new SecureRandom();
            Entropy = SecureRandom.GetNextBytes(secureRandom, 256);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Protects a piece of string text.
        /// </summary>
        /// <param name="data"> The string data. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(this string data)
        {
            ProtectedMemory.Unprotect(Entropy, MemoryProtectionScope.SameProcess);

            byte[] byteData = ProtectedData.Protect(data.FromBase64String(), Entropy, DataProtectionScope.CurrentUser);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
            ProtectedMemoryWrapper.Protect(byteData, MemoryProtectionScope.SameProcess);

            return byteData;
        }

        /// <summary>
        /// Unprotects a byte array of data.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static string Unprotect(this byte[] data)
        {
            ProtectedMemoryWrapper.Unprotect(data, MemoryProtectionScope.SameProcess);
            ProtectedMemory.Unprotect(Entropy, MemoryProtectionScope.SameProcess);

            string stringData = ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser).ToBase64String();

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
            
            return stringData;
        }
    }

}