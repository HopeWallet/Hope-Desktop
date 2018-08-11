using System.Security.Cryptography;
using Hope.Security.Encryption.Symmetric;
using Org.BouncyCastle.Security;

public sealed class HopeMemoryProtector : ProtectorBase
{
    private static byte[] randomEntropy;

    public HopeMemoryProtector(params object[] protectors) : base(protectors)
    {
        if (randomEntropy != null)
            return;

        SecureRandom secureRandom = new SecureRandom();
        randomEntropy = SecureRandom.GetNextBytes(secureRandom, secureRandom.Next(16, 32));
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalProtect(byte[] data, byte[] entropy)
    {
        if (data.Length % 16 != 0 || data.Length == 0)
            data = AesEncryptor.Encrypt(data, entropy?.Length > 0 ? entropy : randomEntropy);

        ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);

        return data;
    }

    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalUnprotect(byte[] encryptedData, byte[] entropy)
    {
        if (encryptedData.Length % 16 != 0 || encryptedData.Length == 0)
            return null;

        ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);

        return AesEncryptor.Decrypt(encryptedData, entropy?.Length > 0 ? entropy : randomEntropy);
    }
}