using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ProtectorBase : IProtector
{
    private readonly object[] protectorObjects;

    protected ProtectorBase(object baseProtector, params object[] additionalProtectors)
    {
        List<object> protectors = new List<object>();
        protectors.AddRange(additionalProtectors ?? (new object[0]));
        protectors.Add(baseProtector);

        protectorObjects = protectors.ToArray();
    }

    public string Protect(string data) => Protect(data, (string)null);

    public byte[] Protect(byte[] data) => Protect(data, (byte[])null);

    public string Protect(string data, string entropy) => Protect(data, entropy?.GetUTF8Bytes());

    public string Protect(string data, byte[] entropy) => Protect(data?.GetUTF8Bytes(), entropy).GetBase64String();

    public byte[] Protect(byte[] data, string entropy) => InternalProtect(data, GetProtectorHash(entropy));

    public byte[] Protect(byte[] data, byte[] entropy) => Protect(data, entropy?.GetUTF8String());

    public string Unprotect(string encryptedData) => Unprotect(encryptedData, (string)null);

    public byte[] Unprotect(byte[] encryptedData) => Unprotect(encryptedData, (byte[])null);

    public string Unprotect(string encryptedData, string entropy) => Unprotect(encryptedData, entropy?.GetUTF8Bytes());

    public string Unprotect(string encryptedData, byte[] entropy) => Unprotect(encryptedData?.GetBase64Bytes(), entropy).GetUTF8String();

    public byte[] Unprotect(byte[] encryptedData, string entropy) => InternalUnprotect(encryptedData, GetProtectorHash(entropy));

    public byte[] Unprotect(byte[] encryptedData, byte[] entropy) => Unprotect(encryptedData, entropy?.GetUTF8String());

    private byte[] GetProtectorHash(string additionalEntropy = null)
    {
        byte[] hashBytes = new byte[0];

        List<object> protectors = new List<object>(protectorObjects);

        if (additionalEntropy?.Length > 0)
            protectors.Add(additionalEntropy);

        if (protectors.Count == 0)
            return null;

        foreach (var obj in protectors)
        {
            byte[] objBytes = obj.ToString().GetUTF8Bytes();

            int currentLength = hashBytes.Length;
            int objBytesLength = objBytes.Length;

            Array.Resize(ref hashBytes, currentLength + objBytesLength);
            Array.Copy(objBytes, 0, hashBytes, currentLength, objBytesLength);

            hashBytes = hashBytes.GetSHA256Hash();
        }

        return hashBytes.ToArray();
    }

    protected abstract byte[] InternalProtect(byte[] data, byte[] entropy);

    protected abstract byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy);
}