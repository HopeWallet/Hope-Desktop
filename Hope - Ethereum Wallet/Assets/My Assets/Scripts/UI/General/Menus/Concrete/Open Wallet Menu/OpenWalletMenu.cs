using Hope.Utils.Ethereum;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
	[SerializeField] private GameObject lockPurposeSection,
										lockPurposeNotificationSection,
										pagesSection;

	[SerializeField] private TMP_Text walletNameText,
									  walletAccountText,
									  assetText,
									  balanceText,
									  netWorthText,
									  lockPrpsNotificationText;

	[SerializeField] private Image assetImage;

	[SerializeField] private IconButtons transactionTabs;

	[SerializeField] private Transform pendingTransactionSection;
	[SerializeField] private Button hopeLogo, ledgerLogo, trezorLogo;

    private EthereumNetworkManager.Settings ethereumNetworkSettings;
    private EthereumTransactionManager ethereumTransactionManager;
    private EthereumPendingTransactionManager ethereumPendingTransactionManager;
    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetPriceManager tradableAssetPriceManager;
    private TradableAssetNotificationManager notificationManager;
    private LockedPRPSManager lockedPrpsManager;
    private PRPS prpsContract;
    private CurrencyManager currencyManager;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private UserWalletManager userWalletManager;
	private LogoutHandler logoutHandler;

    private IdleTimeoutManager idleTimeoutManager;
	private PendingTransactionManager pendingTransactionManager;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 54;

	[Inject]
	public void Construct(
		EthereumTransactionManager ethereumTransactionManager,
		EthereumNetworkManager.Settings ethereumNetworkSettings,
        EthereumPendingTransactionManager ethereumPendingTransactionManager,
        TokenContractManager tokenContractManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetPriceManager tradableAssetPriceManager,
        TradableAssetNotificationManager notificationManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract,
        CurrencyManager currencyManager,
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
		LogoutHandler logoutHandler)
    {
        this.ethereumNetworkSettings = ethereumNetworkSettings;
        this.ethereumTransactionManager = ethereumTransactionManager;
        this.ethereumPendingTransactionManager = ethereumPendingTransactionManager;
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetPriceManager = tradableAssetPriceManager;
        this.notificationManager = notificationManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;
        this.currencyManager = currencyManager;
        this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.userWalletManager = userWalletManager;
		this.logoutHandler = logoutHandler;
    }

	private void Update()
	{
		if (pendingTransactionManager.PendingTransactionSectionOpen && !pendingTransactionManager.PendingTransaction && Input.GetKeyDown(KeyCode.Escape) && !popupManager.ActivePopupExists)
			pendingTransactionManager.AnimatePendingTransactionSection(false);
	}

	/// <summary>
	/// Starts the token load, sets up the wallet name text, and starts the IdleTimeoutManager.
	/// </summary>
	private void OnEnable()
    {
        tokenContractManager.StartTokenLoad(OpenMenu);

        UpdateWalletName();

        if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
        {
            SetUpWalletType(hopeLogo);
            idleTimeoutManager = new IdleTimeoutManager(uiManager, popupManager);
        }
        else if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Ledger)
        {
            SetUpWalletType(ledgerLogo);
        }
        else
        {
            SetUpWalletType(trezorLogo);
        }

        UpdateAssetUI();
        AccountChanged(userWalletManager.AccountNumber);
        ReloadNetWorth();
    }

    /// <summary>
    /// Updates the wallet name text.
    /// </summary>
    private void UpdateWalletName()
    {
        walletNameText.text = userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope
            ? hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName
            : userWalletManager.ActiveWalletType.ToString();
    }

    /// <summary>
    /// Sets up the corresponding logo with the active wallet type
    /// </summary>
    /// <param name="walletLogo"> The corresponding wallet logo </param>
    private void SetUpWalletType(Button walletLogo)
	{
		walletLogo.gameObject.SetActive(true);
		pendingTransactionManager = new PendingTransactionManager(ethereumNetworkSettings, ethereumPendingTransactionManager, userWalletManager, pendingTransactionSection, walletLogo);
	}

    /// <summary>
    /// Resets the visuals of the OpenWalletMenu.
    /// </summary>
    private void OnDisable()
    {
        if (pendingTransactionManager.PendingTransactionSectionOpen)
            pendingTransactionManager.AnimatePendingTransactionSection(false);

		hopeLogo.gameObject.SetActive(false);
		ledgerLogo.gameObject.SetActive(false);
		trezorLogo.gameObject.SetActive(false);

		idleTimeoutManager?.Stop();
        pendingTransactionManager.Reset();

        balanceText.text = "________\n\n";
        netWorthText.text = "_____\n\n";

        AccountChanged(0);
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        AccountsPopup.OnAccountChanged += AccountChanged;
        SettingsPopup.WalletNameSection.OnWalletNameChanged += UpdateWalletName;
        tradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        lockedPrpsManager.OnLockedPRPSUpdated += UpdateAssetUI;

        new PriceManager(currencyManager, tradableAssetPriceManager, tradableAssetManager, netWorthText);
        new TransactionTabManager(tradableAssetManager, ethereumTransactionManager, transactionTabs);
        new TransactionPageManager(tradableAssetManager, ethereumTransactionManager, pagesSection);

        walletAccountText.GetComponent<Button>().onClick.AddListener(() => popupManager.GetPopup<AccountsPopup>().OnPopupClose(walletAccountText.GetComponent<TextButton>().ResetButton));
    }

	/// <summary>
	/// Reloads the net worth of the current account.
	/// </summary>
	private void ReloadNetWorth()
    {
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;

        if (tradableAsset == null)
            return;

        var netWorth = tradableAssetPriceManager.GetPrice(tradableAsset.AssetSymbol) * tradableAsset.AssetBalance;

        if (netWorth == null)
            return;

        netWorthText.text = currencyManager.GetCurrencyFormattedValue(netWorth);
    }

    /// <summary>
    /// Called when the account is changed.
    /// </summary>
    /// <param name="account"> The new account number. </param>
    private void AccountChanged(int account)
    {
        walletAccountText.text = "(Account <size=90%>#</size>" + (account + 1) + ")";
    }

    /// <summary>
    /// Called when the OpenWalletMenu is first opened.
    /// </summary>
    private void OpenMenu()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the ui for the newest TradableAsset.
    /// </summary>
    private void UpdateAssetUI()
    {
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;

        if (tradableAsset == null)
            return;

        string assetBalance = tradableAsset.AssetBalance?.ToString();
        string assetSymbol = tradableAsset.AssetSymbol;

        if (!string.IsNullOrEmpty(assetBalance))
            balanceText.text = assetBalance.LimitEnd(MAX_ASSET_BALANCE_LENGTH - (assetSymbol.Length + 1), "...") + $"<style=Symbol> {assetSymbol}</size>";

        if (!string.IsNullOrEmpty(assetBalance) && tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress) && lockedPrpsManager.UnfulfilledItems.Count > 0)
            balanceText.text = $"{balanceText.text} | {SolidityUtils.ConvertFromUInt(lockedPrpsManager.UnfulfilledItems.Select(item => item.Value).Aggregate((v1, v2) => v1 + v2), 18)}<style=Symbol> Locked</style>";

        lockPurposeSection.SetActive(tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress));

        assetText.text = tradableAsset.AssetName.LimitEnd(MAX_ASSET_NAME_LENGTH, "...");
        assetImage.sprite = tradableAsset.AssetImage;

        UpdateAssetNotifications();
    }

    /// <summary>
    /// Updates the notifications for locked purpose and saves the transaction count of the current asset.
    /// </summary>
    private void UpdateAssetNotifications()
    {
        var lockedPrpsCount = lockedPrpsManager.UnlockableItems.Count;
        lockPurposeNotificationSection.SetActive(lockedPrpsCount > 0);
        lockPrpsNotificationText.text = lockedPrpsCount.ToString();
        lockPrpsNotificationText.fontSize = lockedPrpsCount.ToString().Length > 1 ? 12 : 12.5f;

        if (tradableAssetManager.ActiveTradableAsset == null)
            return;

        notificationManager.SaveTransactionCount(tradableAssetManager.ActiveTradableAsset.AssetAddress);
    }
}