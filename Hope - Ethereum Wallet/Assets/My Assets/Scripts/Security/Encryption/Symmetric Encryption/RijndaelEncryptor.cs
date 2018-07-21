using System.Security.Cryptography;

public sealed class RijndaelEncryptor : SymmetricEncryptor<RijndaelEncryptor, RijndaelManaged>
{
    public override int KeySize => 256;

    public override int SaltIvByteSize => 32;
}