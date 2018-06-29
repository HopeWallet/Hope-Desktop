using Hope.Security.HashGeneration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Hope.Security.Encryption.DPAPI
{

    /// <summary>
    /// Class which interacts with the ProtectedData and ProtectedMemory class to protect data.
    /// </summary>
    public static class DataProtection
    {

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes(PasswordUtils.GenerateRandomPassword().GetSHA512Hash());

        /// <summary>
        /// Protects a piece of string text.
        /// </summary>
        /// <param name="data"> The string data. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(this string data)
        {
            byte[] byteData = ProtectedData.Protect(Convert.FromBase64String(data), Entropy, DataProtectionScope.CurrentUser).PadData();
            ProtectedMemory.Protect(byteData, MemoryProtectionScope.SameProcess);
            return byteData;
        }

        /// <summary>
        /// Unprotects a byte array of data.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static string Unprotect(this byte[] data)
        {
            ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameProcess);
            return Convert.ToBase64String(ProtectedData.Unprotect(data.UnpadData(), Entropy, DataProtectionScope.CurrentUser));
        }
    }

}