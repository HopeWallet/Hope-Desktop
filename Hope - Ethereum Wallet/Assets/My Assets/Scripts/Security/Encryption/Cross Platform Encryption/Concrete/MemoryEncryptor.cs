using Hope.Security.Encryption.Symmetric;
using System.Diagnostics;

/// <summary>
/// Class that encrypts data that can only be decrypted by the same EphemeralEncrypt object during the same session of the program running.
/// </summary>
public sealed class MemoryEncryptor : CrossPlatformEncryptor<WindowsMemoryEncryptor, AesEncryptor>
{
    public MemoryEncryptor(params object[] encryptors) : base(
        Process.GetCurrentProcess().Id,
        Process.GetCurrentProcess().MainModule.ModuleName.GetHashCode(),
        encryptors)
    {
    }
}