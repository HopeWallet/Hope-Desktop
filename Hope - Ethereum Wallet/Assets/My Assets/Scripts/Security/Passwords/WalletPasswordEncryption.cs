using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Security.Cryptography;

public static class WalletPasswordEncryption
{

    private const int ITERATIONS = 100000;
    private const int SALT_SIZE = 64;
    private const int HASH_SIZE = 128;

    public static string GetSaltedPasswordHash(string password)
    {
        byte[] salt = new byte[SALT_SIZE];
        
        RandomNumberGenerator.Create().GetBytes(salt);

        return Convert.ToBase64String(salt.Concat(GetPasswordHash(password, salt)).ToArray());
    }

    private static byte[] GetPasswordHash(string password, byte[] salt)
    {
        var generator = new Pkcs5S2ParametersGenerator(new Sha512Digest());
        generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, ITERATIONS);

        return ((KeyParameter)generator.GenerateDerivedMacParameters(HASH_SIZE * 8)).GetKey();
    }

    public static bool VerifyPassword(string password, string saltedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(saltedHash);
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