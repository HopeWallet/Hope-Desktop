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
        PlayerPrefPassword playerPrefPassword,
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
    /// <param name="encryptedHashes"> The hashes used to encrypt the wallet. </param>
    /// <param name="saltedPasswordHash"> The pbkdf2 salted password hash. </param>
    /// <param name="encryptedSeed"> The encrypted wallet seed. </param>
    private void FinalizeWalletCreation(string[] encryptedHashes, string saltedPasswordHash, string encryptedSeed)
    {
        int walletNum = SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1;
        dynamicDataCache.SetData("walletnum", walletNum);

        userWalletInfoManager.AddWalletInfo(dynamicDataCache.GetData("name"), addresses);

        SecurePlayerPrefs.SetString(walletSettings.walletDerivationPrefName + walletNum, dynamicDataCache.GetData("path"));
        SecurePlayerPrefs.SetString(walletSettings.walletPasswordPrefName + walletNum, saltedPasswordHash);
        SecurePlayerPrefs.SetString(walletSettings.walletDataPrefName + walletNum, encryptedSeed);
        SecurePlayerPrefs.SetString(walletSettings.walletNamePrefName + walletNum, dynamicDataCache.GetData("name"));

        for (int i = 0; i < encryptedHashes.Length; i++)
            SecurePlayerPrefs.SetString(walletNum + walletSettings.walletHashLvlPrefName + (i + 1), encryptedHashes[i]);

        playerPrefPassword.SetupPlayerPrefs(walletNum, onWalletLoaded);

        ((byte[])dynamicDataCache.GetData("seed")).ClearBytes();
        dynamicDataCache.SetData("seed", null);
        dynamicDataCache.SetData("path", null);
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
    /// <param name="basePass"> The password that was entered by the user. </param>
    private void TryCredentials(string basePass)
    {
        if (string.IsNullOrEmpty(basePass) || basePass.Length < 8)
        {
            ExceptionManager.DisplayException(new Exception("Invalid wallet password. Please use a password with more than 8 characters!"));
            return;
        }

        try
        {
            Wallet wallet = new Wallet((byte[])dynamicDataCache.GetData("seed"), (string)dynamicDataCache.GetData("path"));
            string[] addresses = wallet.GetAddresses(50);

            AssignAddresses(addresses);
            walletEncryptor.EncryptWallet(wallet.Seed, basePass, SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) + 1, FinalizeWalletCreation);
        }
        catch (Exception e)
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again. => " + e.Message));
        }
    }
}