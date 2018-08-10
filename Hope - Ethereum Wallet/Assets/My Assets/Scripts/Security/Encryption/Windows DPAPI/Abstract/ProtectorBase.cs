using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ProtectorBase : IProtector
{
    private readonly object[] protectorObjects;

    protected ProtectorBase(params object[] protectorObjects)
    {
        this.protectorObjects = protectorObjects ?? (new object[0]);
    }

    public void Protect(string data) => Protect(data.GetUTF8Bytes());

    public void Protect(byte[] data) => Protect(data, protectorObjects.Length > 0 ? GetProtectorHash() : null);

    public void Protect(string data, string entropy) => Protect(data, entropy.GetUTF8Bytes());

    public void Protect(string data, byte[] entropy)
    {
        throw new NotImplementedException();
    }

    public void Protect(byte[] data, string entropy)
    {
        throw new NotImplementedException();
    }

    public void Protect(byte[] data, byte[] entropy)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(string encryptedData)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(byte[] encryptedData)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(string encryptedData, string entropy)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(string encryptedData, byte[] entropy)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(byte[] data, string entropy)
    {
        throw new NotImplementedException();
    }

    public void Unprotect(byte[] data, byte[] entropy)
    {
        throw new NotImplementedException();
    }

    private byte[] GetProtectorHash(byte[] additionalEntropy = null)
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

    protected abstract void InternalProtect(byte[] data, byte[] entropy);

    protected abstract void InternalUnprotect(byte[] data, byte[] entropy);
}