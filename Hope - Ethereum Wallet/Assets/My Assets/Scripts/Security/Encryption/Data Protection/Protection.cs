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
    public static class Protection
    {

        private static readonly byte[] Entropy;

        /// <summary>
        /// Initializes the entropy to use to encrypt the data.
        /// </summary>
        static Protection()
        {
            SecureRandom secureRandom = new SecureRandom();
            Entropy = SecureRandom.GetNextBytes(secureRandom, 256);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Protects a piece of string text in memory.
        /// ProtectMemory should only be used with UnprotectMemory in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The string data. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] ProtectMemory(this string data)
        {
            ProtectedMemory.Unprotect(Entropy, MemoryProtectionScope.SameProcess);

            byte[] byteData = ProtectedData.Protect(data.FromBase64String(), Entropy, DataProtectionScope.CurrentUser);

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);
            ProtectedMemoryWrapper.Protect(byteData, MemoryProtectionScope.SameProcess);

            return byteData;
        }

        /// <summary>
        /// Unprotects a byte array of data which was protected by ProtectMemory during this session history.
        /// UnprotectMemory should only be used with ProtectMemory in one session of program execution.
        /// Protected data in one session cannot be unprotected in another session.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static string UnprotectMemory(this byte[] data)
        {
            ProtectedMemoryWrapper.Unprotect(data, MemoryProtectionScope.SameProcess);
            ProtectedMemory.Unprotect(Entropy, MemoryProtectionScope.SameProcess);

            string stringData = ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser).ToBase64String();

            ProtectedMemory.Protect(Entropy, MemoryProtectionScope.SameProcess);

            return stringData;
        }

        public static string ProtectStorage(this string data) => ProtectStorage(data, null);

        public static string UnprotectStorage(this string data) => UnprotectStorage(data, null);

        public static string ProtectStorage(this string data, string optionalEntropy) => ProtectStorage(data, optionalEntropy, DataProtectionScope.CurrentUser);

        public static string UnprotectStorage(this string data, string optionalEntropy) => UnprotectStorage(data, optionalEntropy, DataProtectionScope.CurrentUser);

        public static string ProtectStorage(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalProtectStorage(data.DPEncrypt(optionalEntropy).FromBase64String(), optionalEntropy.FromBase64String(), scope).ToBase64String();

        public static string UnprotectStorage(this string data, string optionalEntropy, DataProtectionScope scope)
            => InternalUnprotectStorage(data.FromBase64String(), optionalEntropy.FromBase64String(), scope).ToBase64String().DPDecrypt(optionalEntropy);

        private static byte[] InternalProtectStorage(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Protect(data, optionalEntropy, scope);

        private static byte[] InternalUnprotectStorage(byte[] data, byte[] optionalEntropy, DataProtectionScope scope) => ProtectedData.Unprotect(data, optionalEntropy, scope);


    }

}