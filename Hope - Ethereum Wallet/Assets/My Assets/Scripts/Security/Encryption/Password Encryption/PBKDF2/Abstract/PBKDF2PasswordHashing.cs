using Hope.Utils.Random;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using System.Linq;

public abstract class PBKDF2PasswordHashing<THashAlgorithm> where THashAlgorithm : IDigest, new()
{
    private const int ITERATIONS = 50000;
    private const int SALT_SIZE = 64;
    private const int HASH_SIZE = 128;

    private const int MIN_ITERATIONS = 100;
    private const int MIN_SALT_SIZE = 8;
    private const int MIN_HASH_SIZE = 16;

    public static string GetSaltedPasswordHash(string password)
        => GetSaltedPasswordHash(password, ITERATIONS);

    public static string GetSaltedPasswordHash(string password, int iterations)
        => GetSaltedPasswordHash(password, iterations, SALT_SIZE);

    public static string GetSaltedPasswordHash(string password, int iterations, int saltSize)
        => GetSaltedPasswordHash(password, iterations, saltSize, HASH_SIZE);

    public static string GetSaltedPasswordHash(string password, int iterations, int saltSize, int hashSize)
        => GetSaltedPasswordHash(password.ToCharArray(), iterations, saltSize, hashSize).GetBase64String();

    public static byte[] GetSaltedPasswordHash(char[] password) => GetSaltedPasswordHash(password, ITERATIONS);

    public static byte[] GetSaltedPasswordHash(char[] password, int iterations)
        => GetSaltedPasswordHash(password, iterations, SALT_SIZE);

    public static byte[] GetSaltedPasswordHash(char[] password, int iterations, int saltSize)
        => GetSaltedPasswordHash(password, iterations, saltSize, HASH_SIZE);

    public static byte[] GetSaltedPasswordHash(char[] password, int iterations, int saltSize, int hashSize)
        => InternalGetSaltedPasswordHash(password, iterations, saltSize, hashSize);

    public static bool VerifyPassword(string password, string saltedHash)
        => VerifyPassword(password, saltedHash, ITERATIONS);

    public static bool VerifyPassword(string password, string saltedHash, int iterations)
        => VerifyPassword(password, saltedHash, iterations, SALT_SIZE);

    public static bool VerifyPassword(string password, string saltedHash, int iterations, int saltSize)
        => VerifyPassword(password, saltedHash, iterations, saltSize, HASH_SIZE);

    public static bool VerifyPassword(string password, string saltedHash, int iterations, int saltSize, int hashSize)
        => VerifyPassword(password.ToCharArray(), saltedHash, iterations, saltSize, hashSize);

    public static bool VerifyPassword(char[] password, string saltedHash)
    => VerifyPassword(password, saltedHash, ITERATIONS);

    public static bool VerifyPassword(char[] password, string saltedHash, int iterations)
        => VerifyPassword(password, saltedHash, iterations, SALT_SIZE);

    public static bool VerifyPassword(char[] password, string saltedHash, int iterations, int saltSize)
        => VerifyPassword(password, saltedHash, iterations, saltSize, HASH_SIZE);

    public static bool VerifyPassword(char[] password, string saltedHash, int iterations, int saltSize, int hashSize)
        => InternalVerifyPassword(password, saltedHash.GetBase64Bytes(), iterations, saltSize, hashSize);

    private static bool InternalVerifyPassword(char[] password, byte[] saltedHash, int iterations, int saltSize, int hashSize)
    {
        saltSize = saltSize <= MIN_SALT_SIZE ? MIN_SALT_SIZE : saltSize;
        return CheckEquals(password, saltedHash.Skip(saltSize).ToArray(), saltedHash.Take(saltSize).ToArray(), iterations, hashSize);
    }

    private static byte[] InternalGetSaltedPasswordHash(char[] password, int iterations, int saltSize, int hashSize)
    {
        byte[] salt = new HopeSecureRandom(new THashAlgorithm()).NextBytes(saltSize <= MIN_SALT_SIZE ? MIN_SALT_SIZE : saltSize);
        return salt.Concat(GeneratePasswordHash(password, salt, iterations, hashSize)).ToArray();
    }

    private static byte[] GeneratePasswordHash(char[] password, byte[] salt, int iterations, int hashSize)
    {
        var generator = new Pkcs5S2ParametersGenerator(new THashAlgorithm());
        generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password), salt, iterations <= MIN_ITERATIONS ? MIN_ITERATIONS : iterations);

        return ((KeyParameter)generator.GenerateDerivedMacParameters((hashSize <= MIN_HASH_SIZE ? MIN_HASH_SIZE : hashSize) * 8)).GetKey();
    }

    private static bool CheckEquals(char[] password, byte[] hash, byte[] salt, int iterations, int hashSize)
    {
        byte[] correctHash = hash;
        byte[] hashToCheck = GeneratePasswordHash(password, salt, iterations, hashSize);

        uint diff = (uint)correctHash.Length ^ (uint)hashToCheck.Length;

        for (int i = 0; i < correctHash.Length && i < hashToCheck.Length; i++)
            diff |= (uint)(correctHash[i] ^ hashToCheck[i]);

        return diff == 0;
    }
}