using Hope.Security.Encryption;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using System;

public sealed class WalletCreator : WalletLoaderBase
{
    private readonly WalletEncryptor walletEncryptor;

    private string derivationPath;

    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache) : base(popupManager, playerPrefPassword, dynamicDataCache)
    {
        walletEncryptor = new WalletEncryptor(playerPrefPassword, dynamicDataCache);
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

    private void SetWalletPlayerPrefs(string[] encryptedHashLvls, string saltedPasswordHash, string encryptedSeed)
    {
        int walletNum = SecurePlayerPrefs.GetInt("wallet_count") + 1;
        dynamicDataCache.SetData("walletnum", walletNum);

        SecurePlayerPrefs.SetInt("wallet_count", walletNum);
        SecurePlayerPrefs.SetString("derivation_" + walletNum, derivationPath);
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
    [SecureCaller]
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
                derivationPath = WalletUtils.DetermineCorrectPath(mnemonic.Value);
                var wallet = new Wallet(mnemonic.Value, null, derivationPath);
                GetAddresses(wallet);
                walletEncryptor.EncryptWallet(wallet.Seed, basePass, SetWalletPlayerPrefs);
            }
            catch (Exception e)
            {
                dynamicDataCache.SetData("mnemonic", null);
                ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again. => " + e.Message));
            }
        }
    }
}