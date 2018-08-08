public sealed class UserWalletInfoManager
{
    private readonly SecurePlayerPrefList<WalletInfo> wallets;
    private readonly UserWalletManager.Settings walletSettings;

    public UserWalletInfoManager(UserWalletManager.Settings walletSettings)
    {
        this.walletSettings = walletSettings;

        wallets = new SecurePlayerPrefList<WalletInfo>(walletSettings.walletInfoPrefName);
    }

    public WalletInfo GetWalletInfo(int walletNum) => wallets[walletNum - 1];

    public WalletInfo GetWalletInfo(string address) => wallets[address];

    public void AddWalletInfo(string walletName, string[] walletAddresses)
    {
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count + 1);
        wallets.Add(new WalletInfo(walletName, walletAddresses, wallets.Count));
    }

    public void DeleteWalletInfo(int walletNum)
    {
        if (walletNum > wallets.Count)
            return;

        wallets.RemoveAt(walletNum - 1);
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count);
    }

    public void DeleteWalletInfo(WalletInfo walletInfo)
    {
        if (wallets.Remove(walletInfo))
            SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, wallets.Count);
    }
}