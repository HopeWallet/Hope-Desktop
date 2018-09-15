using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public sealed class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public static event Action<TabType> OnTabChanged;

    public GameObject lockPurposeSection,
                      lockPurposeNotificationSection;

    public TMP_Text assetText,
                    balanceText,
                    netWorthText,
                    lockPrpsNotificationText;

    public Image assetImage;

    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetPriceManager tradableAssetPriceManager;
    private TradableAssetNotificationManager notificationManager;
    private LockedPRPSManager lockedPrpsManager;
    private PRPS prpsContract;
    private CurrencyManager currencyManager;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 54;

    /// <summary>
    /// Injects the required dependency into this class.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetPriceManager">The active TradableAssetPriceManager. </param>
    /// <param name="notificationManager"> The active TradableAssetNotificationManager. </param>
    /// <param name="lockedPrpsManager"> The active LockedPRPSManager. </param>
    /// <param name="prpsContract"> The active PRPS contract. </param>
    /// <param name="uiSettings"> The ui settings. </param>
    /// <param name="currencyManager"> The active CurrencyManager. </param>
    [Inject]
    public void Construct(
        TokenContractManager tokenContractManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetPriceManager tradableAssetPriceManager,
        TradableAssetNotificationManager notificationManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract,
        UIManager.Settings uiSettings,
        CurrencyManager currencyManager)
    {
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetPriceManager = tradableAssetPriceManager;
        this.notificationManager = notificationManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;
        this.currencyManager = currencyManager;
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        TradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        lockedPrpsManager.OnLockedPRPSUpdated += UpdateAssetNotifications;
        tokenContractManager.StartTokenLoad(OpenMenu);

        new IdleTimeoutManager(uiManager);
    }

    /// <summary>
    /// Called when the OpenWalletMenu is first opened.
    /// </summary>
    private void OpenMenu() => transform.GetChild(0).gameObject.SetActive(true);

    /// <summary>
    /// Updates the ui for the newest TradableAsset.
    /// </summary>
    public void UpdateAssetUI()
    {
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;

        if (tradableAsset == null)
            return;

        string assetBalance = tradableAsset.AssetBalance?.ToString();
        string assetSymbol = tradableAsset.AssetSymbol;

        if (!string.IsNullOrEmpty(assetBalance))
            balanceText.text = assetBalance.LimitEnd(MAX_ASSET_BALANCE_LENGTH - (assetSymbol.Length + 1), "...") + "<size=60%> " + assetSymbol + " </size>";

        assetText.text = tradableAsset.AssetName.LimitEnd(MAX_ASSET_NAME_LENGTH, "...");

        lockPurposeSection.SetActive(tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress));
        netWorthText.gameObject.SetActive(tradableAsset.AssetBalance > 0);

        if (netWorthText.gameObject.activeInHierarchy)
        {
            decimal netWorth = tradableAssetPriceManager.GetPrice(tradableAsset.AssetSymbol) * tradableAsset.AssetBalance;
            netWorthText.text = "<size=90%>$ </size>" + netWorth.ToString("0.00") + " <size=60%>" + currencyManager.ActiveCurrency.ToString() + "</size>";
        }

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

    public enum TabType { All, Sent, Received }
}