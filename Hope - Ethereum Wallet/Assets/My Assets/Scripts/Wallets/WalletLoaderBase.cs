﻿using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using System;
using System.Linq;
using System.Threading.Tasks;

public abstract class WalletLoaderBase
{
    protected readonly PopupManager popupManager;
    protected readonly PlayerPrefPassword playerPrefPassword;
    protected readonly DynamicDataCache dynamicDataCache;

    protected Action onWalletLoaded;

    protected ProtectedString[] addresses;

    protected abstract string LoadingText { get; }

    protected WalletLoaderBase(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, DynamicDataCache dynamicDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    public void Load(out ProtectedString[] addresses, Action onWalletLoaded, Action setupAddressAction)
    {
        SetupAddresses(out addresses);
        SetupLoadActions(onWalletLoaded, setupAddressAction);
        SetupPopup();

        using (var pass = dynamicDataCache.GetData("pass").CreateDisposableData())
            LoadWallet(pass.Value);
    }

    private void SetupPopup()
    {
        popupManager.GetPopup<LoadingPopup>(true).Text = LoadingText;
    }

    private void SetupLoadActions(Action onWalletLoaded, Action setupAddressAction)
    {
        this.onWalletLoaded = () =>
        {
            popupManager.CloseAllPopups();
            setupAddressAction?.Invoke();
            onWalletLoaded?.Invoke();
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

    protected abstract void LoadWallet(string userPass);
}
