﻿using Nethereum.HdWallet;
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
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager. </param>
    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager.Settings walletSettings,
        HopeWalletInfoManager hopeWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, hopeWalletInfoManager)
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
        byte[] seed = (byte[])dynamicDataCache.GetData("seed");

        AssignAddresses(new Wallet(seed, Wallet.DEFAULT_PATH).GetAddresses(50), new Wallet(seed, Wallet.ELECTRUM_LEDGER_PATH).GetAddresses(50));
        walletEncryptor.EncryptWallet(seed, userPass, hopeWalletInfoManager.WalletCount + 1, FinalizeWalletCreation);
    }

    /// <summary>
    /// Method used to finalize the wallet creation by saving all the data to the SecurePlayerPrefs.
    /// </summary>
    /// <param name="encryptionHashes"> The hashes used to encrypt the wallet. </param>
    /// <param name="passwordHash"> The pbkdf2 salted password hash. </param>
    /// <param name="encryptedSeed"> The encrypted wallet seed. </param>
    private void FinalizeWalletCreation(string[] encryptionHashes, string passwordHash, string encryptedSeed)
    {
        hopeWalletInfoManager.AddWalletInfo(dynamicDataCache.GetData("name"), addresses, encryptionHashes, encryptedSeed, passwordHash);
        playerPrefPassword.SetupPlayerPrefs(hopeWalletInfoManager.WalletCount, onWalletLoaded);

        ((byte[])dynamicDataCache.GetData("seed")).ClearBytes();
        dynamicDataCache.SetData("seed", null);
        dynamicDataCache.SetData("walletnum", hopeWalletInfoManager.WalletCount);
    }
}