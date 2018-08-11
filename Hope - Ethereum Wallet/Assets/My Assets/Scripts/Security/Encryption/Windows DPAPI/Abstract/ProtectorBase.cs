using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ProtectorBase : IProtector
{
    private readonly List<byte[]> protectorByteData = new List<byte[]>();
    private readonly List<SecureObject> protectorSecureObjects = new List<SecureObject>();

    protected ProtectorBase(params object[] protectors)
    {
        protectorByteData.AddRange(protectors.Where(protector => !protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                             .Select(protector => protector.ToString().GetUTF8Bytes()));

        protectorSecureObjects.AddRange(protectors.Where(protector => protector.GetType().IsSubclassOf(typeof(SecureObject)))
                                                  .Select(protector => protector as SecureObject));
    }

    [SecureCaller]
    public string Protect(string data) => Protect(data, (string)null);

    [SecureCaller]
    public byte[] Protect(byte[] data) => Protect(data, (byte[])null);

    [SecureCaller]
    public string Protect(string data, string entropy) => Protect(data, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public string Protect(string data, byte[] entropy) => Protect(data?.GetUTF8Bytes(), entropy).GetBase64String();

    [SecureCaller]
    public byte[] Protect(byte[] data, string entropy) => Protect(data, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public byte[] Protect(byte[] data, byte[] entropy) => InternalProtect(data, GetProtectorHash(entropy));

    [SecureCaller]
    public string Unprotect(string encryptedData) => Unprotect(encryptedData, (string)null);

    [SecureCaller]
    public byte[] Unprotect(byte[] encryptedData) => Unprotect(encryptedData, (byte[])null);

    [SecureCaller]
    public string Unprotect(string encryptedData, string entropy) => Unprotect(encryptedData, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public string Unprotect(string encryptedData, byte[] entropy) => Unprotect(encryptedData?.GetBase64Bytes(), entropy).GetUTF8String();

    [SecureCaller]
    public byte[] Unprotect(byte[] encryptedData, string entropy) => Unprotect(encryptedData, entropy?.GetUTF8Bytes());

    [SecureCaller]
    public byte[] Unprotect(byte[] encryptedData, byte[] entropy) => InternalUnprotect(encryptedData, GetProtectorHash(entropy));

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    private byte[] GetProtectorHash(byte[] additionalEntropy = null)
    {
        byte[] hashBytes = new byte[0];

        foreach (var objBytes in GetProtectorByteData(additionalEntropy))
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
    private List<byte[]> GetProtectorByteData(byte[] additionalEntropy)
    {
        List<byte[]> protectors = new List<byte[]>();
        protectors.AddRange(protectorByteData);

        foreach (var secureObj in protectorSecureObjects)
        {
            string hashString = secureObj.GetHashCode().ToString();
            string objString = secureObj.ToString();
            protectors.Add((hashString + objString).GetUTF8Bytes());
        }

        if (additionalEntropy?.Length > 0)
            protectors.Add(additionalEntropy);
        return protectors;
    }

    protected abstract byte[] InternalProtect(byte[] data, byte[] entropy);

    protected abstract byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy);
}