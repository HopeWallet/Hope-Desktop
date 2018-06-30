using System.Security.Cryptography;
using System.Text;

namespace Hope.Security.HashGeneration
{

    /// <summary>
    /// Class which contains helper methods for hash generation.
    /// </summary>
    public static class HashGenerationHelpers
    {

        /// <summary>
        /// Gets the hash of a string using a given HashAlgorithm.
        /// </summary>
        /// <typeparam name="T"> The type of the HashAlgorithm. Must exist in the CryptoConfig.CreateFromName directory. </typeparam>
        /// <param name="input"> The string to get the hash for. </param>
        /// <returns> The hashed string. </returns>
        public static string GetHash<T>(string input) where T : HashAlgorithm
        {
            using (T hash = (T)CryptoConfig.CreateFromName(typeof(T).ToString()))
                return hash.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHexString();
        }

    }
}
