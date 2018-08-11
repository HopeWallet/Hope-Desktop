using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{
    /// <summary>
    /// Class which contains helper methods for hash generation.
    /// </summary>
    public static class HashGenerationHelpers
    {
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
