using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{

    /// <summary>
    /// Class used for generating HMAC hashes of string input.
    /// </summary>
    public static class HMACHashGenerator
    {

        /// <summary>
        /// Gets the HMACMD5 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACMD5 hashed string. </returns>
        public static string GetHMACMD5Hash(this string input) => HashGenerationHelpers.GetHash<HMACMD5>(input);

        /// <summary>
        /// Gets the HMACRIPEMD160 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACRIPEMD160 hashed string. </returns>
        public static string GetHMACRIPEMD160Hash(this string input) => HashGenerationHelpers.GetHash<HMACRIPEMD160>(input);

        /// <summary>
        /// Gets the HMACSHA1 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACSHA1 hashed string. </returns>
        public static string GetHMACSHA1Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA1>(input);

        /// <summary>
        /// Gets the HMACSHA256 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACSHA256 hashed string. </returns>
        public static string GetHMACSHA256Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA256>(input);

        /// <summary>
        /// Gets the HMACSHA384 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACSHA384 hashed string. </returns>
        public static string GetHMACSHA384Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA384>(input);

        /// <summary>
        /// Gets the HMACSHA512 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The HMACSHA512 hashed string. </returns>
        public static string GetHMACSHA512Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA512>(input);

    }
}
