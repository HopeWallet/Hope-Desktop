using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WalletLoaderBase
{

    protected readonly PopupManager popupManager;
    protected readonly PlayerPrefPassword playerPrefPassword;
    protected readonly ProtectedStringDataCache protectedStringDataCache;

    protected Action onWalletLoaded;

    protected ProtectedString[] addresses;

    protected abstract string LoadingText { get; }

    protected WalletLoaderBase(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    public void Load(object data, out ProtectedString[] addresses, Action onWalletLoaded)
    {
        SetupAddresses(out addresses);
        SetupLoadActions(onWalletLoaded);
        SetupPopup();

        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
            LoadWallet(data, pass.Value);
    }

    private void SetupPopup()
    {
        popupManager.GetPopup<LoadingPopup>().loadingText.text = LoadingText;
    }

    private void SetupLoadActions(Action onWalletLoaded)
    {
        this.onWalletLoaded = () => { popupManager.CloseActivePopup(); onWalletLoaded?.Invoke(); };
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

    protected abstract void LoadWallet(object data, string userPass);

}
