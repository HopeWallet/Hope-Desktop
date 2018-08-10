using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which allows the user to choose to open a wallet via Ledger or the built in Hope wallet.
/// </summary>
public sealed class ChooseWalletMenu : Menu<ChooseWalletMenu>
{
    public Button ledgerButton;
    public Button hopeButton;
    public Button exitButton;

    private UserWalletInfoManager.Settings walletSettings;

    [Inject]
    public void Construct(UserWalletInfoManager.Settings walletSettings) => this.walletSettings = walletSettings;

    /// <summary>
    /// Adds the button listeners on start.
    /// </summary>
    private void Start()
    {
        ledgerButton.onClick.AddListener(OpenLedgerWallet);
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
        // TODO
    }

    /// <summary>
    /// Opens the ExitConfirmationPopup which allows the user to exit the wallet.
    /// </summary>
    protected override void OnBackPressed()
    {
		popupManager.GetPopup<ExitConfirmationPopup>();
    }
}
