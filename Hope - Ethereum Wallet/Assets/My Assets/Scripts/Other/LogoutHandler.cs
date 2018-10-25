using System;
/// <summary>
/// Class which manages logging out of the wallet.
/// </summary>
public sealed class LogoutHandler
{
    private readonly UIManager uiManager;
	private readonly PopupManager popupManager;
    private readonly DisposableComponentManager disposableComponentManager;
    private readonly DynamicDataCache dynamicDataCache;

	/// <summary>
	/// Initializes the LogoutHandler.
	/// </summary>
	/// <param name="uiManager"> The active UIManager component. </param>
	/// <param name="popupManager"> The active PopupManager. </param>
	/// <param name="disposableComponentManager"> The active DisposableComponentManager. </param>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	public LogoutHandler(
        UIManager uiManager,
		PopupManager popupManager,
        DisposableComponentManager disposableComponentManager,
        DynamicDataCache dynamicDataCache)
    {
        this.uiManager = uiManager;
		this.popupManager = popupManager;
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
		popupManager.CloseAllPopups();

        uiManager.OpenMenu<ChooseWalletMenu>();
    }
}