using System.Security.Cryptography;
using Hope.Security.Encryption.Symmetric;

public sealed class HopeMemoryProtector : ProtectorBase
{

    public HopeMemoryProtector(object baseProtector, params object[] additionalProtectors) : base(baseProtector, additionalProtectors)
    {
    }

    protected override byte[] InternalProtect(byte[] data, byte[] entropy)
    {
        if (data.Length % 16 != 0 || data.Length == 0)
            data = AesEncryptor.Encrypt(data, entropy);

        ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);

        return data;
    }

    protected override byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy)
    {
        if (encryptedData.Length % 16 != 0 || encryptedData.Length == 0)
            return null;

        ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);

        return AesEncryptor.Decrypt(encryptedData, entropy);
    }
}