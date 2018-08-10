using System.Security.Cryptography;

public sealed class HopeDataProtector : ProtectorBase
{
    public HopeDataProtector(object baseProtector, params object[] additionalProtectors) : base(baseProtector, additionalProtectors)
    {
    }

    protected override byte[] InternalProtect(byte[] data, byte[] entropy) => ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);

    protected override byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy) => ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
}