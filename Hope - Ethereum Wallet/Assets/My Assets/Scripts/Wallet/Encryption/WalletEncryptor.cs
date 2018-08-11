using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.Random;
using Org.BouncyCastle.Security;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for encrypting the wallet data.
/// </summary>
public sealed class WalletEncryptor : SecureObject
{
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Initializes the <see cref="WalletEncryptor"/> by assigning the references to needed objects.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPassword"/> object to use with the encryption. </param>
    /// <param name="dynamicDataCache"> The <see cref="DynamicDataCache"/> used for storing the user password. </param>
    public WalletEncryptor(
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Encrypts the wallet given the seed and the base password of the user.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed of the wallet. </param>
    /// <param name="passwordBase"> The password of the user. </param>
    /// <param name="onWalletEncrypted"> Action to call once the wallet has been encrypted. Passing the array of hashes used to encrypt the wallet, the salted password hash, and encrypted seed. </param>
    public void EncryptWallet(byte[] seed, string passwordBase, Action<string[], string, string> onWalletEncrypted)
    {
        AsyncTaskScheduler.Schedule(() => AsyncEncryptWallet(seed, passwordBase, onWalletEncrypted));
    }

    /// <summary>
    /// Encrypts the wallet data asynchronously.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed to encrypt. </param>
    /// <param name="passwordBase"> The base password to use for encryption, retrieved from the user input. </param>
    /// <param name="onWalletEncrypted"> Action called once the wallet has been encrypted. </param>
    /// <returns> Task returned which represents the work needed to encrypt the wallet data. </returns>
    private async Task AsyncEncryptWallet(byte[] seed, string passwordBase, Action<string[], string, string> onWalletEncrypted)
    {
        SecureRandom secureRandom = new SecureRandom();

        string encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(passwordBase)).ConfigureAwait(false);

        Tuple<string, string> splitPass = encryptionPassword.SplitHalf();
        Tuple<string, string> lvl12string = splitPass.Item1.SplitHalf();
        Tuple<string, string> lvl34string = splitPass.Item2.SplitHalf();

        string h1 = await Task.Run(() => lvl12string.Item1.GetSHA256Hash().CombineAndRandomize(RandomBytes.GetSHA512Bytes(30).GetHexString())).ConfigureAwait(false);
        string h2 = await Task.Run(() => lvl12string.Item2.GetSHA256Hash().CombineAndRandomize(RandomBytes.GetSHA512Bytes(30).GetHexString())).ConfigureAwait(false);
        string h3 = await Task.Run(() => lvl34string.Item1.GetSHA256Hash().CombineAndRandomize(RandomBytes.GetSHA512Bytes(30).GetHexString())).ConfigureAwait(false);
        string h4 = await Task.Run(() => lvl34string.Item2.GetSHA256Hash().CombineAndRandomize(RandomBytes.GetSHA512Bytes(30).GetHexString())).ConfigureAwait(false);

        string encryptedSeed = await Task.Run(() => seed.GetHexString().AESEncrypt(h1 + h2).Protect(h3 + h4)).ConfigureAwait(false);
        string saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(passwordBase)).ConfigureAwait(false);

        string[] encryptedHashLvls = await Task.Run(() => new string[] { h1.AESEncrypt(lvl12string.Item1.GetSHA512Hash()),
                                                                         h2.Protect(),
                                                                         h3.Protect(),
                                                                         h4.AESEncrypt(lvl34string.Item2.GetSHA512Hash()) }).ConfigureAwait(false);

        dynamicDataCache.SetData("pass", new ProtectedString(passwordBase, this));
        dynamicDataCache.SetData("mnemonic", null);

        MainThreadExecutor.QueueAction(() => onWalletEncrypted?.Invoke(encryptedHashLvls, saltedPasswordHash, encryptedSeed));
    }
}