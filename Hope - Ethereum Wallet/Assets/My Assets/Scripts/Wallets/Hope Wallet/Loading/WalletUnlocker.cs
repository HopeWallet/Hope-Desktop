using Hope.Security.ProtectedTypes.Types;

/// <summary>
/// Class used for unlocking the hope wallet.
/// </summary>
public sealed class WalletUnlocker : WalletLoaderBase
{
    private readonly WalletDecryptor walletDecryptor;
    private readonly HopeWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Initializes the WalletUnlocker with all required references.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="playerPrefPassword"> The PlayerPrefPassword used to encrypt the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager.Settings walletSettings,
        HopeWalletInfoManager userWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, userWalletInfoManager)
    {
        this.walletSettings = walletSettings;
        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    /// <summary>
    /// Attempts to load the wallet given the password.
    /// </summary>
    /// <param name="userPass"> The password to attempt to unlock the wallet with. </param>
    protected override void LoadWallet(string userPass)
    {
        var walletInfo = hopeWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum"));

        AssignAddresses(walletInfo.WalletAddresses[0], walletInfo.WalletAddresses[1]);

        (dynamicDataCache.GetData("pass") as ProtectedString)?.Dispose();
        dynamicDataCache.SetData("pass", new ProtectedString(userPass, this));

        onWalletLoaded?.Invoke();
    }
}