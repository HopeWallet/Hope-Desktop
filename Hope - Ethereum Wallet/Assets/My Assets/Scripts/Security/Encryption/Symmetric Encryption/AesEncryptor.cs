using System.Security.Cryptography;

/// <summary>
/// SymmetricEncryptor class used for encrypting data using the AesManaged SymmetricAlgorithm.
/// </summary>
public sealed class AesEncryptor : SymmetricEncryptor<AesEncryptor, AesManaged>
{
    /// <summary>
    /// The key size of the <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    protected override int KeySize => 128;

    /// <summary>
    /// The number of bytes to use for the salt and iv for the <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    protected override int SaltIvByteSize => 16;
}