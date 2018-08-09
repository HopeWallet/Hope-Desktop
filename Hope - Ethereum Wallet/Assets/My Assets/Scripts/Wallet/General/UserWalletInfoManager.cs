/// <summary>
/// Class which manages the info for each user wallet.
/// </summary>
public sealed class UserWalletInfoManager
{
    private readonly SecurePlayerPrefList<WalletInfo> wallets;
    private readonly UserWalletManager.Settings walletSettings;

    /// <summary>
    /// Initializes the UserWalletInfoManager.
    /// </summary>
    /// <param name="walletSettings"> The settings of the UserWallet. </param>
    public UserWalletInfoManager(UserWalletManager.Settings walletSettings)
    {
        this.walletSettings = walletSettings;

        wallets = new SecurePlayerPrefList<WalletInfo>(walletSettings.walletInfoPrefName);
    }

    /// <summary>
    /// Gets the wallet info for a given wallet number.
    /// </summary>
    /// <param name="walletNum"> The number of the wallet to get info for. </param>
    /// <returns> The WalletInfo of the given wallet number. </returns>
    public WalletInfo GetWalletInfo(int walletNum) => wallets[walletNum - 1];

    /// <summary>
    /// Gets the wallet info for a given wallet address.
    /// </summary>
    /// <param name="address"> The address of the wallet to get the info for. </param>
    /// <returns> The WalletInfo of the given wallet address. </returns>
    public WalletInfo GetWalletInfo(string address) => wallets[address];

    /// <summary>
    /// Adds a new wallet to the list of WalletInfo.
    /// </summary>
    /// <param name="walletName"> The name of the wallet. </param>
    /// <param name="walletAddresses"> The array of addresses associated with the wallet. </param>
    public void AddWalletInfo(string walletName, string[] walletAddresses)
    {
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count + 1);
        wallets.Add(new WalletInfo(walletName, walletAddresses, wallets.Count));
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
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count);
    }

    /// <summary>
    /// Deletes a wallet from the list of WalletInfo.
    /// </summary>
    /// <param name="walletInfo"> The WalletInfo object to delete. </param>
    public void DeleteWalletInfo(WalletInfo walletInfo)
    {
        if (wallets.Remove(walletInfo))
            SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count);
    }
}