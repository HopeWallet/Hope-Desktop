using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which allows the user to choose to open a wallet via Ledger or the built in Hope wallet.
/// </summary>
public sealed class ChooseWalletMenu : Menu<ChooseWalletMenu>
{
	[SerializeField] private Button ledgerButton, trezorButton, hopeButton;

    private HopeWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Adds the required wallet dependencies.
    /// </summary>
    /// <param name="walletSettings"> The pref settings for the UserWallet. </param>
    [Inject]
    public void Construct(HopeWalletInfoManager.Settings walletSettings) => this.walletSettings = walletSettings;

    /// <summary>
    /// Adds the button listeners on start.
    /// </summary>
    private void Start()
    {
        ledgerButton.onClick.AddListener(OpenLedgerWallet);
		trezorButton.onClick.AddListener(OpenTrezorWallet);
		hopeButton.onClick.AddListener(OpenHopeWallet);
    }

	/// <summary>
	/// Opens the Hope wallet.
	/// </summary>
	private void OpenHopeWallet()
    {
        if (SecurePlayerPrefs.HasKey(walletSettings.walletCountPrefName) && SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName) > 0)
            uiManager.OpenMenu<WalletListMenu>();
        else
            uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// Opens the Ledger wallet.
    /// </summary>
    private void OpenLedgerWallet()
    {
        uiManager.OpenMenu<OpenLedgerWalletMenu>();
    }

	/// <summary>
	/// Opens the Trezor wallet.
	/// </summary>
	private void OpenTrezorWallet()
	{
        uiManager.OpenMenu<OpenTrezorWalletMenu>();
	}

	/// <summary>
	/// Opens the ExitConfirmationPopup which allows the user to exit the wallet.
	/// </summary>
	protected override void OnBackPressed()
    {
		popupManager.GetPopup<ExitConfirmationPopup>();
    }
}
