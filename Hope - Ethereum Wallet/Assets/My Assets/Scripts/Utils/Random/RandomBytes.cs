using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Hope.Utils.Random
{
    /// <summary>
    /// Utility class for generating random byte data.
    /// </summary>
    public static class RandomBytes
    {
        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA512 algorithm.
        /// </summary>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA512 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA512Bytes(int length) => GetSHA512Bytes((byte[])null, length);

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA512 algorithm and an additional <see langword="string"/> seed.
        /// </summary>
        /// <param name="seed"> The <see langword="string"/> seed to apply random <see langword="byte"/>[] generation. </param>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA512 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA512Bytes(string seed, int length) => GetSHA512Bytes(seed.GetUTF8Bytes(), length);

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA512 algorithm and an additional <see langword="byte"/>[] seed.
        /// </summary>
        /// <param name="seed"> The <see langword="byte"/>[] seed to apply random <see langword="byte"/>[] generation. </param>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA512 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA512Bytes(byte[] seed, int length) => GetBytes(seed, length, new Sha512Digest());

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA256 algorithm.
        /// </summary>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA256 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA256Bytes(int length) => GetSHA256Bytes((byte[])null, length);

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA256 algorithm and an additional <see langword="string"/> seed.
        /// </summary>
        /// <param name="seed"> The <see langword="string"/> seed to apply random <see langword="byte"/>[] generation. </param>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA256 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA256Bytes(string seed, int length) => GetSHA256Bytes(seed.GetUTF8Bytes(), length);

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using the SHA256 algorithm and an additional <see langword="byte"/>[] seed.
        /// </summary>
        /// <param name="seed"> The <see langword="byte"/>[] seed to apply random <see langword="byte"/>[] generation. </param>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <returns> The random SHA256 <see langword="byte"/>[] data. </returns>
        public static byte[] GetSHA256Bytes(byte[] seed, int length) => GetBytes(seed, length, new Sha256Digest());

        /// <summary>
        /// Generates a series of random <see langword="byte"/>[] data using a specific <see cref="IDigest"/> and an additional <see langword="byte"/>[] seed.
        /// </summary>
        /// <param name="seed"> The <see langword="byte"/>[] seed to apply random <see langword="byte"/>[] generation. </param>
        /// <param name="length"> The length of the <see langword="byte"/>[] data. </param>
        /// <param name="digest"> The <see cref="IDigest"/> object to use to generate the <see langword="byte"/>[] data. </param>
        /// <returns> The random <see langword="byte"/>[] data. </returns>
        private static byte[] GetBytes(byte[] seed, int length, IDigest digest) => (seed == null ? new HopeSecureRandom(digest) : new HopeSecureRandom(digest, seed)).NextBytes(length);
    }
}