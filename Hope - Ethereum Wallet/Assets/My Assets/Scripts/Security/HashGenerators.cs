using System.Security.Cryptography;
using System.Text;

namespace Hope.Security
{

    /// <summary>
    /// Class which contains a series of methods for generating hashes for string values based on different hash algorithms.
    /// </summary>
    public static class HashGenerators
    {

        /// <summary>
        /// Gets the MD5 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetMD5Hash(this string input) => GetHash<MD5>(input);

        /// <summary>
        /// Gets the SHA1 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetSHA1Hash(this string input) => GetHash<SHA1>(input);

        /// <summary>
        /// Gets the SHA256 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetSHA256Hash(this string input) => GetHash<SHA256>(input);

        /// <summary>
        /// Gets the SHA384 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetSHA384Hash(this string input) => GetHash<SHA384>(input);

        /// <summary>
        /// Gets the SHA512 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetSHA512Hash(this string input) => GetHash<SHA512>(input);

        /// <summary>
        /// Gets the hash of a string using a given HashAlgorithm.
        /// </summary>
        /// <typeparam name="T"> The type of the HashAlgorithm. Must exist in the CryptoConfig.CreateFromName directory. </typeparam>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        private static string GetHash<T>(string input) where T : HashAlgorithm
        {
            using (T hash = (T)CryptoConfig.CreateFromName(typeof(T).ToString()))
                return hash.ComputeHash(Encoding.UTF8.GetBytes(input)).GetHexString();
        }
    }

}