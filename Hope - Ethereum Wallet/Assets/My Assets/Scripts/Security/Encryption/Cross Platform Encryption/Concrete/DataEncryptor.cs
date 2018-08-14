using Hope.Security.Encryption.DPAPI;
using Hope.Security.Encryption.Symmetric;
using RandomNET.Secure;

/// <summary>
/// Cross platform class which encrypts/decrypts data for use across multiple sessions.
/// </summary>
public sealed class DataEncryptor : CrossPlatformEncryptor<WindowsDataEncryptor, AesEncryptor>
{
    /// <summary>
    /// Whether this <see cref="CrossPlatformEncryptor"/> implements shorter term encryption methods.
    /// </summary>
    protected override bool IsEphemeral => false;

    /// <summary>
    /// Initializes the <see cref="DataEncryptor"/> by assigning the encryptors to the <see cref="CrossPlatformEncryptor"/>.
    /// </summary>
    /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
    public DataEncryptor(params object[] encryptors) : base(encryptors)
    {
    }

    /// <summary>
    /// Initializes the <see cref="DataEncryptor"/> given the <see cref="AdvancedSecureRandom"/> instance to use for our encryption.
    /// </summary>
    /// <param name="secureRandom"> The <see cref="AdvancedSecureRandom"/> instance to use for our encryption. </param>
    public DataEncryptor(AdvancedSecureRandom secureRandom) : base(secureRandom)
    {
    }
}