using Hope.Security.Encryption.Symmetric;
using System.Diagnostics;

/// <summary>
/// Cross platform class for encrypting/decrypting certain data in memory to avoid potential tampering.
/// Data can only be decrypted by the same instance of MemoryEncryptor.
/// Methods that encrypt/decrypt data need to have the attribute SecureCallEnd or SecureCaller.
/// </summary>
public sealed class MemoryEncryptor : CrossPlatformEncryptor<WindowsMemoryEncryptor, AesEncryptor>
{
    /// <summary>
    /// Whether this <see cref="CrossPlatformEncryptor"/> implements shorter term encryption methods.
    /// </summary>
    protected override bool IsEphemeral => true;

    /// <summary>
    /// Initializes the <see cref="MemoryEncryptor"/> by assigning the encryptors to the <see cref="CrossPlatformEncryptor"/> 
    /// with additional parameters which hold our current process info.
    /// </summary>
    /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
    public MemoryEncryptor(params object[] encryptors) : base(
        Process.GetCurrentProcess().Id,
        Process.GetCurrentProcess().MainModule.ModuleName.GetHashCode(),
        encryptors)
    {
    }
}