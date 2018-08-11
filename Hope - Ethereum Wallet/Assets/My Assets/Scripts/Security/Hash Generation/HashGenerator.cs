using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{
    /// <summary>
    /// Class which contains a series of methods for generating hashes for string data based on different hash algorithms.
    /// </summary>
    public static class HashGenerator
    {
        /// <summary>
        /// Gets the <see cref="MD5"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="MD5"/> hashed <see langword="string"/>. </returns>
        public static string GetMD5Hash(this string input) => HashGenerationHelpers.GetHash<MD5>(input);

        /// <summary>
        /// Gets the <see cref="MD5"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="MD5"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetMD5Hash(this byte[] input) => HashGenerationHelpers.GetHash<MD5>(input);

        /// <summary>
        /// Gets the <see cref="RIPEMD160"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="RIPEMD160"/> hashed <see langword="string"/>. </returns>
        public static string GetRIPEMD160Hash(this string input) => HashGenerationHelpers.GetHash<RIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="RIPEMD160"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="RIPEMD160"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetRIPEMD160Hash(this byte[] input) => HashGenerationHelpers.GetHash<RIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="SHA1"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA1"/> hashed <see langword="string"/>. </returns>
        public static string GetSHA1Hash(this string input) => HashGenerationHelpers.GetHash<SHA1>(input);

        /// <summary>
        /// Gets the <see cref="SHA1"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA1"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA1Hash(this byte[] input) => HashGenerationHelpers.GetHash<SHA1>(input);

        /// <summary>
        /// Gets the <see cref="SHA256"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA256"/> hashed <see langword="string"/>. </returns>
        public static string GetSHA256Hash(this string input) => HashGenerationHelpers.GetHash<SHA256>(input);

        /// <summary>
        /// Gets the <see cref="SHA256"/> hash of a <see langword="byte"[]/> input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA256"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA256Hash(this byte[] input) => HashGenerationHelpers.GetHash<SHA256>(input);

        /// <summary>
        /// Gets the <see cref="SHA384"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA384"/> hashed <see langword="string"/>. </returns>
        public static string GetSHA384Hash(this string input) => HashGenerationHelpers.GetHash<SHA384>(input);

        /// <summary>
        /// Gets the <see cref="SHA384"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA384"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA384Hash(this byte[] input) => HashGenerationHelpers.GetHash<SHA384>(input);

        /// <summary>
        /// Gets the <see cref="SHA512"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA512"/> hashed <see langword="string"/>. </returns>
        public static string GetSHA512Hash(this string input) => HashGenerationHelpers.GetHash<SHA512>(input);

        /// <summary>
        /// Gets the <see cref="SHA512"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA512"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA512Hash(this byte[] input) => HashGenerationHelpers.GetHash<SHA512>(input);
    }
}