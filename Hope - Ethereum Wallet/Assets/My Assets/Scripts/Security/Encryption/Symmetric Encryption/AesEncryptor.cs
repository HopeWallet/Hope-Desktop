using System.Security.Cryptography;

public sealed class AesEncryptor : SymmetricEncryptor<AesEncryptor, AesManaged>
{
    public override int KeySize => 128;

    public override int SaltIvByteSize => 16;
}