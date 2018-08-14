using Org.BouncyCastle.Crypto;

namespace Hope.Utils.Random.Abstract
{
    /// <summary>
    /// Base class used for generating random strings.
    /// </summary>
    /// <typeparam name="T"> The hash algorithm to use to generate our random strings. </typeparam>
    public abstract class RandomStringBase<T> where T : IDigest, new()
    {
        /// <summary>
        /// Generates a random <see langword="string"/> using the specified algorithm.
        /// <para> Uses a default length of 16. </para>
        /// </summary>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        public static string GetString() => GetString(16);

        /// <summary>
        /// Generates a random <see langword="string"/> using the specified algorithm and a seed.
        /// <para> Uses a default length of 16. </para>
        /// </summary>
        /// <param name="seed"> The seed to apply to the random <see langword="string"/> generation. </param>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        public static string GetString(string seed) => GetString(seed.GetUTF8Bytes());

        /// <summary>
        /// Generates a random <see langword="string"/> using the specified algorithm and a seed.
        /// <para> Uses a default length of 16. </para>
        /// </summary>
        /// <param name="seed"> The seed to apply to the random <see langword="string"/> generation. </param>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        public static string GetString(byte[] seed) => GetString(seed, 16);

        /// <summary>
        /// Generates a random <see langword="string"/> of a given length using the specified algorithm.
        /// </summary>
        /// <param name="length"> The length of the random <see langword="string"/>. </param>
        /// <returns> The randomly generated <see langword="string"/> </returns>
        public static string GetString(int length) => GetString((byte[])null, length);

        /// <summary>
        /// Generates a random <see langword="string"/> of a given length using the specified algorithm and a seed.
        /// </summary>
        /// <param name="seed"> The seed to apply to the random <see langword="string"/> generation. </param>
        /// <param name="length"> The length of the random <see langword="string"/>. </param>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        public static string GetString(string seed, int length) => GetString(seed.GetUTF8Bytes(), length);

        /// <summary>
        /// Generates a random <see langword="string"/> of a given length using the specified algorithm and a seed.
        /// </summary>
        /// <param name="seed"> The seed to apply to the random <see langword="string"/> generation. </param>
        /// <param name="length"> The length of the random <see langword="string"/>. </param>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        public static string GetString(byte[] seed, int length) => InternalGetString(seed, length, new T());

        /// <summary>
        /// Generates a random <see langword="string"/> of a given length using the specified <see cref="IDigest"/> and a seed.
        /// </summary>
        /// <param name="seed"> The seed to apply to the random <see langword="string"/> generation. </param>
        /// <param name="length"> The length of the random <see langword="string"/>. </param>
        /// <param name="digest"> The <see cref="IDigest"/> object to use to generate the random <see langword="string"/>. </param>
        /// <returns> The randomly generated <see langword="string"/>. </returns>
        private static string InternalGetString(byte[] seed, int length, IDigest digest)
        {
            return (seed == null ? new AdvancedSecureRandom(digest) : new AdvancedSecureRandom(digest, seed)).NextBytes(length).GetBase64String().LimitEnd(length);
        }
    }
}