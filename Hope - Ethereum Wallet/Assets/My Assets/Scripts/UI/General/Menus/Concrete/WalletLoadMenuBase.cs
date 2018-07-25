
using Zenject;

/// <summary>
/// Base class for different GUI components used to unlock or create the wallet.
/// </summary>
/// <typeparam name="T"> The type of the class responsible for loading the wallet via create or unlock. </typeparam>
public abstract class WalletLoadMenuBase<T> : Menu<T> where T : Menu<T>
{

    protected UserWalletManager userWalletManager;

    /// <summary>
    /// Injects the UserWalletManager as this class's dependency.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(UserWalletManager userWalletManager) => this.userWalletManager = userWalletManager;

    /// <summary>
    /// Adds the OnWalletLoad method to the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    protected virtual void OnEnable() => UserWallet.OnWalletLoadSuccessful += OnWalletLoad;

    /// <summary>
    /// Removes the OnWalletLoad method from the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    protected virtual void OnDisable() => UserWallet.OnWalletLoadSuccessful -= OnWalletLoad;

    /// <summary>
    /// Enables the open wallet gui once the user wallet has been successfully loaded.
    /// </summary>
    private void OnWalletLoad() => uiManager.OpenMenu<OpenWalletMenu>();

    /// <summary>
    /// Loads the wallet.
    /// </summary>
    public abstract void LoadWallet();

}