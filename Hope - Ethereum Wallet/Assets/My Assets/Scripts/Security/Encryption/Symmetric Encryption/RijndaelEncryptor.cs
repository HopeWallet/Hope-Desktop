using System.Security.Cryptography;

/// <summary>
/// SymmetricEncryptor class used for encrypting data using the RijndaelManaged SymmetricAlgorithm.
/// </summary>
public sealed class RijndaelEncryptor : SymmetricEncryptor<RijndaelEncryptor, RijndaelManaged>
{
    /// <summary>
    /// The key size of the <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    protected override int KeySize => 256;

    /// <summary>
    /// The number of bytes to use for the salt and iv for the <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    protected override int SaltIvByteSize => 32;
}