using TMPro;
using Zenject;

/// <summary>
/// Class used for displaying the different wallets in the WalletListMenu.
/// </summary>
public sealed class WalletButton : InfoButton<WalletButton, WalletInfo>
{
    public TMP_Text walletNameText;

    private PopupManager popupManager;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Injects required dependencies.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(PopupManager popupManager, DynamicDataCache dynamicDataCache)
    {
        this.popupManager = popupManager;
        this.dynamicDataCache = dynamicDataCache;
    }

	/// <summary>
	/// Adds the button click listener.
	/// </summary>
	protected override void OnAwake() => Button.onClick.AddListener(WalletButtonClicked);

	/// <summary>
	/// Updates the name of the wallet with the WalletInfo object.
	/// </summary>
	/// <param name="info"> The WalletInfo of this WalletButton. </param>
	protected override void OnValueUpdated(WalletInfo info) => walletNameText.text = info.WalletName?.LimitEnd(17, "...");

	/// <summary>
	/// Sets the wallet num in the data cache and opens the <see cref="UnlockWalletPopup"/>.
	/// </summary>
	private void WalletButtonClicked()
    {
        dynamicDataCache.SetData("walletnum", ButtonInfo.WalletNum);
        popupManager.GetPopup<UnlockWalletPopup>();
    }
}