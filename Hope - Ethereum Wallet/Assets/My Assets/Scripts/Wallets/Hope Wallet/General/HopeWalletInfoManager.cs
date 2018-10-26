using System;
using System.Collections.Generic;

/// <summary>
/// Class which manages the info for each wallet.
/// </summary>
public sealed class HopeWalletInfoManager
{
    private readonly SecurePlayerPrefList<WalletInfo> wallets;
    private readonly Settings walletSettings;

    /// <summary>
    /// The current wallet count.
    /// </summary>
    public int WalletCount => wallets.Count;

    /// <summary>
    /// The current list of wallets.
    /// </summary>
    public List<WalletInfo> Wallets => wallets.ReadonlyList;

    /// <summary>
    /// Initializes the UserWalletInfoManager.
    /// </summary>
    /// <param name="walletSettings"> The settings of the UserWallet. </param>
    public HopeWalletInfoManager(Settings walletSettings)
    {
        this.walletSettings = walletSettings;

        wallets = new SecurePlayerPrefList<WalletInfo>(walletSettings.walletInfoPrefName);
    }

    /// <summary>
    /// Gets the wallet info for a given wallet number.
    /// </summary>
    /// <param name="walletNum"> The number of the wallet to get info for. </param>
    /// <returns> The WalletInfo of the given wallet number. </returns>
    public WalletInfo GetWalletInfo(int walletNum) => wallets.Count >= walletNum ? wallets[walletNum - 1] : new WalletInfo("", null, 0);

    /// <summary>
    /// Gets the wallet info for a given wallet address.
    /// </summary>
    /// <param name="address"> The address of the wallet to get the info for. </param>
    /// <returns> The WalletInfo of the given wallet address. </returns>
    public WalletInfo GetWalletInfo(string address) => !string.IsNullOrEmpty(address) && wallets.Contains(address) ? wallets[address] : new WalletInfo("", null, 0);

    /// <summary>
    /// Adds a new wallet to the list of WalletInfo.
    /// </summary>
    /// <param name="walletName"> The name of the wallet. </param>
    /// <param name="walletAddresses"> The array of addresses associated with the wallet. </param>
    /// <param name="encryptionHashes"> The encrypted hashes used to encrypt the seed of the wallet. </param>
    /// <param name="encryptedSeed"> The encrypted wallet seed. </param>
    /// <param name="passwordHash"> The pbkdf2 password hash of the password used to encrypt the wallet. </param>
    public void AddWalletInfo(string walletName, string[][] walletAddresses, string[] encryptionHashes, string encryptedSeed, string passwordHash)
    {
        var encryptedWalletData = new WalletInfo.EncryptedDataContainer(encryptionHashes, encryptedSeed, passwordHash);
        var walletInfo = new WalletInfo(encryptedWalletData, walletName, (string[][])walletAddresses.Clone(), wallets.Count + 1);

        wallets.Add(walletInfo);
    }

    /// <summary>
    /// Updates the info of the wallet given the address of the wallet.
    /// </summary>
    /// <param name="walletAddress"> An address of the wallet to update. </param>
    /// <param name="newWalletInfo"> The new wallet info. </param>
    public void UpdateWalletInfo(string walletAddress, WalletInfo newWalletInfo)
    {
        if (!wallets.Contains(walletAddress))
            return;

        wallets[walletAddress] = newWalletInfo;
    }

    /// <summary>
    /// Updates the info of the wallet given the wallet number.
    /// </summary>
    /// <param name="walletNum"> The wallet number to update. </param>
    /// <param name="newWalletInfo"> The new wallet info. </param>
    public void UpdateWalletInfo(int walletNum, WalletInfo newWalletInfo)
    {
        if (wallets.Count < walletNum)
            return;

        wallets[walletNum - 1] = newWalletInfo;
    }

    /// <summary>
    /// Deletes a wallet from the list of WalletInfo.
    /// </summary>
    /// <param name="walletNum"> The number of the WalletInfo to delete. </param>
    public void DeleteWalletInfo(int walletNum)
    {
        if (walletNum > wallets.Count)
            return;

        wallets.RemoveAt(walletNum - 1);
    }

    /// <summary>
    /// Deletes a wallet from the list of WalletInfo.
    /// </summary>
    /// <param name="walletInfo"> The WalletInfo object to delete. </param>
    public void DeleteWalletInfo(WalletInfo walletInfo)
    {
        wallets.Remove(walletInfo);
    }

    /// <summary>
    /// Class which holds general wallet settings.
    /// </summary>
    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string walletInfoPrefName;
    }
}