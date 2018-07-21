using Hope.Security.Encryption.Symmetric;
using Hope.Security.HashGeneration;
using System.Diagnostics;

/// <summary>
/// Class that encrypts data that can only be decrypted by the same EphemeralEncrypt object during the same session of the program running.
/// </summary>
public sealed class EphemeralEncryption : SecureObject
{
    private readonly SecureObject optionalEncryptor;

    /// <summary>
    /// Initializes the <see cref="EphemeralEncryption"/> object with no additional encrypting object.
    /// </summary>
    public EphemeralEncryption()
    {
    }

    /// <summary>
    /// Initializes the <see cref="EphemeralEncryption"/> object with an additional object used to encrypt data.
    /// </summary>
    /// <param name="optionalEncryptor"> The optional instance of <see cref="SecureObject"/> to use to encrypt and decrypt data. </param>
    public EphemeralEncryption(SecureObject optionalEncryptor)
    {
        this.optionalEncryptor = optionalEncryptor;
    }

    [SecureCaller]
    public string Encrypt(string data) => Encrypt(data.GetUTF8Bytes()).GetBase64String();

    [SecureCaller]
    public byte[] Encrypt(byte[] data) => Encrypt(data, string.Empty);

    [SecureCaller]
    public string Encrypt(string data, string entropy) => Encrypt(data.GetUTF8Bytes(), entropy).GetBase64String();

    [SecureCaller]
    public string Decrypt(string encryptedText) => Decrypt(encryptedText.GetBase64Bytes()).GetUTF8String();

    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData) => Decrypt(encryptedData, string.Empty);

    [SecureCaller]
    public string Decrypt(string encryptedText, string entropy) => Decrypt(encryptedText.GetBase64Bytes(), entropy).GetUTF8String();

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    public byte[] Encrypt(byte[] data, string entropy)
    {
        return AesEncryptor.Encrypt(data, GetEncryptionPassword(entropy));
    }

    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    public byte[] Decrypt(byte[] encryptedData, string entropy)
    {
        return AesEncryptor.Decrypt(encryptedData, GetEncryptionPassword(entropy));
    }

    [SecureCaller]
    [ReflectionProtect(typeof(string))]
    private string GetEncryptionPassword(string entropy)
    {
        return GetProcessId().ToString().GetSHA256Hash()
                    .CombineAndRandomize(optionalEncryptor == null ? "" : optionalEncryptor.GetHashCode().ToString().GetSHA384Hash())
                    .CombineAndRandomize(GetHashCode().ToString().GetSHA256Hash())
                    .CombineAndRandomize(GetProcessModuleHashCode().ToString().GetSHA384Hash())
                    .CombineAndRandomize(entropy.GetSHA256Hash()).GetSHA512Hash();
    }

    [ReflectionProtect(typeof(int))]
    private int GetProcessId()
    {
        return Process.GetCurrentProcess().Id;
    }

    [ReflectionProtect(typeof(int))]
    private int GetProcessModuleHashCode()
    {
        return Process.GetCurrentProcess().MainModule.ModuleName.GetHashCode();
    }
}