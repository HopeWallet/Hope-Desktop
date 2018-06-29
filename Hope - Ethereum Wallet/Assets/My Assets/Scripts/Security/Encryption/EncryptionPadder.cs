using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Class which pads the string/byte data to the required length (16) of certain encryption algorithms.
/// </summary>
public static class EncryptionPadder
{

    private static readonly byte[] KEY;
    private static readonly byte[] IV;

    /// <summary>
    /// Initializes the EncryptionPadder with a key and iv for the encryption.
    /// </summary>
    static EncryptionPadder()
    {
        SecureRandom secureRandom = new SecureRandom();
        KEY = SecureRandom.GetNextBytes(secureRandom, 16);
        IV = SecureRandom.GetNextBytes(secureRandom, 16);

        ProtectPaddingSeed();
    }

    /// <summary>
    /// Pads the byte data to the nearest multiple of 16.
    /// </summary>
    /// <param name="data"> The data to pad. </param>
    /// <returns> The padded byte data. </returns>
    public static byte[] PadData(this byte[] data) => Encrypt(Convert.ToBase64String(data));

    /// <summary>
    /// Unpads the byte data from the nearest multiple of 16 and restores its original state.
    /// </summary>
    /// <param name="data"> The data to unpad. </param>
    /// <returns> The unpadded data. </returns>
    public static byte[] UnpadData(this byte[] data) => Decrypt(Convert.ToBase64String(data));

    /// <summary>
    /// Pads the data by encrypting it using the AES encryption algorithm which encrypts data to a multiple of 16.
    /// </summary>
    /// <param name="data"> The data to encrypt. </param>
    /// <returns> The encrypted and padded byte data. </returns>
    public static byte[] Encrypt(this string data)
    {
        UnprotectPaddingSeed();

        byte[] encryptedData;
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(KEY, IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                    sw.Write(data);

                encryptedData = ms.ToArray();
            }
        }

        ProtectPaddingSeed();

        return encryptedData;
    }

    /// <summary>
    /// Unpads the data by decrypting it using the AES encryption algorithm.
    /// </summary>
    /// <param name="data"> The data to decrypt. </param>
    /// <returns> The decrypted and unpadded byte data. </returns>
    public static byte[] Decrypt(this string data)
    {
        UnprotectPaddingSeed();

        byte[] decryptedData;
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(KEY, IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sw = new StreamReader(cs))
                decryptedData = Convert.FromBase64String(sw.ReadToEnd());
        }

        ProtectPaddingSeed();

        return decryptedData;
    }

    /// <summary>
    /// Calls ProtectedMemory.Protect on the key and iv of the AES algorithm.
    /// </summary>
    private static void ProtectPaddingSeed()
    {
        ProtectedMemory.Protect(KEY, MemoryProtectionScope.SameProcess);
        ProtectedMemory.Protect(IV, MemoryProtectionScope.SameProcess);
    }

    /// <summary>
    /// Calls ProtectedMemory.Unprotect on the key and iv of the AES algorithm.
    /// </summary>
    private static void UnprotectPaddingSeed()
    {
        ProtectedMemory.Unprotect(KEY, MemoryProtectionScope.SameProcess);
        ProtectedMemory.Unprotect(IV, MemoryProtectionScope.SameProcess);
    }

}