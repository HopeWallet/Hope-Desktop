using Hope.Random;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for decrypting wallet data.
/// </summary>
public sealed class WalletDecryptor : SecureObject
{
    private readonly PlayerPrefPasswordDerivation playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Initializes the <see cref="WalletDecryptor"/> with the references to needed objects.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPasswordDerivation"/> object used to encrypt the wallet data. </param>
    /// <param name="dynamicDataCache"> The <see cref="DynamicDataCache"/> used for retrieving the number of the wallet we are decrypting. </param>
    /// <param name="walletSettings"> The settings for the <see cref="HopeWallet"/>. </param>
    public WalletDecryptor(
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Decrypts the wallet given the user's password.
    /// </summary>
    /// <param name="walletInfo"> The wallet to decrypt. </param>
    /// <param name="password"> The user's password to the wallet. </param>
    /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted, passing the <see langword="byte"/>[] seed of the wallet. </param>
    public void DecryptWallet(WalletInfo walletInfo, byte[] password, Action<byte[]> onWalletDecrypted)
    {
        MainThreadExecutor.QueueAction(() =>
        {
            playerPrefPassword.PopulatePrefDictionary(walletInfo.WalletNum);

            Task.Factory.StartNew(() => AsyncDecryptWallet(
                walletInfo.EncryptedWalletData.EncryptionHashes,
                walletInfo.EncryptedWalletData.EncryptedSeed,
                password,
                walletInfo.WalletNum,
                onWalletDecrypted));
        });
    }

    /// <summary>
    /// Decrypts the wallet asynchronously.
    /// </summary>
    /// <param name="hashes"> Different hash levels used for multi level encryption of the wallet seed. </param>
    /// <param name="encryptedSeed"> The encrypted seed of the wallet. </param>
    /// <param name="password"> The user's password to the wallet. </param>
    /// <param name="walletNum"> The number of the wallet to decrypt. </param>
    /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted, passing the <see langword="byte"/>[] seed of the wallet. </param>
    private void AsyncDecryptWallet(
        string[] hashes,
        string encryptedSeed,
        byte[] password,
        int walletNum,
        Action<byte[]> onWalletDecrypted)
    {
        byte[] derivedPassword = playerPrefPassword.Restore(password);
        byte[] decryptedSeed = null;

        using (var dataEncryptor = new DataEncryptor(new AdvancedSecureRandom(new Blake2bDigest(512), derivedPassword)))
        {
            byte[] hash1 = dataEncryptor.Decrypt(hashes[0].GetBase64Bytes());
            byte[] hash2 = dataEncryptor.Decrypt(hashes[1].GetBase64Bytes());

            decryptedSeed = dataEncryptor.Decrypt(dataEncryptor.Decrypt(encryptedSeed, hash2), hash1).HexToByteArray();

            password.ClearBytes();
            hash1.ClearBytes();
            hash2.ClearBytes();
        }

        onWalletDecrypted?.Invoke(decryptedSeed);
    }
}