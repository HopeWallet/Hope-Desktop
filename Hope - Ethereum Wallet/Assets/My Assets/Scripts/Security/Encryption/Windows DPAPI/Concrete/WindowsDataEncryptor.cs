using System.Security.Cryptography;

public sealed class WindowsDataEncryptor : WindowsEncryptor
{
    public WindowsDataEncryptor(params object[] encryptors) : base(encryptors)
    {
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalEncrypt(byte[] data, byte[] entropy)
    {
        return ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
    {
        return ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
    }
}