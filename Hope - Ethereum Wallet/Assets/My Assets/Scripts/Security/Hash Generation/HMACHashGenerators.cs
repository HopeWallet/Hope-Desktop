using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{
    /// <summary>
    /// Class used for generating <see cref="HMAC"/> hashes of string input.
    /// </summary>
    public static class HMACHashGenerators
    {
        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACMD5"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACMD5"/> hashed <see langword="string"/>. </returns>
        public static string HMACMD5(this string input) => HashGenerationHelpers.GetHash<HMACMD5>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACMD5"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACMD5"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACMD5(this byte[] input) => HashGenerationHelpers.GetHash<HMACMD5>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACRIPEMD160"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACRIPEMD160"/> hashed <see langword="string"/>. </returns>
        public static string HMACRIPEMD160(this string input) => HashGenerationHelpers.GetHash<HMACRIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACRIPEMD160"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACRIPEMD160"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACRIPEMD160(this byte[] input) => HashGenerationHelpers.GetHash<HMACRIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACSHA1"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACSHA1"/> hashed <see langword="string"/>. </returns>
        public static string HMACSHA1(this string input) => HashGenerationHelpers.GetHash<HMACSHA1>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.HMACSHA1"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.HMACSHA1"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACSHA1(this byte[] input) => HashGenerationHelpers.GetHash<HMACSHA1>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA256"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA256"/> hashed <see langword="string"/>. </returns>
        public static string HMACSHA2_256(this string input) => HashGenerationHelpers.GetHash<HMACSHA256>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA256"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA256"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACSHA2_256(this byte[] input) => HashGenerationHelpers.GetHash<HMACSHA256>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA384"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA384"/> hashed <see langword="string"/>. </returns>
        public static string HMACSHA2_384(this string input) => HashGenerationHelpers.GetHash<HMACSHA384>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA384"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA384"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACSHA2_384(this byte[] input) => HashGenerationHelpers.GetHash<HMACSHA384>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA512"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA512"/> hashed <see langword="string"/>. </returns>
        public static string HMACSHA2_512(this string input) => HashGenerationHelpers.GetHash<HMACSHA512>(input);

        /// <summary>
        /// Gets the <see cref="HMACSHA512"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="HMACSHA512"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] HMACSHA2_512(this byte[] input) => HashGenerationHelpers.GetHash<HMACSHA512>(input);
    }
}