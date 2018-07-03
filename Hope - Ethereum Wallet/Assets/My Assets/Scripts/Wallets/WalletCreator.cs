using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using System;
using System.Linq;
using System.Threading.Tasks;

public class WalletCreator
{
    private static readonly string WALLET_NUM_PREF = HashGenerator.GetSHA512Hash("wallet_count");

    private readonly PopupManager popupManager;
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly ProtectedStringDataCache protectedStringDataCache;

    private Action onWalletCreated;

    public WalletCreator(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    public void CreateWallet(string mnemonic, Action walletCreateFinished)
    {
        SetupWalletCreatedCallback(walletCreateFinished);
        StartLoadingPopup();
        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            CreateWalletCountPref();
            StartWalletEncryption(pass.Value, mnemonic);
        }
    }

    private void StartLoadingPopup()
    {
        popupManager.GetPopup<LoadingPopup>().SetLoadingText(" wallet", "Encrypting");
    }

    private void SetupWalletCreatedCallback(Action walletCreateFinished)
    {
        onWalletCreated = () =>
        {
            popupManager.CloseActivePopup();
            walletCreateFinished?.Invoke();
        };
    }

    private void StartWalletEncryption(string basePass, string mnemonic)
    {
        TryCreateWallet(mnemonic, () => EncryptWalletData(mnemonic, basePass));
    }

    private async void EncryptWalletData(string mnemonic, string basePass)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(basePass).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.firstHalf.SplitHalf();
        var lvl34string = splitPass.secondHalf.SplitHalf();

        string[] hashLvls = new string[4];
        hashLvls[0] = await Task.Run(() => lvl12string.firstHalf.GetSHA512Hash()).ConfigureAwait(false);
        hashLvls[1] = await Task.Run(() => lvl12string.secondHalf.GetSHA512Hash()).ConfigureAwait(false);
        hashLvls[2] = await Task.Run(() => lvl34string.firstHalf.GetSHA512Hash()).ConfigureAwait(false);
        hashLvls[3] = await Task.Run(() => lvl34string.secondHalf.GetSHA512Hash()).ConfigureAwait(false);

        string combinedHashes = hashLvls[0] + hashLvls[1] + hashLvls[2] + hashLvls[3];

        string encryptedPhrase = await Task.Run(() => mnemonic.AESEncrypt(combinedHashes).Protect(combinedHashes)).ConfigureAwait(false);
        string saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(basePass)).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => SetWalletPlayerPrefs(hashLvls, saltedPasswordHash, encryptedPhrase));
    }

    private void SetWalletPlayerPrefs(string[] hashLvls, string saltedPasswordHash, string encryptedPhrase)
    {
        int currentWalletNum = /*SecurePlayerPrefs.GetInt(WALLET_NUM_PREF) + 1*/ 1;

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
    /// <param name="onWalletCreatedSuccessfully"> Action to call if the wallet was created successfully. </param>
    private void TryCreateWallet(string mnemonic, Action onWalletCreatedSuccessfully)
    {
        try
        {
            var wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));
            onWalletCreatedSuccessfully?.Invoke();
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that seed. Please try again."));
        }
    }
}
