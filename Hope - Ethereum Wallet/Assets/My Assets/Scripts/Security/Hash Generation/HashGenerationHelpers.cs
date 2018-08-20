using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{
    /// <summary>
    /// Class which contains helper methods for hash generation.
    /// </summary>
    public static class HashGenerationHelpers
    {
        /// <summary>
        /// Gets the hash of some <see cref="string"/> input given the digest for processing the hash.
        /// </summary>
        /// <param name="input"> The <see cref="string"/> input to hash. </param>
        /// <param name="digest"> The <see cref="IDigest"/> used for processing the hash. </param>
        /// <returns> The hashed <see cref="string"/> value. </returns>
        public static string GetHash(string input, IDigest digest)
        {
            return GetHash(input.GetUTF8Bytes(), digest).GetHexString();
        }

        /// <summary>
        /// Gets the hash of some <see cref="byte"/>[] input given the digest for processing the hash.
        /// </summary>
        /// <param name="input"> The <see cref="byte"/>[] input to hash. </param>
        /// <param name="digest"> The <see cref="IDigest"/> used for processing the hash. </param>
        /// <returns> The hashed <see cref="byte[]"/> value. </returns>
        public static byte[] GetHash(byte[] input, IDigest digest)
        {
            byte[] output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(input, 0, input.Length);
            digest.DoFinal(output, 0);

            return output;
        }

        /// <summary>
        /// Gets the hash of a string using a given <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <typeparam name="T"> The type of the <see cref="HashAlgorithm"/>. Must exist in the <see cref="CryptoConfig.CreateFromName"/> directory. </typeparam>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The hashed <see langword="string"/>. </returns>
        public static string GetHash<T>(string input) where T : HashAlgorithm
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return ComputeHash<T>(input.GetUTF8Bytes()).GetHexString();
        }

        /// <summary>
        /// Gets the hash of some <see langword="byte"/>[] input using a given <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <typeparam name="T"> The type of the <see cref="HashAlgorithm"/>. Must exist in the <see cref="CryptoConfig.CreateFromName"/> directory. </typeparam>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The hashed <see langword="byte"/>[] data. </returns>
        public static byte[] GetHash<T>(byte[] input) where T : HashAlgorithm
        {
            if (input == null || input.Length == 0)
                return null;

            return ComputeHash<T>(input);
        }

        /// <summary>
        /// Computes the hash of some <see langword="byte"/>[] data given the <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <typeparam name="T"> The type of the <see cref="HashAlgorithm"/>. Must exist in the <see cref="CryptoConfig.CreateFromName"/> directory. </typeparam>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The hashed <see langword="byte"/>[] data. </returns>
        private static byte[] ComputeHash<T>(byte[] input) where T : HashAlgorithm
        {
            using (T hash = (T)CryptoConfig.CreateFromName(typeof(T).ToString()))
                return hash.ComputeHash(input);
        }
    }
}
