using System;

public abstract class ProtectorBase : IProtector
{
    private readonly object[] protectorObjects;

    protected ProtectorBase(params object[] protectorObjects)
    {
        this.protectorObjects = protectorObjects;
    }

    public void Protect(string data) => Protect(data.GetUTF8Bytes());

    public void Protect(byte[] data) => Protect(data, protectorObjects.Length > 0 ? GetProtectorHash() : null);

    public void Protect(string data, string entropy)
    {

    }

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

    private byte[] GetProtectorHash()
    {
        return null;
    }

    protected abstract void InternalProtect(byte[] data, byte[] entropy);

    protected abstract void InternalUnprotect(byte[] data, byte[] entropy);
}