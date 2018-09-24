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

    [SerializeField] private GeneralRadioButtons transactionTabs;

    public Image assetImage;

    private EthereumTransactionManager ethereumTransactionManager;
    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetPriceManager tradableAssetPriceManager;
    private TradableAssetNotificationManager notificationManager;
    private LockedPRPSManager lockedPrpsManager;
    private PRPS prpsContract;
    private CurrencyManager currencyManager;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private UserWalletManager userWalletManager;

    private IdleTimeoutManager idleTimeoutManager;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 54;

    [Inject]
    public void Construct(
        EthereumTransactionManager ethereumTransactionManager,
        TokenContractManager tokenContractManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetPriceManager tradableAssetPriceManager,
        TradableAssetNotificationManager notificationManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract,
        CurrencyManager currencyManager,
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
        PopupManager popupManager)
    {
        this.ethereumTransactionManager = ethereumTransactionManager;
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetPriceManager = tradableAssetPriceManager;
        this.notificationManager = notificationManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;
        this.currencyManager = currencyManager;
        this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.userWalletManager = userWalletManager;

        walletAccountText.GetComponent<Button>().onClick.AddListener
			(() => popupManager.GetPopup<AccountsPopup>().SetOnCloseAction(walletAccountText.GetComponent<TextButton>().PopupClosed));
    }

    private void OnEnable()
    {
        tokenContractManager.StartTokenLoad(OpenMenu);

        walletNameText.text = userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope
            ? hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName
            : userWalletManager.ActiveWalletType.ToString();

        if (userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope)
            idleTimeoutManager = new IdleTimeoutManager(uiManager);
    }

    private void OnDisable()
    {
        idleTimeoutManager?.Stop();

        balanceText.text = "________\n\n";
        netWorthText.text = "_____\n\n";
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        AccountsPopup.OnAccountChanged += AccountChanged;
        TradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        lockedPrpsManager.OnLockedPRPSUpdated += UpdateAssetNotifications;

        new PriceManager(currencyManager, tradableAssetPriceManager, tradableAssetManager, netWorthText);
        new TransactionTabManager(tradableAssetManager, ethereumTransactionManager, transactionTabs);
        new TransactionPageManager(tradableAssetManager, ethereumTransactionManager, pagesSection);
    }

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
            balanceText.text = assetBalance.LimitEnd(MAX_ASSET_BALANCE_LENGTH - (assetSymbol.Length + 1), "...") + "<style=Symbol> " + assetSymbol + " </size>";

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
        lockPrpsNotificationText.fontSize = lockedPrpsCount.ToString().Length > 1 ? 15 : 19;

        notificationManager.SaveTransactionCount(tradableAssetManager.ActiveTradableAsset.AssetAddress);
    }
}