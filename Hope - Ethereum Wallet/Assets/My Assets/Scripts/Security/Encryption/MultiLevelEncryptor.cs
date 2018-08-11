using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class MultiLevelEncryptor : SecureObject
{
    private readonly List<byte[]> encryptorData = new List<byte[]>();
    private readonly List<SecureObject> encryptorSecureObjects = new List<SecureObject>();

    protected MultiLevelEncryptor(params object[] encryptors)
    {
        encryptorData.AddRange(encryptors.Where(protector => !protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                         .Select(protector => protector.ToString().GetUTF8Bytes()));

        encryptorSecureObjects.AddRange(encryptors.Where(protector => protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                                  .Select(protector => protector as SecureObject));
    }

    [SecureCaller]
    public string Encrypt(string data) => Encrypt(data, (string)null);

    [SecureCaller]
    public byte[] Encrypt(byte[] data) => Encrypt(data, (byte[])null);

    [SecureCaller]
    public string Encrypt(string data, string entropy) => Encrypt(data, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public string Encrypt(string data, byte[] entropy) => Encrypt(data?.GetUTF8Bytes(), entropy).GetBase64String();

    [SecureCaller]
    public byte[] Encrypt(byte[] data, string entropy) => Encrypt(data, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public string Decrypt(string encryptedData) => Decrypt(encryptedData, (string)null);

    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData) => Decrypt(encryptedData, (byte[])null);

    [SecureCaller]
    public string Decrypt(string encryptedData, string entropy) => Decrypt(encryptedData, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public string Decrypt(string encryptedData, byte[] entropy) => Decrypt(encryptedData?.GetBase64Bytes(), entropy).GetUTF8String();

    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData, string entropy) => Decrypt(encryptedData, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public abstract byte[] Encrypt(byte[] data, byte[] entropy);

    [SecureCaller]
    public abstract byte[] Decrypt(byte[] encryptedData, byte[] entropy);

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    protected byte[] GetMultiLevelEncryptionHash(byte[] additionalEntropy = null)
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