using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;

namespace Hope.Security.HashGeneration
{
    /// <summary>
    /// Class which contains a series of methods for generating hashes for string data based on different hash algorithms.
    /// </summary>
    public static class HashGenerator
    {
        public static string Blake2_160(this string input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(160));

        public static byte[] Blake2_160(this byte[] input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(160));

        public static string Blake2_256(this string input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(256));

        public static byte[] Blake2_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(256));

        public static string Blake2_384(this string input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(384));

        public static byte[] Blake2_384(this byte[] input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(384));

        public static string Blake2_512(this string input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(512));

        public static byte[] Blake2_512(this byte[] input) => HashGenerationHelpers.GetHash(input, new Blake2bDigest(512));

        public static string Keccak_128(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(128));

        public static byte[] Keccak_128(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(128));

        public static string Keccak_224(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(224));

        public static byte[] Keccak_224(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(224));

        public static string Keccak_256(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(256));

        public static byte[] Keccak_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(256));

        public static string Keccak_288(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(288));

        public static byte[] Keccak_288(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(288));

        public static string Keccak_384(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(384));

        public static byte[] Keccak_384(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(384));

        public static string Keccak_512(this string input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(512));

        public static byte[] Keccak_512(this byte[] input) => HashGenerationHelpers.GetHash(input, new KeccakDigest(512));

        public static string MD2(this string input) => HashGenerationHelpers.GetHash(input, new MD2Digest());

        public static byte[] MD2(this byte[] input) => HashGenerationHelpers.GetHash(input, new MD2Digest());

        public static string MD4(this string input) => HashGenerationHelpers.GetHash(input, new MD4Digest());

        public static byte[] MD4(this byte[] input) => HashGenerationHelpers.GetHash(input, new MD4Digest());

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.MD5"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.MD5"/> hashed <see langword="string"/>. </returns>
        public static string MD5(this string input) => HashGenerationHelpers.GetHash<MD5>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.MD5"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.MD5"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] MD5(this byte[] input) => HashGenerationHelpers.GetHash<MD5>(input);

        public static string RIPEMD_128(this string input) => HashGenerationHelpers.GetHash(input, new RipeMD128Digest());

        public static byte[] RIPEMD_128(this byte[] input) => HashGenerationHelpers.GetHash(input, new RipeMD128Digest());

        /// <summary>
        /// Gets the <see cref="RIPEMD160"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="RIPEMD160"/> hashed <see langword="string"/>. </returns>
        public static string RIPEMD_160(this string input) => HashGenerationHelpers.GetHash<RIPEMD160>(input);

        /// <summary>
        /// Gets the <see cref="RIPEMD160"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="RIPEMD160"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] RIPEMD_160(this byte[] input) => HashGenerationHelpers.GetHash<RIPEMD160>(input);

        public static string RIPEMD_256(this string input) => HashGenerationHelpers.GetHash(input, new RipeMD256Digest());

        public static byte[] RIPEMD_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new RipeMD256Digest());

        public static string RIPEMD_320(this string input) => HashGenerationHelpers.GetHash(input, new RipeMD320Digest());

        public static byte[] RIPEMD_320(this byte[] input) => HashGenerationHelpers.GetHash(input, new RipeMD320Digest());

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.SHA1"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.SHA1"/> hashed <see langword="string"/>. </returns>
        public static string SHA1(this string input) => HashGenerationHelpers.GetHash<SHA1>(input);

        /// <summary>
        /// Gets the <see cref="System.Security.Cryptography.SHA1"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="System.Security.Cryptography.SHA1"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] SHA1(this byte[] input) => HashGenerationHelpers.GetHash<SHA1>(input);

        public static string SHA2_224(this string input) => HashGenerationHelpers.GetHash(input, new Sha224Digest());

        public static byte[] SHA2_224(this byte[] input) => HashGenerationHelpers.GetHash(input, new Sha224Digest());

        /// <summary>
        /// Gets the <see cref="SHA256"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA256"/> hashed <see langword="string"/>. </returns>
        public static string SHA2_256(this string input) => HashGenerationHelpers.GetHash<SHA256>(input);

        /// <summary>
        /// Gets the <see cref="SHA256"/> hash of a <see langword="byte"[]/> input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA256"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] SHA2_256(this byte[] input) => HashGenerationHelpers.GetHash<SHA256>(input);

        /// <summary>
        /// Gets the <see cref="SHA384"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA384"/> hashed <see langword="string"/>. </returns>
        public static string SHA2_384(this string input) => HashGenerationHelpers.GetHash<SHA384>(input);

        /// <summary>
        /// Gets the <see cref="SHA384"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA384"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] SHA2_384(this byte[] input) => HashGenerationHelpers.GetHash<SHA384>(input);

        /// <summary>
        /// Gets the <see cref="SHA512"/> hash of a <see langword="string"/> input.
        /// </summary>
        /// <param name="input"> The <see langword="string"/> to get the hash for. </param>
        /// <returns> The <see cref="SHA512"/> hashed <see langword="string"/>. </returns>
        public static string SHA2_512(this string input) => HashGenerationHelpers.GetHash<SHA512>(input);

        /// <summary>
        /// Gets the <see cref="SHA512"/> hash of a <see langword="byte"/>[] input.
        /// </summary>
        /// <param name="input"> The <see langword="byte"/>[] data to get the hash for. </param>
        /// <returns> The <see cref="SHA512"/> hashed <see langword="byte"/>[] data. </returns>
        public static byte[] SHA2_512(this byte[] input) => HashGenerationHelpers.GetHash<SHA512>(input);

        public static string SHA3_224(this string input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(224));

        public static byte[] SHA3_224(this byte[] input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(224));

        public static string SHA3_256(this string input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(256));

        public static byte[] SHA3_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(256));

        public static string SHA3_384(this string input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(384));

        public static byte[] SHA3_384(this byte[] input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(384));

        public static string SHA3_512(this string input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(512));

        public static byte[] SHA3_512(this byte[] input) => HashGenerationHelpers.GetHash(input, new Sha3Digest(512));

        public static string Shake_128(this string input) => HashGenerationHelpers.GetHash(input, new ShakeDigest(128));

        public static byte[] Shake_128(this byte[] input) => HashGenerationHelpers.GetHash(input, new ShakeDigest(128));

        public static string Shake_256(this string input) => HashGenerationHelpers.GetHash(input, new ShakeDigest(256));

        public static byte[] Shake_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new ShakeDigest(256));

        public static string Skein_256(this string input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(256, 256));

        public static byte[] Skein_256(this byte[] input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(256, 256));

        public static string Skein_512(this string input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(512, 512));

        public static byte[] Skein_512(this byte[] input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(512, 512));

        public static string Skein_1024(this string input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(1024, 1024));

        public static byte[] Skein_1024(this byte[] input) => HashGenerationHelpers.GetHash(input, new SkeinDigest(1024, 1024));

        public static string SM3(this string input) => HashGenerationHelpers.GetHash(input, new SM3Digest());

        public static byte[] SM3(this byte[] input) => HashGenerationHelpers.GetHash(input, new SM3Digest());

        public static string Whirlpool(this string input) => HashGenerationHelpers.GetHash(input, new WhirlpoolDigest());

        public static byte[] Whirlpool(this byte[] input) => HashGenerationHelpers.GetHash(input, new WhirlpoolDigest());
    }
}