using System.Security.Cryptography;
using System.Text;

namespace Hope.Security.Encryption
{

    /// <summary>
    /// Class which interacts with the ProtectedData class to protect string data.
    /// </summary>
    public static class Protection
    {

        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes(PasswordUtils.GenerateRandomPassword().GetSHA512Hash());

        /// <summary>
        /// Protects a piece of string text.
        /// </summary>
        /// <param name="data"> The string data. </param>
        /// <returns> The protected data as a byte array. </returns>
        public static byte[] Protect(this string data) => ProtectedData.Protect(Encoding.UTF8.GetBytes(data), Entropy, DataProtectionScope.CurrentUser);

        /// <summary>
        /// Unprotects a byte array of data.
        /// </summary>
        /// <param name="data"> The data to unprotect. </param>
        /// <returns> The unprotected string retrieved from the data. </returns>
        public static string Unprotect(this byte[] data) => Encoding.UTF8.GetString(ProtectedData.Unprotect(data, Entropy, DataProtectionScope.CurrentUser));

    }

}