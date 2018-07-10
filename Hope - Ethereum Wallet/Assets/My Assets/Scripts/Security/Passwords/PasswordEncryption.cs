using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Linq;

public static class PasswordEncryption
{
    private const int ITERATIONS = 50000;
    private const int SALT_SIZE = 64;
    private const int HASH_SIZE = 128;

    public static string GetSaltedPasswordHash(string password)
    {
        SecureRandom secureRandom = new SecureRandom();
        byte[] salt = SecureRandom.GetNextBytes(secureRandom, SALT_SIZE);

        return salt.Concat(GetPasswordHash(password, salt)).ToArray().GetBase64String();
    }

    private static byte[] GetPasswordHash(string password, byte[] salt)
    {
        var generator = new Pkcs5S2ParametersGenerator(new Sha512Digest());
        generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, ITERATIONS);

        return ((KeyParameter)generator.GenerateDerivedMacParameters(HASH_SIZE * 8)).GetKey();
    }

    public static bool VerifyPassword(string password, string saltedHash)
    {
        byte[] hashBytes = saltedHash.GetBase64Bytes();
        return VerifyPassword(password, hashBytes.Skip(SALT_SIZE).ToArray(), hashBytes.Take(SALT_SIZE).ToArray());
    }

    private static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        return CheckEquals(hash, GetPasswordHash(password, salt));
    }

    private static bool CheckEquals(byte[] correctHash, byte[] hashToCheck)
    {
        uint diff = (uint)correctHash.Length ^ (uint)hashToCheck.Length;
        for (int i = 0; i < correctHash.Length && i < hashToCheck.Length; i++)
            diff |= (uint)(correctHash[i] ^ hashToCheck[i]);
        return diff == 0;
    }

}