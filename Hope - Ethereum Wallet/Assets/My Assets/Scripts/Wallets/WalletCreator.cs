using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using System;
using System.Threading.Tasks;
using SecureRandom = Org.BouncyCastle.Security.SecureRandom;

public class WalletCreator : WalletLoaderBase
{
    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache) : base(popupManager, playerPrefPassword, dynamicDataCache)
    {
    }

    protected override void LoadWallet(string userPass)
    {
        CreateWalletCountPref();
        TryCredentials(userPass);
    }

    protected override void SetupPopup()
    {
        popupManager.GetPopup<LoadingPopup>(true).Text = "Creating wallet";
    }

    private void SetWalletPlayerPrefs(string[] encryptedHashLvls, string saltedPasswordHash, string encryptedSeed)
    {
        int walletNum = SecurePlayerPrefs.GetInt("wallet_count") + 1;

        SecurePlayerPrefs.SetInt("wallet_count", walletNum);
        SecurePlayerPrefs.SetString("password_" + walletNum, saltedPasswordHash);
        SecurePlayerPrefs.SetString("wallet_" + walletNum, encryptedSeed);
        SecurePlayerPrefs.SetString("wallet_" + walletNum + "_name", dynamicDataCache.GetData("name"));

        for (int i = 0; i < encryptedHashLvls.Length; i++)
            SecurePlayerPrefs.SetString("wallet_" + walletNum + "_h" + (i + 1), encryptedHashLvls[i]);

        playerPrefPassword.SetupPlayerPrefs(walletNum, onWalletLoaded);
    }

    private void CreateWalletCountPref()
    {
        if (!SecurePlayerPrefs.HasKey("wallet_count"))
            SecurePlayerPrefs.SetInt("wallet_count", 0);
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="basePass"> The password that was entered by the user. </param>
    private void TryCredentials(string basePass)
    {
        if (string.IsNullOrEmpty(basePass) || basePass.Length < AESEncryption.MIN_PASSWORD_LENGTH)
        {
            ExceptionManager.DisplayException(new Exception("Invalid wallet password. Please use a password with more than 8 characters!"));
            return;
        }

        using (var mnemonic = (dynamicDataCache.GetData("mnemonic") as ProtectedString)?.CreateDisposableData())
        {
            try
            {
                var wallet = new Wallet(mnemonic.Value, null, WalletUtils.DetermineCorrectPath(mnemonic.Value));
                AsyncTaskScheduler.Schedule(() => GetAddresses(wallet));
                AsyncTaskScheduler.Schedule(() => EncryptWalletData(wallet.Seed, basePass));
            }
            catch (Exception e)
            {
                dynamicDataCache.SetData("mnemonic", null);
                ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again. => " + e.Message));
            }
        }
    }

    private async Task EncryptWalletData(byte[] seed, string basePass)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(basePass).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.Item1.SplitHalf();
        var lvl34string = splitPass.Item2.SplitHalf();

        SecureRandom secureRandom = new SecureRandom();

        string h1 = await Task.Run(() => lvl12string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h2 = await Task.Run(() => lvl12string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h3 = await Task.Run(() => lvl34string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h4 = await Task.Run(() => lvl34string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string encryptedSeed = await Task.Run(() => seed.GetHexString().AESEncrypt(h1 + h2).Protect(h3 + h4)).ConfigureAwait(false);
        string saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(basePass)).ConfigureAwait(false);
        string[] encryptedHashLvls = await Task.Run(() => new string[] { h1.AESEncrypt(lvl12string.Item1.GetSHA512Hash()), h2.Protect(), h3.Protect(), h4.AESEncrypt(lvl34string.Item2.GetSHA512Hash()) }).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => SetWalletPlayerPrefs(encryptedHashLvls, saltedPasswordHash, encryptedSeed));
    }
}
