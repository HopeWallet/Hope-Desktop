using System.Security.Cryptography;

public sealed class HopeDataProtector : ProtectorBase
{
    public HopeDataProtector(params object[] protectors) : base(protectors)
    {
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalProtect(byte[] data, byte[] entropy)
    {
        return ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy)
    {
        return ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
    }
}