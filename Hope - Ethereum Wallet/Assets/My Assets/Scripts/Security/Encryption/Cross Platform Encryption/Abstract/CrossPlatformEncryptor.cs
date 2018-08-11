using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class CrossPlatformEncryptor<TWinEncryptor, TOtherEncryptor> : MultiLevelEncryptor

    where TWinEncryptor : MultiLevelEncryptor, ISimpleEncryptor

    where TOtherEncryptor : MultiLevelEncryptor, ISimpleEncryptor
{
    private readonly ISimpleEncryptor encryptor;

    protected CrossPlatformEncryptor(params object[] encryptors) : base(new object[0])
    {
        encryptor = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? (ISimpleEncryptor)Activator.CreateInstance(typeof(TWinEncryptor), encryptors)
            : (ISimpleEncryptor)Activator.CreateInstance(typeof(TOtherEncryptor), encryptors);
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