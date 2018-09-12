using Hope.Random;
using Hope.Random.Bytes;
using Hope.Security.HashGeneration;
using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using Hope.Security.ProtectedTypes.Types;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Class used for encrypting the wallet data.
/// </summary>
public sealed class WalletEncryptor : SecureObject
{
    private readonly PlayerPrefPasswordDerivation playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;
    private readonly HopeWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Initializes the <see cref="WalletEncryptor"/> by assigning the references to needed objects.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPasswordDerivation"/> object to use with the encryption. </param>
    /// <param name="dynamicDataCache"> The <see cref="DynamicDataCache"/> used for storing the user password. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    public WalletEncryptor(
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache, 
        HopeWalletInfoManager.Settings walletSettings)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
        this.walletSettings = walletSettings;
    }

    /// <summary>
    /// Encrypts the wallet given the seed and the base password of the user.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed of the wallet. </param>
    /// <param name="password"> The password of the user. </param>
    /// <param name="walletNum"> The number of the wallet to encrypt. </param>
    /// <param name="onWalletEncrypted"> Action to call once the wallet has been encrypted. Passing the array of hashes used to encrypt the wallet, the salted password hash, and encrypted seed. </param>
    public void EncryptWallet(byte[] seed, string password, int walletNum, Action<string[], string, string> onWalletEncrypted)
    {
        Task.Factory.StartNew(() => AsyncEncryptWallet(seed, password, walletNum, onWalletEncrypted));
    }

    /// <summary>
    /// Encrypts the wallet data asynchronously.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed to encrypt. </param>
    /// <param name="password"> The base password to use for encryption, retrieved from the user input. </param>
    /// <param name="walletNum"> The number of the wallet to encrypt. </param>
    /// <param name="onWalletEncrypted"> Action called once the wallet has been encrypted. </param>
    private void AsyncEncryptWallet(
        byte[] seed,
        string password,
        int walletNum,
        Action<string[], string, string> onWalletEncrypted)
    {
        byte[] derivedPassword = playerPrefPassword.Derive(password.GetUTF8Bytes());

        AdvancedSecureRandom secureRandom = new AdvancedSecureRandom(
                new Blake2bDigest(512),
                walletSettings.walletCountPrefName,
                walletSettings.walletDataPrefName,
                walletSettings.walletDerivationPrefName,
                walletSettings.walletEncryptionEntropy,
                walletSettings.walletHashLvlPrefName,
                walletSettings.walletInfoPrefName,
                walletSettings.walletNamePrefName,
                walletSettings.walletPasswordPrefName,
                walletNum,
                derivedPassword);

        string[] encryptedHashes = null;
        string saltedPasswordHash = null;
        string encryptedSeed = null;

        using (var dataEncryptor = new DataEncryptor(secureRandom))
        {
            byte[] hash1 = derivedPassword.Take(derivedPassword.Length / 2).ToArray().Blake2_512();
            byte[] hash2 = RandomBytes.Secure.Blake2.GetBytes(128);
            byte[] hash3 = derivedPassword.Skip(derivedPassword.Length / 2).ToArray().Blake2_512();
            byte[] hash4 = RandomBytes.Secure.Blake2.GetBytes(128);

            encryptedSeed = dataEncryptor.Encrypt(dataEncryptor.Encrypt(seed.GetHexString(), hash1.Concat(hash2).ToArray().SHA3_512()), hash3.Concat(hash4).ToArray().SHA3_512());
            saltedPasswordHash = new PBKDF2PasswordHashing(new Blake2b_512_Engine()).GetSaltedPasswordHash(password);

            encryptedHashes = new string[]
            {
                dataEncryptor.Encrypt(hash1).GetBase64String(),
                dataEncryptor.Encrypt(hash2).GetBase64String(),
                dataEncryptor.Encrypt(hash3).GetBase64String(),
                dataEncryptor.Encrypt(hash4).GetBase64String()
            };

            hash1.ClearBytes();
            hash2.ClearBytes();
            hash3.ClearBytes();
            hash4.ClearBytes();
        }

        dynamicDataCache.SetData("pass", new ProtectedString(password, this));
        dynamicDataCache.SetData("mnemonic", null);

        MainThreadExecutor.QueueAction(() => onWalletEncrypted?.Invoke(encryptedHashes, saltedPasswordHash, encryptedSeed));
    }
}