using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Class used for displaying the different wallets in the WalletListMenu.
/// </summary>
public sealed class WalletButton : InfoButton<WalletButton, WalletInfo>
{
	[SerializeField] private TMP_Text walletNameText;

	private WalletListMenu walletListMenu;
    private PopupManager popupManager;
    private DynamicDataCache dynamicDataCache;

	private string fullWalletName;

	private readonly Color PURE_WHITE = new Color(1f, 1f, 1f);

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
	protected override void OnAwake()
	{
		Button.onClick.AddListener(WalletButtonClicked);
		walletListMenu = transform.GetComponentInParent<WalletListMenu>();
	}

	/// <summary>
	/// Updates the name of the wallet with the WalletInfo object.
	/// </summary>
	/// <param name="info"> The WalletInfo of this WalletButton. </param>
	protected override void OnValueUpdated(WalletInfo info)
	{
		fullWalletName = info.WalletName;
		walletNameText.text = fullWalletName?.LimitEnd(20, "...");
	}

	/// <summary>
	/// Sets the wallet num in the data cache and opens the <see cref="UnlockWalletPopup"/>.
	/// </summary>
	private void WalletButtonClicked()
	{
		Button.interactable = false;
		walletNameText.gameObject.AnimateColor(PURE_WHITE, 0.15f);

		dynamicDataCache.SetData("walletnum", ButtonInfo.WalletNum);

        var popup = popupManager.GetPopup<UnlockWalletPopup>();
        popup.SetWalletInfo(fullWalletName);
        popup.OnPopupClose(() =>
        {
            Button.interactable = true;
            walletNameText.gameObject.AnimateColor(UIColors.White, 0.15f);
        });
	}
}