using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for decrypting wallet data.
/// </summary>
public sealed class WalletDecryptor : SecureObject
{
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Initializes the <see cref="WalletDecryptor"/> with the references to needed objects.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPassword"/> object used to encrypt the wallet data. </param>
    /// <param name="dynamicDataCache"> The <see cref="DynamicDataCache"/> used for retrieving the number of the wallet we are decrypting. </param>
    public WalletDecryptor(
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Decrypts the wallet given the user's password.
    /// </summary>
    /// <param name="password"> The user's password to the wallet. </param>
    /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted, passing the <see langword="byte"/>[] seed of the wallet. </param>
    public void DecryptWallet(string password, Action<byte[]> onWalletDecrypted)
    {
        int walletNum = (int)dynamicDataCache.GetData("walletnum");

        MainThreadExecutor.QueueAction(() =>
        {
            playerPrefPassword.PopulatePrefDictionary(walletNum);

            string[] hashLvls = new string[4];
            for (int i = 0; i < hashLvls.Length; i++)
                hashLvls[i] = SecurePlayerPrefs.GetString("wallet_" + walletNum + "_h" + (i + 1));

            AsyncTaskScheduler.Schedule(() => AsyncDecryptWallet(hashLvls, SecurePlayerPrefs.GetString("wallet_" + walletNum), password, onWalletDecrypted));
        });
    }

    /// <summary>
    /// Decrypts the wallet asynchronously.
    /// </summary>
    /// <param name="hashLvls"> Different hash levels used for multi level encryption of the wallet seed. </param>
    /// <param name="encryptedSeed"> The encrypted seed of the wallet. </param>
    /// <param name="password"> The user's password to the wallet. </param>
    /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted, passing the <see langword="byte"/>[] seed of the wallet. </param>
    /// <returns> Task returned which represents the decryption processing. </returns>
    private async Task AsyncDecryptWallet(string[] hashLvls, string encryptedSeed, string password, Action<byte[]> onWalletDecrypted)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.ExtractEncryptionPassword(password).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.Item1.SplitHalf();
        var lvl34string = splitPass.Item2.SplitHalf();

        string unprotectedSeed = await Task.Run(() => encryptedSeed.Unprotect(hashLvls[2].Unprotect() + hashLvls[3].AESDecrypt(lvl34string.Item2.GetSHA512Hash()))).ConfigureAwait(false);
        byte[] decryptedSeed = await Task.Run(() => unprotectedSeed.AESDecrypt(hashLvls[0].AESDecrypt(lvl12string.Item1.GetSHA512Hash()) + hashLvls[1].Unprotect()).HexToByteArray()).ConfigureAwait(false);

        onWalletDecrypted?.Invoke(decryptedSeed);
    }
}