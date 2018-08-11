using System;
using System.Linq;

public abstract class CrossPlatformEncryptor<TWinEncryptor, TOtherEncryptor> : AdvancedEntropyEncryptor
    where TWinEncryptor : AdvancedEntropyEncryptor
    where TOtherEncryptor : AdvancedEntropyEncryptor
{
    private readonly AdvancedEntropyEncryptor encryptor;

    protected abstract bool IsEphemeral { get; }

    protected CrossPlatformEncryptor(params object[] encryptors) : base(new object[0])
    {
        if (IsEphemeral)
            encryptors = encryptors.Concat(new object[] { this }).ToArray();
        else
            encryptors = encryptors.Where(protector => !protector.GetType().IsSubclassOf(typeof(SecureObject))).ToArray();

        encryptor = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? (AdvancedEntropyEncryptor)Activator.CreateInstance(typeof(TWinEncryptor), encryptors)
            : (AdvancedEntropyEncryptor)Activator.CreateInstance(typeof(TOtherEncryptor), encryptors);
    }

    [SecureCaller]
    public override byte[] Encrypt(byte[] data, byte[] entropy) => InternalEncrypt(data, entropy);

    [SecureCaller]
    public override byte[] Decrypt(byte[] encryptedData, byte[] entropy) => InternalDecrypt(encryptedData, entropy);

    [SecureCaller]
    protected byte[] InternalEncrypt(byte[] data, byte[] entropy)
    {
        return encryptor.Encrypt(data, entropy);
    }

    [SecureCaller]
    protected byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
    {
        return encryptor.Decrypt(encryptedData, entropy);
    }
}