using Org.BouncyCastle.Crypto.Digests;

/// <summary>
/// Class which contains implementations of PBKDF2PasswordHashing using different algorithms.
/// <para> Used to encrypt passwords and verify if they are correct afterwards. </para>
/// </summary>
public static class PasswordEncryption
{
    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the Blake2 hash function (https://en.wikipedia.org/wiki/BLAKE_(hash_function)).
    /// <para> This is considered a very secure hash algorithm. It is arguably the most secure hash algorithm used for password hashing. </para>
    /// </summary>
    public sealed class Blake2 : PBKDF2PasswordHashing<Blake2bDigest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the MD5 hash function (https://en.wikipedia.org/wiki/MD5).
    /// <para> This is not considered a very secure algorithm, while still being much more secure than the <see cref="Fast"/> algorithm. </para>
    /// </summary>
    public sealed class MD5 : PBKDF2PasswordHashing<MD5Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the RIPEMD256 hash function (https://en.wikipedia.org/wiki/RIPEMD).
    /// <para> This is considered not as secure as the SHA family of hash functions, while it is still often used for PGP encryption. </para>
    /// </summary>
    public sealed class RIPEMD256 : PBKDF2PasswordHashing<RipeMD256Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the RIPEMD320 hash function (https://en.wikipedia.org/wiki/RIPEMD).
    /// <para> This is considered not as secure as the SHA family of hash functions, while it is still often used for PGP encryption. </para>
    /// </summary>
    public sealed class RIPEMD320 : PBKDF2PasswordHashing<RipeMD320Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the SHA1 hash function (https://en.wikipedia.org/wiki/SHA-1).
    /// <para> This is not considered a very secure algorithm, for more security consider using <see cref="SHA256"/>/<see cref="SHA512"/> or <see cref="SHA3"/> variants instead. </para>
    /// </summary>
    public sealed class SHA1 : PBKDF2PasswordHashing<Sha1Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the SHA3 hash function (https://en.wikipedia.org/wiki/SHA-3).
    /// <para> This is considered a very secure hash algorithm as it is a subset of the Keccak family of hashing functions. </para>
    /// </summary>
    public sealed class SHA3 : PBKDF2PasswordHashing<Sha3Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the SHA256 hash function (https://en.wikipedia.org/wiki/SHA-2).
    /// <para> This is considered a moderately secure hash algorithm. While it has yet to be broken, <see cref="SHA3"/> offers higher level of security. </para>
    /// </summary>
    public sealed class SHA256 : PBKDF2PasswordHashing<Sha256Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the SHA512 hash function (https://en.wikipedia.org/wiki/SHA-2).
    /// <para> This is considered a moderately secure hash algorithm. While it has yet to be broken, <see cref="SHA3"/> offers higher level of security. </para>
    /// </summary>
    public sealed class SHA512 : PBKDF2PasswordHashing<Sha512Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the SHAKE hash function (https://en.wikipedia.org/wiki/SHA-3).
    /// <para> This is considered a very secure hash algorithm as it is a subset of the Keccak family of hashing functions. </para>
    /// </summary>
    public sealed class Shake : PBKDF2PasswordHashing<ShakeDigest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the chinese SM3 hash function (https://tools.ietf.org/id/draft-oscca-cfrg-sm3-01.html).
    /// <para> This appears to be a secure algorithm, while it is not widely adopted and tested compared to other hash functions. </para>
    /// </summary>
    public sealed class SM3 : PBKDF2PasswordHashing<SM3Digest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the Tiger hash function (https://en.wikipedia.org/wiki/Tiger_(hash_function)).
    /// <para> This is generally considered less secure than <see cref="RIPEMD256"/> and <see cref="RIPEMD320"/> variants. </para>
    /// </summary>
    public sealed class Tiger : PBKDF2PasswordHashing<TigerDigest> { }

    /// <summary>
    /// Class which is used to generate password hashes and verify passwords using the Whirlpool hash function (https://en.wikipedia.org/wiki/Whirlpool_(hash_function)).
    /// <para> This is a very niche algorithm, and has not seen wide use while being fairly secure. </para>
    /// </summary>
    public sealed class Whirlpool : PBKDF2PasswordHashing<WhirlpoolDigest> { }
}