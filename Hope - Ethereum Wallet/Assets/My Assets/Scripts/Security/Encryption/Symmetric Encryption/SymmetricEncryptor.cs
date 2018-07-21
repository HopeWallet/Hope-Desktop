using Org.BouncyCastle.Security;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

/// <summary>
/// Generic class which implements usability to symmetric encryption algorithms derived from SymmetricAlgorithm.
/// </summary>
/// <typeparam name="T"> The type of the class inheriting the SymmetricEncryptor. </typeparam>
/// <typeparam name="A"> The derived type of the SymmetricAlgorithm. </typeparam>
public abstract class SymmetricEncryptor<T, A> where T : SymmetricEncryptor<T, A>, new() where A : SymmetricAlgorithm, new()
{

    private const int ITERATIONS = 1000;

    private static readonly SecureRandom Random = new SecureRandom();
    private static readonly T Encryptor = new T();

    /// <summary>
    /// The key size and block size to use during the encryption.
    /// </summary>
    public abstract int KeySize { get; }

    /// <summary>
    /// The salt and iv byte size to use during the encryption.
    /// </summary>
    public abstract int SaltIvByteSize { get; }

    /// <summary>
    /// Encrypts <see langword="string"/> text using the <see cref="SymmetricAlgorithm"/> of the derived class.
    /// </summary>
    /// <param name="text"> The <see langword="string"/> text to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="string"/> text. </returns>
    public static string Encrypt(string text, string entropy) => Encrypt(text, entropy, ITERATIONS);

    public static string Decrypt(string encryptedText, string entropy) => Decrypt(encryptedText, entropy, ITERATIONS);

    public static string Encrypt(string text, string entropy, int iterations) => Encryptor.InternalEncrypt(text.GetUTF8Bytes(), entropy, iterations).GetBase64String();

    public static string Decrypt(string encryptedText, string entropy, int iterations) => Encryptor.InternalDecrypt(encryptedText.GetBase64Bytes(), entropy, iterations).GetUTF8String();

    public static byte[] Encrypt(byte[] data, string entropy) => Encryptor.InternalEncrypt(data, entropy, ITERATIONS);

    public static byte[] Decrypt(byte[] encryptedData, string entropy) => Encryptor.InternalDecrypt(encryptedData, entropy, ITERATIONS);

    public static byte[] Encrypt(byte[] data, string entropy, int iterations) => Encryptor.InternalEncrypt(data, entropy, iterations);

    public static byte[] Decrypt(byte[] encryptedData, string entropy, int iterations) => Encryptor.InternalDecrypt(encryptedData, entropy, iterations);

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data using the <see cref="SymmetricAlgorithm"/> provided.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The entropy to use to encrypt the data. </param>
    /// <param name="iterations"> The number of iterations to apply to <see cref="Rfc2898DeriveBytes"/> to derive the encryption key. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    private byte[] InternalEncrypt(byte[] data, string entropy, int iterations)
    {
        byte[] saltStringBytes = SecureRandom.GetNextBytes(Random, SaltIvByteSize);
        byte[] ivStringBytes = SecureRandom.GetNextBytes(Random, SaltIvByteSize);

        using (var password = new Rfc2898DeriveBytes(entropy, saltStringBytes, iterations))
        using (var symmetricKey = new A())
        {
            symmetricKey.BlockSize = KeySize;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using (var encryptor = symmetricKey.CreateEncryptor(password.GetBytes(KeySize / 8), ivStringBytes))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                symmetricKey.Clear();

                return saltStringBytes.Concat(ivStringBytes).ToArray().Concat(memoryStream.ToArray()).ToArray();
            }
        }
    }

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data using the <see cref="SymmetricAlgorithm"/> provided.
    /// </summary>
    /// <param name="encryptedData"> The <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The entropy to use to decrypt the data. </param>
    /// <param name="iterations"> The number of iterations to apply to <see cref="Rfc2898DeriveBytes"/> to derive the decryption key. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    private byte[] InternalDecrypt(byte[] encryptedData, string entropy, int iterations)
    {
        byte[] saltStringBytes = encryptedData.Take(KeySize / 8).ToArray();
        byte[] ivStringBytes = encryptedData.Skip(KeySize / 8).Take(KeySize / 8).ToArray();
        byte[] cipherTextBytes = encryptedData.Skip((KeySize / 8) * 2).Take(encryptedData.Length - ((KeySize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(entropy, saltStringBytes, iterations))
        using (var symmetricKey = new A())
        {
            symmetricKey.BlockSize = KeySize;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using (var decryptor = symmetricKey.CreateDecryptor(password.GetBytes(KeySize / 8), ivStringBytes))
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                symmetricKey.Clear();

                return plainTextBytes;
            }
        }
    }
}