using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{

    /// <summary>
    /// Class which contains a series of methods for generating hashes for string data based on different hash algorithms.
    /// </summary>
    public static class HashGenerator
    {

        /// <summary>
        /// Gets the MD5 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The MD5 hashed string. </returns>
        public static string GetMD5Hash(this string input) => HashGenerationHelpers.GetHash<MD5>(input);

        /// <summary>
        /// Gets the RIPEMD160 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The RIPEMD160 hashed string. </returns>
        public static string GetRIPEMD160Hash(this string input) => HashGenerationHelpers.GetHash<RIPEMD160>(input);

        /// <summary>
        /// Gets the SHA1 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The SHA1 hashed string. </returns>
        public static string GetSHA1Hash(this string input) => HashGenerationHelpers.GetHash<SHA1>(input);

        /// <summary>
        /// Gets the SHA256 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The SHA256 hashed string. </returns>
        public static string GetSHA256Hash(this string input) => HashGenerationHelpers.GetHash<SHA256>(input);

        /// <summary>
        /// Gets the SHA384 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The SHA384 hashed string. </returns>
        public static string GetSHA384Hash(this string input) => HashGenerationHelpers.GetHash<SHA384>(input);

        /// <summary>
        /// Gets the SHA512 hash of a string input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The SHA512 hashed string. </returns>
        public static string GetSHA512Hash(this string input) => HashGenerationHelpers.GetHash<SHA512>(input);

    }

}