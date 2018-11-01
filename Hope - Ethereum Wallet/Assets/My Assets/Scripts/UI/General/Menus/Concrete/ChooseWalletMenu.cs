using System;
using TMPro;
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
	[SerializeField] private TMP_Dropdown networkSettingDropdown;

	private UserWalletManager userWalletManager;
	private HopeWalletInfoManager walletInfoManager;
	private EthereumNetworkManager.Settings ethereumNetworkSettings;

	/// <summary>
	/// Adds the required wallet dependencies.
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="walletInfoManager"> The avtive HopeWalletInfoManager. </param>
	/// <param name="ethereumNetworkSettings"> The avtive EthereumNetworkManager.Settings. </param>
	[Inject]
	public void Construct(
		UserWalletManager userWalletManager,
		HopeWalletInfoManager walletInfoManager,
		EthereumNetworkManager.Settings ethereumNetworkSettings)
	{
		this.userWalletManager = userWalletManager;
		this.walletInfoManager = walletInfoManager;
		this.ethereumNetworkSettings = ethereumNetworkSettings;

		networkSettingDropdown.value = ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? 0 : 1;
	}

	/// <summary>
	/// Adds the button listeners on start.
	/// </summary>
	private void Start()
	{
		ledgerButton.onClick.AddListener(OpenLedgerWallet);
		trezorButton.onClick.AddListener(OpenTrezorWallet);
		hopeButton.onClick.AddListener(OpenHopeWallet);
		networkSettingDropdown.onValueChanged.AddListener((value) => ethereumNetworkSettings.ChangeNetwork(value == 0 ? EthereumNetworkManager.NetworkType.Mainnet : EthereumNetworkManager.NetworkType.Rinkeby));
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
