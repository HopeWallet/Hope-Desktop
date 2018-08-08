using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class UserWalletInfoManager
{
    private readonly SecurePlayerPrefList<WalletInfo> walletInfo;
    private readonly UserWalletManager.Settings walletSettings;

    public UserWalletInfoManager(UserWalletManager.Settings walletSettings)
    {
        this.walletSettings = walletSettings;

        walletInfo = new SecurePlayerPrefList<WalletInfo>(walletSettings.walletInfoPrefName);
    }

    public WalletInfo GetWalletInfo(int walletNum) => walletInfo[walletNum - 1];

    public WalletInfo GetWalletInfo(string address) => walletInfo[address];

    public void AddWalletInfo(string walletName, string[] walletAddresses)
    {
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, walletInfo.Count + 1);
        walletInfo.Add(new WalletInfo(walletName, walletAddresses, walletInfo.Count));
    }

    public void DeleteWalletInfo(int walletNum)
    {
        walletInfo.RemoveAt(walletNum - 1);
        SecurePlayerPrefs.SetInt(walletSettings.walletCountPrefName, walletInfo.Count);
    }
}