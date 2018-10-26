using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which allows the user to choose to open a wallet via Ledger or the built in Hope wallet.
/// </summary>
public sealed class ChooseWalletMenu : Menu<ChooseWalletMenu>
{
    public static event Action OnAppLoaded;

	[SerializeField] private Button ledgerButton, trezorButton, hopeButton;

    private UserWalletManager userWalletManager;
    private HopeWalletInfoManager walletInfoManager;

    /// <summary>
    /// Adds the required wallet dependencies.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="walletInfoManager"> The avtive HopeWalletInfoManager. </param>
    [Inject]
    public void Construct(UserWalletManager userWalletManager, HopeWalletInfoManager walletInfoManager)
    {
        this.userWalletManager = userWalletManager;
        this.walletInfoManager = walletInfoManager;
    }

    /// <summary>
    /// Adds the button listeners on start.
    /// </summary>
    private void Start()
    {
        ledgerButton.onClick.AddListener(OpenLedgerWallet);
		trezorButton.onClick.AddListener(OpenTrezorWallet);
		hopeButton.onClick.AddListener(OpenHopeWallet);
    }

    private void OnEnable()
    {
        OnAppLoaded?.Invoke();
    }

    /// <summary>
    /// Opens the Hope wallet.
    /// </summary>
    private void OpenHopeWallet()
    {
        userWalletManager.SetWalletType(UserWalletManager.WalletType.Hope);

        if (walletInfoManager.WalletCount > 0)
            uiManager.OpenMenu<WalletListMenu>();
        else
            uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// Opens the Ledger wallet.
    /// </summary>
    private void OpenLedgerWallet()
    {
        userWalletManager.SetWalletType(UserWalletManager.WalletType.Ledger);

        uiManager.OpenMenu<OpenLedgerWalletMenu>();
    }

	/// <summary>
	/// Opens the Trezor wallet.
	/// </summary>
	private void OpenTrezorWallet()
	{
        userWalletManager.SetWalletType(UserWalletManager.WalletType.Trezor);

        uiManager.OpenMenu<OpenTrezorWalletMenu>();
	}
}
