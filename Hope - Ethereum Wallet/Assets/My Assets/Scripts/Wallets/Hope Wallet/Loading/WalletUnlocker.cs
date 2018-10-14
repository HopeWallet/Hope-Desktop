using Hope.Random.Strings;
using Hope.Security.ProtectedTypes.Types;

/// <summary>
/// Class used for unlocking the hope wallet.
/// </summary>
public sealed class WalletUnlocker : WalletLoaderBase
{
    private readonly WalletDecryptor walletDecryptor;

    /// <summary>
    /// Initializes the WalletUnlocker with all required references.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="playerPrefPassword"> The PlayerPrefPassword used to encrypt the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager. </param>
    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager hopeWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, hopeWalletInfoManager)
    {
        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache);
    }

    /// <summary>
    /// Attempts to load the wallet given the password.
    /// </summary>
    /// <param name="password"> The password to attempt to unlock the wallet with. </param>
    protected override void LoadWallet(byte[] password)
    {
        var walletInfo = hopeWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum"));

        AssignAddresses(walletInfo.WalletAddresses[0], walletInfo.WalletAddresses[1]);

        (dynamicDataCache.GetData("pass") as ProtectedString)?.Dispose();
        dynamicDataCache.SetData("pass", new ProtectedString(RandomString.Fast.GetString(16), this));
        (dynamicDataCache.GetData("pass") as ProtectedString)?.SetValue(password);

        onWalletLoaded?.Invoke();
    }
}