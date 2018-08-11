using Hope.Security.Encryption.Symmetric;

/// <summary>
/// Class which encrypts data which can be decrypted over several sessions of lifetime.
/// </summary>
public sealed class DataEncryptor : CrossPlatformEncryptor<WindowsDataEncryptor, AesEncryptor>
{
    public DataEncryptor(params object[] encryptors) : base(encryptors)
    {
    }
}