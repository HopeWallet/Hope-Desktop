using Nethereum.HdWallet;
using System;

/// <summary>
/// Class used to create a new hope wallet.
/// </summary>
public sealed class WalletCreator : WalletLoaderBase
{
    private readonly WalletEncryptor walletEncryptor;
    private readonly HopeWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Initializes the WalletCreator with all required references.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="playerPrefPassword"> The PlayerPrefPassword to use to encrypt the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager.Settings walletSettings,
        HopeWalletInfoManager userWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, userWalletInfoManager)
    {
        this.walletSettings = walletSettings;

        walletEncryptor = new WalletEncryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    /// <summary>
    /// Creates a new wallet given the password.
    /// </summary>
    /// <param name="userPass"> The password to encrypt the wallet with. </param>
    protected override void LoadWallet(string userPass)
    {
        CreateWalletCountPref();
        TryCredentials(userPass);
    }

    /// <summary>
    /// Method used to finalize the wallet creation by saving all the data to the SecurePlayerPrefs.
    /// </summary>
    /// <param name="encryptionHashes"> The hashes used to encrypt the wallet. </param>
    /// <param name="passwordHash"> The pbkdf2 salted password hash. </param>
    /// <param name="encryptedSeed"> The encrypted wallet seed. </param>
    private void FinalizeWalletCreation(string[] encryptionHashes, string passwordHash, string encryptedSeed)
    {
        int walletNum = SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1;
        dynamicDataCache.SetData("walletnum", walletNum);

        hopeWalletInfoManager.AddWalletInfo(dynamicDataCache.GetData("name"), addresses, encryptionHashes, encryptedSeed, passwordHash);

        SecurePlayerPrefs.SetString(walletSettings.walletPasswordPrefName + walletNum, passwordHash);
        SecurePlayerPrefs.SetString(walletSettings.walletDataPrefName + walletNum, encryptedSeed);
        SecurePlayerPrefs.SetString(walletSettings.walletNamePrefName + walletNum, dynamicDataCache.GetData("name"));

        for (int i = 0; i < encryptionHashes.Length; i++)
            SecurePlayerPrefs.SetString(walletNum + walletSettings.walletHashLvlPrefName + (i + 1), encryptionHashes[i]);

        playerPrefPassword.SetupPlayerPrefs(walletNum, onWalletLoaded);

        ((byte[])dynamicDataCache.GetData("seed")).ClearBytes();
        dynamicDataCache.SetData("seed", null);
    }

    /// <summary>
    /// Creates the player pref for the wallet count if it is not already created.
    /// </summary>
    private void CreateWalletCountPref()
    {
        if (!SecurePlayerPrefs.HasKey(walletSettings.walletCountPrefName))
            SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, 0);
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="password"> The password that was entered by the user. </param>
    private void TryCredentials(string password)
    {
        try
        {
            byte[] seed = (byte[])dynamicDataCache.GetData("seed");

            AssignAddresses(new Wallet(seed, Wallet.DEFAULT_PATH).GetAddresses(50), new Wallet(seed, Wallet.ELECTRUM_LEDGER_PATH).GetAddresses(50));
            walletEncryptor.EncryptWallet(seed, password, SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1, FinalizeWalletCreation);
        }
        catch (Exception e)
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again. => " + e.Message));
        }
    }
}