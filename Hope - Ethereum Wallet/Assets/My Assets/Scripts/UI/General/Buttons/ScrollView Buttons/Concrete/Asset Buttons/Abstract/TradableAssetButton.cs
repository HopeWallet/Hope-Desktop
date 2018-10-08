using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Base class used to manage the buttons for TradableAssets.
/// </summary>
/// <typeparam name="T"> The concrete TradableAssetButton type. </typeparam>
public abstract class TradableAssetButton<T> : InfoButton<T, TradableAsset>, ITradableAssetButton where T : InfoButton<T, TradableAsset>, ITradableAssetButton
{
    public GameObject notificationImageObj,
					  loadingTransactionsObj;

    public TMP_Text amountText,
                    symbolText,
                    notificationCountText;

    public Image assetImage;

    protected PopupManager popupManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetButtonManager tradableAssetButtonManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private EthereumTransactionButtonManager transactionButtonManager;
    private TradableAssetNotificationManager notificationManager;

    /// <summary>
    /// The display text for the symbol/name of the asset button.
    /// </summary>
    protected abstract string AssetDisplayText { get; }

    /// <summary>
    /// The display text for the balance of the asset button.
    /// </summary>
    protected abstract string AssetBalanceText { get; }

    /// <summary>
    /// Injects the required dependencies into this class.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager object. </param>
    /// <param name="tradableAssetButtonManager"> The active TradableAssetButtonManager object. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager object. </param>
    /// <param name="transactionButtonManager"> The active TransactionButtonManager object. </param>
    /// <param name="notificationManager"> The active TradableAssetNotificationManager. </param>
    [Inject]
    public void Construct(PopupManager popupManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetButtonManager tradableAssetButtonManager,
        TradableAssetImageManager tradableAssetImageManager,
        EthereumTransactionButtonManager transactionButtonManager,
        TradableAssetNotificationManager notificationManager)
    {
        this.popupManager = popupManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetButtonManager = tradableAssetButtonManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.transactionButtonManager = transactionButtonManager;
        this.notificationManager = notificationManager;

        notificationManager.OnNotificationsUpdated += UpdateAssetNotifications;
    }

    /// <summary>
    /// Removes the UpdateAssetNotifications method from the notification manager.
    /// </summary>
    private void OnDestroy()
    {
        notificationManager.OnNotificationsUpdated -= UpdateAssetNotifications;
        ButtonInfo.OnAssetBalanceChanged -= _ => UpdateButtonBalance();
    }

    /// <summary>
    /// Updates the visuals of the AssetButton to the new TradableAsset.
    /// </summary>
    /// <param name="info"> The TradableAsset to use to set the visuals of this button. </param>
    protected override void OnValueUpdated(TradableAsset info)
    {
        SubscribeBalanceEventListener();

        UpdateButtonBalance();
        UpdateButtonSymbol();
        UpdateButtonImage();
        UpdateButtonTransformOrder();
        UpdateAssetNotifications();
    }

    /// <summary>
    /// Resets the notifications on the button.
    /// </summary>
    public void ResetButtonNotifications()
    {
        loadingTransactionsObj.SetActive(true);
        notificationImageObj.SetActive(false);
    }

    /// <summary>
    /// Sets the current button to be the active button in the wallet.
    /// </summary>
    public override void ButtonLeftClicked()
    {
        if (tradableAssetManager.ActiveTradableAsset == ButtonInfo)
            return;

        tradableAssetManager.SetNewActiveAsset(ButtonInfo);
        tradableAssetButtonManager.EnableNewTokenButton(this);
        transactionButtonManager.ProcessNewAssetList();
    }

    /// <summary>
    /// Sets the balance text of the button.
    /// </summary>
    private void UpdateButtonBalance()
    {
        if (ButtonInfo.AssetBalance == null)
            return;

        amountText.text = AssetBalanceText;
    }

    /// <summary>
    /// Updates the notification count of this asset.
    /// </summary>
    private void UpdateAssetNotifications()
    {
        var notifications = notificationManager.GetAssetNotificationCount(ButtonInfo.AssetAddress);

        if (notifications.HasValue)
            loadingTransactionsObj.SetActive(false);

        notificationImageObj.SetActive(notifications.HasValue && notifications.Value > 0);
        notificationCountText.text = notifications.HasValue ? notifications.Value.ToString() : "0";
    }

    /// <summary>
    /// Adds the UpdateButtonBalance method callback to the TradableAsset.OnAssetBalanceChanged event.
    /// </summary>
    private void SubscribeBalanceEventListener() => ButtonInfo.OnAssetBalanceChanged += _ => UpdateButtonBalance();

    /// <summary>
    /// Updates the symbol of this button to reflect this button's TradableAsset.
    /// </summary>
    private void UpdateButtonSymbol() => symbolText.text = AssetDisplayText;

    /// <summary>
    /// Updates the image of this button.
    /// </summary>
    private void UpdateButtonImage() => assetImage.sprite = ButtonInfo.AssetImage;

    /// <summary>
    /// Updates the order of this transform to reflect the proper order of the AssetButtons.
    /// </summary>
    private void UpdateButtonTransformOrder() => transform.SetSiblingIndex(transform.parent.childCount - 2);
}