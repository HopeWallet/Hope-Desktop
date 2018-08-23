using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the click events for an asset button.
/// </summary>
public class AssetButton : InfoButton<AssetButton, TradableAsset>
{
    public GameObject notificationImageObj,
                      loadingTransactionsObj;

    public TMP_Text amountText,
                    symbolText,
                    notificationCountText;

    public Image assetImage;

    private PopupManager popupManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetButtonManager tradableAssetButtonManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private EthereumTransactionButtonManager transactionButtonManager;
    private TradableAssetNotificationManager notificationManager;

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
    }

    /// <summary>
    /// Updates the visuals of the AssetButton to the new TradableAsset.
    /// </summary>
    /// <param name="info"> The TradableAsset to use to set the visuals of this button. </param>
    protected override void OnValueUpdated(TradableAsset info)
    {
        UpdateButtonBalance();
        UpdateButtonSymbol();
        UpdateButtonImage();
        UpdateButtonTransformOrder();
        UpdateAssetNotifications();
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
    public void UpdateButtonBalance()
    {
        string balanceText = ButtonInfo.AssetBalance + "";
        amountText.text = balanceText.LimitEnd(7, "...");
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
    /// Updates the symbol of this button to reflect this button's TradableAsset.
    /// </summary>
    private void UpdateButtonSymbol() => symbolText.text = ButtonInfo.AssetSymbol.LimitEnd(5, "...");

    /// <summary>
    /// Updates the image of this button.
    /// </summary>
    private void UpdateButtonImage() => tradableAssetImageManager.LoadImage(ButtonInfo.AssetSymbol, image => assetImage.sprite = image);

    /// <summary>
    /// Updates the order of this transform to reflect the proper order of the AssetButtons.
    /// </summary>
    private void UpdateButtonTransformOrder() => transform.SetSiblingIndex(transform.parent.childCount - 2);
}