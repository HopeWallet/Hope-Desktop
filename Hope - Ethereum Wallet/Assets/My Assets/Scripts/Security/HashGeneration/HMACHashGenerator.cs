using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{

    /// <summary>
    /// Class used for generating <see cref="HMAC"/> hashes of string input.
    /// </summary>
    public static class HMACHashGenerator
    {

        /// <summary>
        /// Gets the <see cref="HMACMD5"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACMD5"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACMD5Hash(this string input) => HashGenerationHelpers.GetHash<HMACMD5>(input);

        /// <summary>
        /// Gets the <see cref="HMACRIPEMD160"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACRIPEMD160"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACRIPEMD160Hash(this string input) => HashGenerationHelpers.GetHash<HMACRIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA1"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA1"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACSHA1Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA1>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA256"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA256"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACSHA256Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA256>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA384"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA384"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACSHA384Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA384>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA512"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA512"/> hashed <see langword="string"/>. </returns>
        public static string GetHMACSHA512Hash(this string input) => HashGenerationHelpers.GetHash<HMACSHA512>(input);

    }
}
