using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class UserWalletInfoManager
{
    private readonly SecurePlayerPrefList<WalletInfoJson> walletInfo;

    public UserWalletInfoManager(UserWalletManager.Settings settings)
    {
        walletInfo = new SecurePlayerPrefList<WalletInfoJson>(settings.walletInfoPrefName);
    }
}