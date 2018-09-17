using Hope.Security.ProtectedTypes.Types;
using System;

/// <summary>
/// Base class used for loading the wallet.
/// </summary>
public abstract class WalletLoaderBase : SecureObject
{
    protected readonly PopupManager popupManager;
    protected readonly PlayerPrefPasswordDerivation playerPrefPassword;
    protected readonly DynamicDataCache dynamicDataCache;
    protected readonly HopeWalletInfoManager userWalletInfoManager;

    protected Action onWalletLoaded;

    protected string[][] addresses;

    /// <summary>
    /// Initializes the WalletLoaderBase with all required references.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="playerPrefPassword"> The PlayerPrefPassword used to encrypt the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    protected WalletLoaderBase(
        PopupManager popupManager,
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager userWalletInfoManager)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
        this.userWalletInfoManager = userWalletInfoManager;
    }

    /// <summary>
    /// Loads the wallet given the output array of addresses and the Action to call once the wallet is loaded.
    /// </summary>
    /// <param name="addresses"> The output array of wallet addresses. </param>
    /// <param name="onWalletLoaded"> Action to call once the wallet is loaded. </param>
    [SecureCaller]
    public void Load(string[][] addresses, Action onWalletLoaded)
    {
        SetupLoadActions(onWalletLoaded);

        addresses[0] = new string[50];
        addresses[1] = new string[50];
        this.addresses = addresses;

        (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData().OnSuccess(disposableData => LoadWallet(disposableData.Value));
    }

    /// <summary>
    /// Sets up the actions which will be called once the wallet is loaded.
    /// </summary>
    /// <param name="onWalletLoaded"> Action to call once the wallet is loaded. </param>
    private void SetupLoadActions(Action onWalletLoaded)
    {
        this.onWalletLoaded = () =>
        {
            popupManager.CloseAllPopups();
            onWalletLoaded?.Invoke();

            GC.Collect(); // Collect any remnants of important data
        };
    }

    /// <summary>
    /// Assigns the addresses to the wallet.
    /// </summary>
    /// <param name="addressSetOne"> The first set of addresses to assign to the HopeWallet. </param>
    /// <param name="addressSetTwo"> The second set of addresses to assign to the HopeWallet. </param>
    protected void AssignAddresses(string[] addressSetOne, string[] addressSetTwo)
    {
        Array.Copy(addressSetOne, addresses[0], addressSetOne.Length);
        Array.Copy(addressSetTwo, addresses[1], addressSetTwo.Length);
    }

    /// <summary>
    /// Loads the wallet given the user password.
    /// </summary>
    /// <param name="userPass"> The password entered by the user. </param>
    protected abstract void LoadWallet(string userPass);
}