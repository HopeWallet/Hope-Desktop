using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.Random;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for encrypting the wallet data.
/// </summary>
public sealed class WalletEncryptor : SecureObject
{
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;
    private readonly UserWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Initializes the <see cref="WalletEncryptor"/> by assigning the references to needed objects.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPassword"/> object to use with the encryption. </param>
    /// <param name="dynamicDataCache"> The <see cref="DynamicDataCache"/> used for storing the user password. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    public WalletEncryptor(
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache, 
        UserWalletInfoManager.Settings walletSettings)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
        this.walletSettings = walletSettings;
    }

    /// <summary>
    /// Encrypts the wallet given the seed and the base password of the user.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed of the wallet. </param>
    /// <param name="passwordBase"> The password of the user. </param>
    /// <param name="walletNum"> The number of the wallet to encrypt. </param>
    /// <param name="onWalletEncrypted"> Action to call once the wallet has been encrypted. Passing the array of hashes used to encrypt the wallet, the salted password hash, and encrypted seed. </param>
    public void EncryptWallet(byte[] seed, string passwordBase, int walletNum, Action<string[], string, string> onWalletEncrypted)
    {
        AsyncTaskScheduler.Schedule(() => AsyncEncryptWallet(seed, passwordBase, walletNum, onWalletEncrypted));
    }

    /// <summary>
    /// Encrypts the wallet data asynchronously.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed to encrypt. </param>
    /// <param name="passwordBase"> The base password to use for encryption, retrieved from the user input. </param>
    /// <param name="walletNum"> The number of the wallet to encrypt. </param>
    /// <param name="onWalletEncrypted"> Action called once the wallet has been encrypted. </param>
    /// <returns> Task returned which represents the work needed to encrypt the wallet data. </returns>
    private async Task AsyncEncryptWallet(
        byte[] seed,
        string passwordBase,
        int walletNum,
        Action<string[], string, string> onWalletEncrypted)
    {
        string encryptionPassword = playerPrefPassword.GenerateEncryptionPassword(passwordBase);

        AdvancedSecureRandom secureRandom = await Task.Run(() =>
            new AdvancedSecureRandom(
                new Blake2bDigest(),
                walletSettings.walletCountPrefName,
                walletSettings.walletDataPrefName,
                walletSettings.walletDerivationPrefName,
                walletSettings.walletEncryptionEntropy,
                walletSettings.walletHashLvlPrefName,
                walletSettings.walletInfoPrefName,
                walletSettings.walletNamePrefName,
                walletSettings.walletPasswordPrefName,
                walletNum,
                encryptionPassword)).ConfigureAwait(false);

        string[] encryptedHashes = null;
        string saltedPasswordHash = null;
        string encryptedSeed = null;

        using (var dataEncryptor = new DataEncryptor(secureRandom))
        {
            string hash1 = encryptionPassword.SplitHalf().Item1;
            string hash2 = RandomBytes.Blake2.GetBytes(128).GetBase64String();
            string hash3 = encryptionPassword.SplitHalf().Item2;
            string hash4 = RandomBytes.Blake2.GetBytes(128).GetBase64String();

            encryptedSeed = dataEncryptor.Encrypt(dataEncryptor.Encrypt(seed.GetHexString(), (hash1 + hash2).GetSHA256Hash()), (hash3 + hash4).GetSHA256Hash());
            saltedPasswordHash = PasswordEncryption.Blake2.GetSaltedPasswordHash(passwordBase);

            encryptedHashes = new string[] { dataEncryptor.Encrypt(hash1), dataEncryptor.Encrypt(hash2), dataEncryptor.Encrypt(hash3), dataEncryptor.Encrypt(hash4) };
        }

        dynamicDataCache.SetData("pass", new ProtectedString(passwordBase, this));
        dynamicDataCache.SetData("mnemonic", null);

        MainThreadExecutor.QueueAction(() => onWalletEncrypted?.Invoke(encryptedHashes, saltedPasswordHash, encryptedSeed));
    }
}