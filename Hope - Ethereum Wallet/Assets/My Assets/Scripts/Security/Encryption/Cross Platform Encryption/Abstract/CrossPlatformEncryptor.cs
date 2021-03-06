﻿using Hope.Random;
using Hope.Random.Bytes;
using System;
using System.Linq;

/// <summary>
/// Base class used for encrypting/decrypting data with cross platform functionality.
/// </summary>
/// <typeparam name="TWinEncryptor"> The type of our Windows encryptor for this algorithm. </typeparam>
/// <typeparam name="TOtherEncryptor"> The type of our non-Windows encryptor for this algorithm. </typeparam>
public abstract class CrossPlatformEncryptor<TWinEncryptor, TOtherEncryptor> : AdvancedEntropyEncryptor
    where TWinEncryptor : AdvancedEntropyEncryptor
    where TOtherEncryptor : AdvancedEntropyEncryptor
{
    private readonly AdvancedEntropyEncryptor encryptor;

    /// <summary>
    /// Whether this <see cref="CrossPlatformEncryptor"/> implements shorter term encryption methods.
    /// </summary>
    protected abstract bool IsEphemeral { get; }

    /// <summary>
    /// Initializes the <see cref="CrossPlatformEncryptor"/> by assigning the encryptors to the <see cref="AdvancedEntropyEncryptor"/>.
    /// Also initializes our given <see cref="AdvancedEntropyEncryptor"/> based on the currently running platform.
    /// </summary>
    /// <param name="encryptors"> The additional encryptors to use as our advanced entropy. </param>
    protected CrossPlatformEncryptor(params object[] encryptors) : base(new object[0])
    {
        if (IsEphemeral)
            encryptors = encryptors.Concat(new object[] { this, RandomBytes.Secure.SHA3.GetBytes(32).GetHexString() }).ToArray();
        else
            encryptors = encryptors.Where(protector => !protector.GetType().IsSubclassOf(typeof(SecureObject))).ToArray();

        encryptor = Environment.OSVersion.Platform == PlatformID.Win32NT
            ? (AdvancedEntropyEncryptor)Activator.CreateInstance(typeof(TWinEncryptor), encryptors)
            : (AdvancedEntropyEncryptor)Activator.CreateInstance(typeof(TOtherEncryptor), encryptors);
    }

    /// <summary>
    /// Initializes the <see cref="CrossPlatformEncryptor"/> given the <see cref="AdvancedSecureRandom"/> instance to use for our encryption.
    /// </summary>
    /// <param name="secureRandom"> The <see cref="AdvancedSecureRandom"/> instance to use for our encryption. </param>
    protected CrossPlatformEncryptor(AdvancedSecureRandom secureRandom) : this(
        secureRandom.NextBytes(2),
        secureRandom.NextBytes(4),
        secureRandom.NextBytes(8),
        secureRandom.NextBytes(16),
        secureRandom.NextBytes(32),
        secureRandom.NextBytes(64),
        secureRandom.NextBytes(128),
        secureRandom.NextBytes(256))
    {
    }

    /// <summary>
    /// Disposes of the <see cref="AdvancedEntropyEncryptor"/> object.
    /// </summary>
    public override void Dispose()
    {
        if (!Disposed)
        {
            encryptor.Dispose();
            Disposed = true;
        }
    }

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data with an additional entropy parameter.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public override byte[] Encrypt(byte[] data, byte[] entropy) => InternalEncrypt(data, entropy);

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data using the additional entropy parameter.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    public override byte[] Decrypt(byte[] encryptedData, byte[] entropy) => InternalDecrypt(encryptedData, entropy);

    /// <summary>
    /// Encrypts <see langword="byte"/>[] data using our currently assigned <see cref="AdvancedEntropyEncryptor"/>
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to encrypt. </param>
    /// <param name="entropy"> The additional entropy to apply to the encryption. </param>
    /// <returns> The encrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    //[ReflectionProtect(typeof(byte[]))]
    protected byte[] InternalEncrypt(byte[] data, byte[] entropy)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentNullException("Data to encrypt is null or has a length of 0!");

        byte[] encryptedData = encryptor.Encrypt(data, entropy);
        data.ClearBytes();

        return encryptedData;
    }

    /// <summary>
    /// Decrypts <see langword="byte"/>[] data using our currently assigned <see cref="AdvancedEntropyEncryptor"/>.
    /// </summary>
    /// <param name="encryptedData"> The encrypted <see langword="byte"/>[] data to decrypt. </param>
    /// <param name="entropy"> The additional entropy to use to decrypt the data. </param>
    /// <returns> The decrypted <see langword="byte"/>[] data. </returns>
    [SecureCaller]
    //[ReflectionProtect(typeof(byte[]))]
    protected byte[] InternalDecrypt(byte[] encryptedData, byte[] entropy)
    {
        if (encryptedData == null || encryptedData.Length == 0)
            throw new ArgumentNullException("Data to decrypt is null or has a length of 0!");

        byte[] decryptedData = encryptor.Decrypt(encryptedData, entropy);
        encryptedData.ClearBytes();

        return decryptedData;
    }
}