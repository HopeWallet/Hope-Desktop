public sealed class LogoutHandler
{
    private readonly UIManager uiManager;
    private readonly DisposableComponentManager disposableComponentManager;
    private readonly DynamicDataCache dynamicDataCache;

    public LogoutHandler(
        UIManager uiManager,
        DisposableComponentManager disposableComponentManager,
        DynamicDataCache dynamicDataCache)
    {
        this.uiManager = uiManager;
        this.disposableComponentManager = disposableComponentManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    public void Logout()
    {
        dynamicDataCache.ClearAllData();
        disposableComponentManager.Dispose();

        uiManager.OpenMenu<ChooseWalletMenu>();
    }
}