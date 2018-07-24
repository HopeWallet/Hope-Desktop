using Hope.Security.Encryption.DPAPI;
using Hope.Security.Encryption.Symmetric;
using Hope.Security.HashGeneration;
using System;
using System.Diagnostics;

/// <summary>
/// Class that encrypts data that can only be decrypted by the same EphemeralEncrypt object during the same session of the program running.
/// </summary>
public sealed class EphemeralEncryption : SecureObject
{
    private readonly SecureObject[] optionalEncryptors;
    private readonly bool isWindows;

    /// <summary>
    /// Initializes the <see cref="EphemeralEncryption"/> object with no additional encrypting object.
    /// </summary>
    public EphemeralEncryption()
    {
        isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    /// <summary>
    /// Initializes the <see cref="EphemeralEncryption"/> object with an additional object used to encrypt data.
    /// </summary>
    /// <param name="optionalEncryptors"> The optional instances of <see cref="SecureObject"/> to use to encrypt and decrypt data. </param>
    public EphemeralEncryption(params SecureObject[] optionalEncryptors) : this()
    {
        this.optionalEncryptors = optionalEncryptors;
    }

    /// <summary>
    /// Encrypts <see langword="string"/> data which can only be decrypted over one session of lifetime.
    /// </summary>
    /// <param name="data"> The <see langword="string"/> data to encrypt. </param>
    /// <returns> The encrypted data as a <see langword="string"/>. </returns>
    [SecureCaller]
    public string Encrypt(string data) => Encrypt(data.GetUTF8Bytes()).GetBase64String();

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data which can only be decrypted over one session of lifetime.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Encrypt(byte[] data) => Encrypt(data, string.Empty);

    /// <summary>
    /// Encrypts <see langword="string"/> data which can only be decrypted over one session of lifetime.
    /// Uses an additional <see langword="string"/> entropy for encryption.
    /// </summary>
    /// <param name="data"> The <see langword="string"/> data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Encrypt(string data, string entropy) => Encrypt(data.GetUTF8Bytes(), entropy).GetBase64String();

    /// <summary>
    /// Decrypts <see langword="string"/> data which was encrypted earlier using <see cref="EphemeralEncryption.Encrypt"/>.
    /// </summary>
    /// <param name="encryptedText"> The encrypted text to decrypt. </param>
    /// <returns> The decrypted <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Decrypt(string encryptedText) => Decrypt(encryptedText.GetBase64Bytes()).GetUTF8String();

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data which was encrypted earlier using <see cref="Encrypt(byte[])"/>.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public byte[] Decrypt(byte[] encryptedData) => Decrypt(encryptedData, string.Empty);

    /// <summary>
    /// Decrypts <see langword="string"/> data which was encrypted earlier using <see cref="Encrypt(string, string)"/>.
    /// Uses an additional <see langword="string"/> entropy for decryption.
    /// </summary>
    /// <param name="encryptedText"> The encrypted <see langword="string"/> data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the decryption. </param>
    /// <returns> The decrypteed <see langword="string"/> data. </returns>
    [SecureCaller]
    public string Decrypt(string encryptedText, string entropy) => Decrypt(encryptedText.GetBase64Bytes(), entropy).GetUTF8String();

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data which can only be decrypted over one session of lifetime.
    /// Uses an additional <see langword="string"/> entropy for the encryption.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The additional <see langword="string"/> entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    public byte[] Encrypt(byte[] data, string entropy)
    {
        byte[] aesEncryptedData = AesEncryptor.Encrypt(data, GetEncryptionPassword(entropy));
        return isWindows ? MemoryProtect.Protect(aesEncryptedData) : aesEncryptedData;
    }

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data which was encrypted using <see cref="Encrypt(byte[], string)"/>.
    /// Uses an additional <see langword="string"/> entropy for decryption.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The additional <see langword="string"/> entropy to apply to the decryption. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(byte[]))]
    public byte[] Decrypt(byte[] encryptedData, string entropy)
    {
        encryptedData = isWindows ? MemoryProtect.Unprotect(encryptedData) : encryptedData;
        return AesEncryptor.Decrypt(encryptedData, GetEncryptionPassword(entropy));
    }

    /// <summary>
    /// Gets the encryption password used to encrypt/decrypt data.
    /// </summary>
    /// <param name="entropy"> The additional <see langword="string"/> entropy to use to derive the encryption password. </param>
    /// <returns> The encryption password. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(string))]
    private string GetEncryptionPassword(string entropy)
    {
        string optionalEncryptionData = string.Empty;
        foreach (var obj in optionalEncryptors)
            optionalEncryptionData = optionalEncryptionData.CombineAndRandomize(obj.GetHashCode().ToString().GetSHA1Hash());

        Process process = Process.GetCurrentProcess();

        return process.Id.ToString().GetSHA256Hash()
                    .CombineAndRandomize(optionalEncryptionData.GetSHA384Hash())
                    .CombineAndRandomize(GetHashCode().ToString().GetSHA256Hash())
                    .CombineAndRandomize(process.MainModule.ModuleName.GetHashCode().ToString().GetSHA384Hash())
                    .CombineAndRandomize(entropy.GetSHA256Hash()).GetSHA512Hash();
    }
}