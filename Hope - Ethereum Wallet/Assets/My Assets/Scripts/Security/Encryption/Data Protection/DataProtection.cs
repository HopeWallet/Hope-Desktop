using System.Security.Cryptography;
using System.Text;

namespace Hope.Security.Encryption
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
            //byte[] byteData = ProtectedData.Protect(Encoding.UTF8.GetBytes(data), Entropy, DataProtectionScope.CurrentUser);
            //byteData.Length.Log();
            //ProtectedMemory.Protect(byteData, MemoryProtectionScope.SameProcess);

            return ProtectedData.Protect(Encoding.UTF8.GetBytes(data), Entropy, DataProtectionScope.CurrentUser)/*byteData*/;
        }

        /// <summary>
        /// Unprotects a byte array of data.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static string Unprotect(this byte[] data)
        {
            //ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameProcess);
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser));
        }
    }

}