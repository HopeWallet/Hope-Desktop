using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class which encrypts data using many different objects as the entropy for the encryption.
/// </summary>
public abstract class AdvancedEntropyEncryptor : SecureObject, IDisposable
{
    private readonly List<byte[]> encryptorData = new List<byte[]>();
    private readonly List<SecureObject> encryptorSecureObjects = new List<SecureObject>();

    /// <summary>
    /// Whether this <see cref="AdvancedEntropyEncryptor"/> has been disposed of yet.
    /// </summary>
    public bool Disposed { get; protected set; }

    /// <summary>
    /// Initializes the <see cref="AdvancedEntropyEncryptor"/> with the array of object data to use to formulate our entropy.
    /// </summary>
    /// <param name="encryptors"> Objects to use to formulate our encryption entropy. </param>
    protected AdvancedEntropyEncryptor(params object[] encryptors)
    {
        encryptorData.AddRange(encryptors.Where(protector => !protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                         .Select(protector => protector is byte[] ? protector as byte[] : protector.ToString().GetUTF8Bytes()));

        encryptorSecureObjects.AddRange(encryptors.Where(protector => protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                                  .Select(protector => protector as SecureObject));
    }

    /// <summary>
    /// Disposes of the advanced entropy encryptors this <see cref="AdvancedEntropyEncryptor"/> class holds and cleans all garbage.
    /// </summary>
    public virtual void Dispose()
    {
        if (!Disposed)
        {
            encryptorData.ForEach(bytes => bytes?.ClearBytes());
            Disposed = true;
        }

        GC.SuppressFinalize(this);
        GC.Collect();
    }

    /// <summary>
    /// Encrypts <see langword="string"/> data with only the entropy from the object creation.
    /// </summary>
    /// <param name="data"> The <see langword="string"/> data to encrypt. </param>
    /// <returns> The encrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Encrypt(string data) => Encrypt(data, (string)null);

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data with only the entropy from the object creation.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Encrypt(byte[] data) => Encrypt(data, (byte[])null);

    /// <summary>
    /// Encrypts <see langword="string"/> data with an additional entropy parameter.
    /// </summary>
    /// <param name="data"> The <see langword="string"/> data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Encrypt(string data, string entropy) => Encrypt(data, entropy?.GetUTF8Bytes());

    /// <summary>
    /// Encrypts <see langword="string"/> data with an additional entropy parameter.
    /// </summary>
    /// <param name="data"> The <see langword="string"/> data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public string Encrypt(string data, byte[] entropy) => Encrypt(data?.GetUTF8Bytes(), entropy).GetBase64String();

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data with an additional entropy parameter.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Encrypt(byte[] data, string entropy) => Encrypt(data, entropy?.GetUTF8Bytes());

    /// <summary>
    /// Decrypts <see langword="string"/> data with only the entropy from the object creation.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="string"/> data to decrypt. </param>
    /// <returns> The decrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Decrypt(string encryptedData) => Decrypt(encryptedData, (string)null);

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data with only the entropy from the object creation.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData) => Decrypt(encryptedData, (byte[])null);

    /// <summary>
    /// Decrypts <see langword="string"/> data using the additional entropy parameter.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="string"/> data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Decrypt(string encryptedData, string entropy) => Decrypt(encryptedData, entropy?.GetUTF8Bytes());

    /// <summary>
    /// Decrypts <see langword="string"/> data using the additional entropy parameter.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="string"/> data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Decrypt(string encryptedData, byte[] entropy) => Decrypt(encryptedData?.GetBase64Bytes(), entropy).GetUTF8String();

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data using the additional entropy parameter.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData, string entropy) => Decrypt(encryptedData, entropy?.GetUTF8Bytes());

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data with an additional entropy parameter.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public abstract byte[] Encrypt(byte[] data, byte[] entropy);

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data using the additional entropy parameter.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public abstract byte[] Decrypt(byte[] encryptedData, byte[] entropy);

    /// <summary>
    /// Gets the hash of the entropy created from all of our encryptor objects.
    /// </summary>
    /// <param name="additionalEntropy"> The additional <see langword="byte"/>[] data to add to our entropy formulation. </param>
    /// <returns> The <see langword="byte"/>[] data which can be used as our encryption entropy. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    protected byte[] GetAdvancedEntropyHash(byte[] additionalEntropy)
    {
        byte[] hashBytes = new byte[0];

        foreach (var objBytes in GetEncryptionByteData(additionalEntropy))
        {
            int currentLength = hashBytes.Length;
            int objBytesLength = objBytes.Length;

            Array.Resize(ref hashBytes, currentLength + objBytesLength);
            Array.Copy(objBytes, 0, hashBytes, currentLength, objBytesLength);

            hashBytes = hashBytes.GetSHA256Hash();
        }

        return hashBytes;
    }

    /// <summary>
    /// Gets the full <see langword="byte"/>[] data to use with our encryption.
    /// </summary>
    /// <param name="additionalEntropy"> The additional <see langword="byte"/>[] data to add to our entropy formulation. </param>
    /// <returns> The list of <see langword="byte"/>[] data to formulate our entropy. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(List<byte[]>))]
    private List<byte[]> GetEncryptionByteData(byte[] additionalEntropy)
    {
        List<byte[]> protectors = new List<byte[]>();
        protectors.AddRange(encryptorData);

        foreach (var secureObj in encryptorSecureObjects)
        {
            string hashString = secureObj.GetHashCode().ToString();
            string objString = secureObj.ToString();
            protectors.Add((hashString + objString).GetUTF8Bytes());
        }

        if (additionalEntropy?.Length > 0)
            protectors.Add(additionalEntropy);

        return protectors;
    }
}