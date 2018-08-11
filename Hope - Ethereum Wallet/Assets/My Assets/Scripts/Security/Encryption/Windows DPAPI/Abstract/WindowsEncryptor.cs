public abstract class WindowsEncryptor : MultiLevelEncryptor, ISimpleEncryptor
{
    protected WindowsEncryptor(params object[] encryptors) : base(encryptors)
    {
    }

    [SecureCaller]
    public override byte[] Encrypt(byte[] data, byte[] entropy) => InternalEncrypt(data, GetMultiLevelEncryptionHash(entropy));

    [SecureCaller]
    public override byte[] Decrypt(byte[] encryptedData, byte[] entropy) => InternalDecrypt(encryptedData, GetMultiLevelEncryptionHash(entropy));

    protected abstract byte[] InternalEncrypt(byte[] data, byte[] entropy);

    protected abstract byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy);
}