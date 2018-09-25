/// <summary>
/// Class which manages logging out of the wallet.
/// </summary>
public sealed class LogoutHandler
{
    private readonly UIManager uiManager;
    private readonly DisposableComponentManager disposableComponentManager;
    private readonly DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Initializes the LogoutHandler.
    /// </summary>
    /// <param name="uiManager"> The active UIManager component. </param>
    /// <param name="disposableComponentManager"> The active DisposableComponentManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    public LogoutHandler(
        UIManager uiManager,
        DisposableComponentManager disposableComponentManager,
        DynamicDataCache dynamicDataCache)
    {
        this.uiManager = uiManager;
        this.disposableComponentManager = disposableComponentManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Disposes of all necessary components before switching to the ChooseWalletMenu.
    /// </summary>
    public void Logout()
    {
        dynamicDataCache.ClearAllData();
        disposableComponentManager.Dispose();

        uiManager.OpenMenu<ChooseWalletMenu>();
    }
}