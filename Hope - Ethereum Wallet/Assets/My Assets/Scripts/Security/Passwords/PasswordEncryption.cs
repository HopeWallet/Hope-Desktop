﻿using Hope.Security.HashGeneration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class PasswordEncryption
{
    // TODO:
    // Implement wallet password encryption
    // Save the salted hash to player prefs
    // Only check the password when the wallet is loaded

    public static readonly string PWD_PREF_NAME = HashGenerator.GetSHA512Hash("password");

    private const int ITERATIONS = 50000;
    private const int SALT_SIZE = 64;
    private const int HASH_SIZE = 128;

    public static async Task GetSaltedPasswordHashAsync(string password, Action<string> onHashReceived)
    {
        string saltedHash = await Task.Run(() => GetSaltedPasswordHash(password)).ConfigureAwait(false);
        onHashReceived?.Invoke(saltedHash);
    }

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

    public static async Task VerifyPasswordAsync(string password, string saltedHash, Action<bool> onResultReceived)
    {
        bool result = await Task.Run(() => VerifyPassword(password, saltedHash)).ConfigureAwait(false);
        onResultReceived?.Invoke(result);
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