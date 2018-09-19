using Hope.Random;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.Encryption.Symmetric;
using Org.BouncyCastle.Crypto.Digests;
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
        new AdvancedSecureRandom(new Sha3Digest(512), Process.GetCurrentProcess().Id).NextBytes(128),
        new AdvancedSecureRandom(new KeccakDigest(512), Process.GetCurrentProcess().MainModule.ModuleName.GetHashCode()).NextBytes(128),
        new AdvancedSecureRandom(new Blake2bDigest(512), encryptors).NextBytes(128))
    {
    }
}