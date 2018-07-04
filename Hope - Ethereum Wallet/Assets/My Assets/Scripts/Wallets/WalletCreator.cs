using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SecureRandom = Org.BouncyCastle.Security.SecureRandom;

public class WalletCreator
{
    private static readonly string WALLET_NUM_PREF = HashGenerator.GetSHA512Hash("wallet_count");

    private readonly PopupManager popupManager;
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly ProtectedStringDataCache protectedStringDataCache;

    private ProtectedString[] addresses;

    private Action onWalletCreated;

    public WalletCreator(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    public void CreateWallet(string mnemonic, Action walletCreateFinished, Action<ProtectedString[]> onAddressesLoaded)
    {
        SetupActions(walletCreateFinished, onAddressesLoaded);
        StartLoadingPopup();
        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            CreateWalletCountPref();
            TryMnemonic(mnemonic, pass.Value);
        }
    }

    private void StartLoadingPopup()
    {
        popupManager.GetPopup<LoadingPopup>().SetLoadingText(" wallet", "Encrypting");
    }

    private void SetupActions(Action walletCreateFinished, Action<ProtectedString[]> onAddressesLoaded)
    {
        onWalletCreated = () =>
        {
            popupManager.CloseActivePopup();
            onAddressesLoaded?.Invoke(addresses);
            walletCreateFinished?.Invoke();
        };
    }

    private void SetWalletPlayerPrefs(string[] hashLvls, string saltedPasswordHash, string encryptedPhrase)
    {
        const int currentWalletNum = /*SecurePlayerPrefs.GetInt(WALLET_NUM_PREF) + 1*/ 1;

        SecurePlayerPrefs.SetInt(WALLET_NUM_PREF, currentWalletNum);
        SecurePlayerPrefs.SetString(PasswordEncryption.PWD_PREF_NAME + "_" + currentWalletNum, saltedPasswordHash);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum, encryptedPhrase);

        for (int i = 0; i < hashLvls.Length; i++)
            SecurePlayerPrefs.SetString("wallet_" + currentWalletNum + "_h" + (i + 1), hashLvls[i]);

        playerPrefPassword.SetupPlayerPrefs(currentWalletNum, onWalletCreated);
    }

    private void CreateWalletCountPref()
    {
        if (!SecurePlayerPrefs.HasKey(WALLET_NUM_PREF))
            SecurePlayerPrefs.SetInt(WALLET_NUM_PREF, 0);
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="mnemonic"> The phrase to attempt to create a wallet with. </param>
    /// <param name="basePass"> The password that was entered by the user. </param>
    private void TryMnemonic(string mnemonic, string basePass)
    {
        try
        {
            var wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));
            AsyncTaskScheduler.Schedule(() => GetAddresses(wallet));
            AsyncTaskScheduler.Schedule(() => EncryptWalletData(mnemonic, basePass));
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that seed. Please try again."));
        }
    }

    private async Task EncryptWalletData(string mnemonic, string basePass)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(basePass).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.firstHalf.SplitHalf();
        var lvl34string = splitPass.secondHalf.SplitHalf();

        SecureRandom secureRandom = new SecureRandom();

        string h1 = await Task.Run(() => lvl12string.firstHalf.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h2 = await Task.Run(() => lvl12string.secondHalf.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h3 = await Task.Run(() => lvl34string.firstHalf.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h4 = await Task.Run(() => lvl34string.secondHalf.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string encryptedPhrase = await Task.Run(() => mnemonic.AESEncrypt(h1 + h2).Protect(h3 + h4)).ConfigureAwait(false);
        string saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(basePass)).ConfigureAwait(false);
        string[] encryptedHashLvls = await Task.Run(() => new string[] { h1.AESEncrypt(lvl12string.firstHalf.GetSHA512Hash()), h2.Protect(), h3.Protect(), h4.AESEncrypt(lvl34string.secondHalf.GetSHA512Hash()) }).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => SetWalletPlayerPrefs(encryptedHashLvls, saltedPasswordHash, encryptedPhrase));
    }

    private async Task GetAddresses(Wallet wallet)
    {
        addresses = await Task.Run(() => wallet.GetAddresses(50).Select(str => new ProtectedString(str)).ToArray()).ConfigureAwait(false);
    }
}
