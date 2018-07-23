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

    protected Action onWalletLoaded;

    protected ProtectedString[] addresses;

    protected WalletLoaderBase(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, DynamicDataCache dynamicDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    [SecureCaller]
    public void Load(out ProtectedString[] addresses, Action onWalletLoaded, Action setupAddressAction)
    {
        SetupAddresses(out addresses);
        SetupLoadActions(onWalletLoaded, setupAddressAction);
        SetupPopup();

        using (var pass = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData())
            LoadWallet(pass.Value);
    }

    private void SetupLoadActions(Action onWalletLoaded, Action setupAddressAction)
    {
        this.onWalletLoaded = () =>
        {
            popupManager.CloseAllPopups();
            setupAddressAction?.Invoke();
            onWalletLoaded?.Invoke();

            GC.Collect(); // Collect any remnants of important data
        };
    }

    private void SetupAddresses(out ProtectedString[] addresses)
    {
        addresses = new ProtectedString[50];
        this.addresses = addresses;
    }

    protected async Task GetAddresses(Wallet wallet)
    {
        var addressesToCopy = await Task.Run(() => wallet.GetAddresses(50).Select(str => new ProtectedString(str)).ToArray()).ConfigureAwait(false);
        Array.Copy(addressesToCopy, addresses, addresses.Length);
    }

    protected abstract void SetupPopup();

    [SecureCaller]
    protected abstract void LoadWallet(string userPass);
}
