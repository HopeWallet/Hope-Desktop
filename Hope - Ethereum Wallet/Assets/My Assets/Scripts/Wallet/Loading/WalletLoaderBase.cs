using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using System;
using System.Linq;
using System.Threading.Tasks;

public abstract class WalletLoaderBase : SecureObject
{
    protected readonly PopupManager popupManager;
    protected readonly PlayerPrefPassword playerPrefPassword;
    protected readonly DynamicDataCache dynamicDataCache;
    protected readonly UserWalletInfoManager userWalletInfoManager;

    protected Action onWalletLoaded;

    protected string[] addresses;

    protected WalletLoaderBase(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        UserWalletInfoManager userWalletInfoManager)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
        this.userWalletInfoManager = userWalletInfoManager;
    }

    [SecureCaller]
    public void Load(out string[] addresses, Action onWalletLoaded)
    {
        SetupAddresses(out addresses);
        SetupLoadActions(onWalletLoaded);
        SetupPopup();

        using (var pass = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData())
            LoadWallet(pass.Value);
    }

    private void SetupLoadActions(Action onWalletLoaded)
    {
        this.onWalletLoaded = () =>
        {
            popupManager.CloseAllPopups();
            onWalletLoaded?.Invoke();

            GC.Collect(); // Collect any remnants of important data
        };
    }

    private void SetupAddresses(out string[] addresses)
    {
        addresses = new string[50];
        this.addresses = addresses;
    }

    protected void AssignAddresses(string[] walletAddresses)
    {
        Array.Copy(walletAddresses, addresses, addresses.Length);
    }

    protected abstract void SetupPopup();

    [SecureCaller]
    protected abstract void LoadWallet(string userPass);
}