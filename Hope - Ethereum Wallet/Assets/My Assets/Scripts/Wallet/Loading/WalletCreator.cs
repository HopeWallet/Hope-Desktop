using Hope.Security.Encryption;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.Ethereum;
using Nethereum.HdWallet;
using System;

public sealed class WalletCreator : WalletLoaderBase
{
    private readonly WalletEncryptor walletEncryptor;
    private readonly UserWalletInfoManager.Settings walletSettings;

    private string derivationPath;

    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        UserWalletInfoManager.Settings walletSettings,
        UserWalletInfoManager userWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, userWalletInfoManager)
    {
        this.walletSettings = walletSettings;

        walletEncryptor = new WalletEncryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    [SecureCaller]
    protected override void LoadWallet(string userPass)
    {
        CreateWalletCountPref();
        TryCredentials(userPass);
    }

    protected override void SetupPopup()
    {
        popupManager.GetPopup<LoadingPopup>(true).Text = "Creating wallet";
    }

    private void SetWalletPlayerPrefs(string[] encryptedHashes, string saltedPasswordHash, string encryptedSeed)
    {
        int walletNum = SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1;
        dynamicDataCache.SetData("walletnum", walletNum);

        userWalletInfoManager.AddWalletInfo(dynamicDataCache.GetData("name"), addresses);

        SecurePlayerPrefs.SetString(walletSettings.walletDerivationPrefName + walletNum, derivationPath);
        SecurePlayerPrefs.SetString(walletSettings.walletPasswordPrefName + walletNum, saltedPasswordHash);
        SecurePlayerPrefs.SetString(walletSettings.walletDataPrefName + walletNum, encryptedSeed);
        SecurePlayerPrefs.SetString(walletSettings.walletNamePrefName + walletNum, dynamicDataCache.GetData("name"));

        for (int i = 0; i < encryptedHashes.Length; i++)
            SecurePlayerPrefs.SetString(walletNum + walletSettings.walletHashLvlPrefName + (i + 1), encryptedHashes[i]);

        playerPrefPassword.SetupPlayerPrefs(walletNum, onWalletLoaded);
    }

    private void CreateWalletCountPref()
    {
        if (!SecurePlayerPrefs.HasKey(walletSettings.walletCountPrefName))
            SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, 0);
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="basePass"> The password that was entered by the user. </param>
    [SecureCaller]
    private void TryCredentials(string basePass)
    {
        if (string.IsNullOrEmpty(basePass) || basePass.Length < 8)
        {
            ExceptionManager.DisplayException(new Exception("Invalid wallet password. Please use a password with more than 8 characters!"));
            return;
        }

        using (var mnemonic = (dynamicDataCache.GetData("mnemonic") as ProtectedString)?.CreateDisposableData())
        {
            try
            {
                derivationPath = WalletUtils.DetermineCorrectPath(mnemonic.Value);

                var wallet = new Wallet(mnemonic.Value, null, derivationPath);
                var addresses = wallet.GetAddresses(50);

                AssignAddresses(addresses);
                walletEncryptor.EncryptWallet(wallet.Seed, basePass, SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1, SetWalletPlayerPrefs);
            }
            catch (Exception e)
            {
                dynamicDataCache.SetData("mnemonic", null);
                ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again. => " + e.Message));
            }
        }
    }
}