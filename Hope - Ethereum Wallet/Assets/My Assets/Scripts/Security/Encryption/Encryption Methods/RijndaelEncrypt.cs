using Org.BouncyCastle.Security;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public static class RijndaelEncrypt
{

    private static readonly SecureRandom Random = new SecureRandom();

    private const int KEYSIZE = 256;
    private const int ITERATIONS = 1000;

    private const int SALT_IV_BYTE_SIZE = 32;

    public static string Encrypt(string text, string entropy) => Encrypt(text, entropy, ITERATIONS);

    public static string Decrypt(string encryptedText, string entropy) => Decrypt(encryptedText, entropy, ITERATIONS);

    public static string Encrypt(string text, string entropy, int iterations) => Encrypt(text.GetUTF8Bytes(), entropy, iterations).GetBase64String();

    public static string Decrypt(string encryptedText, string entropy, int iterations) => Decrypt(encryptedText.GetBase64Bytes(), entropy, iterations).GetUTF8String();

    public static byte[] Encrypt(byte[] data, string entropy) => Encrypt(data, entropy, ITERATIONS);

    public static byte[] Decrypt(byte[] encryptedData, string entropy) => Decrypt(encryptedData, entropy, ITERATIONS);

    public static byte[] Encrypt(byte[] data, string entropy, int iterations)
    {
        byte[] saltStringBytes = SecureRandom.GetNextBytes(Random, SALT_IV_BYTE_SIZE);
        byte[] ivStringBytes = SecureRandom.GetNextBytes(Random, SALT_IV_BYTE_SIZE);

        using (var password = new Rfc2898DeriveBytes(entropy, saltStringBytes, iterations))
        using (var symmetricKey = new RijndaelManaged())
        {
            symmetricKey.BlockSize = KEYSIZE;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using (var encryptor = symmetricKey.CreateEncryptor(password.GetBytes(KEYSIZE / 8), ivStringBytes))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return saltStringBytes.Concat(ivStringBytes).ToArray().Concat(memoryStream.ToArray()).ToArray();
            }
        }
    }

    public static byte[] Decrypt(byte[] encryptedData, string entropy, int iterations)
    {
        byte[] saltStringBytes = encryptedData.Take(KEYSIZE / 8).ToArray();
        byte[] ivStringBytes = encryptedData.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
        byte[] cipherTextBytes = encryptedData.Skip((KEYSIZE / 8) * 2).Take(encryptedData.Length - ((KEYSIZE / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(entropy, saltStringBytes, iterations))
        using (var symmetricKey = new RijndaelManaged())
        {
            symmetricKey.BlockSize = KEYSIZE;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using (var decryptor = symmetricKey.CreateDecryptor(password.GetBytes(KEYSIZE / 8), ivStringBytes))
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                return plainTextBytes;
            }
        }
    }

}