public abstract class WindowsEncryptor : AdvancedEntropyEncryptor
{
    protected WindowsEncryptor(params object[] encryptors) : base(encryptors)
    {
    }

    [SecureCaller]
    public override byte[] Encrypt(byte[] data, byte[] entropy) => InternalEncrypt(data, GetAdvancedEntropyHash(entropy));

    [SecureCaller]
    public override byte[] Decrypt(byte[] encryptedData, byte[] entropy) => InternalDecrypt(encryptedData, GetAdvancedEntropyHash(entropy));

    [SecureCaller]
    protected abstract byte[] InternalEncrypt(byte[] data, byte[] entropy);

    [SecureCaller]
    protected abstract byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy);
}