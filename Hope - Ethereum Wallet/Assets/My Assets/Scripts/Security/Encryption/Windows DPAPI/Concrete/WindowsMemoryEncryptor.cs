using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Hope.Security.Encryption.Symmetric;

public sealed class WindowsMemoryEncryptor : WindowsEncryptor
{
    private readonly AesEncryptor aes;
    private readonly byte[] randomEntropy;

    public WindowsMemoryEncryptor(params object[] encryptors) : base(encryptors)
    {
        aes = new AesEncryptor(encryptors);
        randomEntropy = SecureRandom.GetNextBytes(new SecureRandom(), 32);
    }

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalEncrypt(byte[] data, byte[] entropy)
    {
        byte[] encryptedData = data;
        if (data.Length % 16 != 0 || data.Length == 0)
        {
            encryptedData = aes.Encrypt(data, entropy?.Length > 0 ? entropy : randomEntropy);
            data.ClearBytes();
        }

        ProtectedMemory.Protect(encryptedData, MemoryProtectionScope.SameProcess);

        return encryptedData;
    }

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    protected override byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
    {
        if (encryptedData.Length % 16 != 0 || encryptedData.Length == 0)
            return null;

        ProtectedMemory.Unprotect(encryptedData, MemoryProtectionScope.SameProcess);

        return aes.Decrypt(encryptedData, entropy?.Length > 0 ? entropy : randomEntropy);
    }
}