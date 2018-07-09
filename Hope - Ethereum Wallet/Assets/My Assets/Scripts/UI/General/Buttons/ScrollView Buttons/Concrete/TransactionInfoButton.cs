using Hope.Utils.EthereumUtils;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which represents the button which holds the transaction info.
/// </summary>
public class TransactionInfoButton : InfoButton<TransactionInfoButton, TransactionInfo>
{

    public TMP_Text amountText,
                    timeFromNowText,
                    addressText,
                    dateText;

    public Image assetImage;

    private PopupManager popupManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;

    /// <summary>
    /// Adds the required dependencies to this class.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="tradableAssetManager"> The current active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The current active TradableAssetImageManager. </param>
    [Inject]
    public void Construct(PopupManager popupManager, TradableAssetManager tradableAssetManager, TradableAssetImageManager tradableAssetImageManager)
    {
        this.popupManager = popupManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
    }

    /// <summary>
    /// Adds the display popup method to the button listener.
    /// </summary>
    protected override void OnAwake() => Button.onClick.AddListener(DisplayTransactionInfoPopup);

    /// <summary>
    /// Displays the transaction info popup.
    /// </summary>
    private void DisplayTransactionInfoPopup() => popupManager.GetPopup<TransactionInfoPopup>().SetTransactionInfo(ButtonInfo);

    /// <summary>
    /// Sets the info of a button based on the TransactionInfo object.
    /// </summary>
    /// <param name="info"> The information of the transaction. </param>
    /// <returns> Returns the button that had its values updated. </returns>
    protected override void OnValueUpdated(TransactionInfo info)
    {
        var tradableAsset = tradableAssetManager.GetTradableAsset(info.AssetAddress);
        SetAmount(info, tradableAsset);
        SetAddress(info);
        SetDate(info);
        SetTimeFromNow(info);
        SetImage(info, tradableAsset);
    }

    /// <summary>
    /// Sets the amount of the asset that was traded in this transaction.
    /// </summary>
    /// <param name="transaction"> The info of this transaction. </param>
    /// <param name="tradableAsset"> The asset that was traded. </param>
    private void SetAmount(TransactionInfo transaction, TradableAsset tradableAsset)
    {
        var send = transaction.Type == TransactionInfo.TransactionType.Send;
        var start = send ? "-" : "+";
        var amount = start + SolidityUtils.ConvertFromUInt(transaction.Value, tradableAsset.AssetDecimals);
        var symbol = tradableAsset.AssetSymbol;

        amountText.SetText(amount.LimitEnd(18 - symbol.Length, "...") + " " + symbol);
        amountText.color = send ? UIColors.Red : UIColors.Green;
    }

    /// <summary>
    /// Sets the sending/receiving address of this transaction to the text component.
    /// </summary>
    /// <param name="transaction"> The info of this transaction. </param>
    private void SetAddress(TransactionInfo transaction)
    {
        var address = transaction.Type == TransactionInfo.TransactionType.Send ? transaction.To : transaction.From;
        addressText.SetText(address.LimitEnd(6) + "..." + address.Substring(address.Length - 4, 4));
    }

    /// <summary>
    /// Sets the date this transaction took place.
    /// </summary>    
    /// <param name="transaction"> The info of this transaction. </param>
    private void SetDate(TransactionInfo transaction)
    {
        dateText.SetText(DateTimeUtils.TimeStampToDateTime(transaction.TimeStamp).GetStringFormattedDate());
    }

    /// <summary>
    /// Sets how much time ago this transaction took place.
    /// </summary>
    /// <param name="transaction"> The info of this transaction. </param>
    private void SetTimeFromNow(TransactionInfo transaction)
    {
        timeFromNowText.SetText(DateTimeUtils.GetMaxTimeInterval(DateTimeUtils.GetCurrentUnixTime() - transaction.TimeStamp, " ago"));
    }

    /// <summary>
    /// Sets the image of this transaction to the asset's image.
    /// </summary>
    /// <param name="transaction"> The info of this transaction. </param>
    /// <param name="tradableAsset"> The asset to use to set the image. </param>
    private void SetImage(TransactionInfo transaction, TradableAsset tradableAsset)
    {
        tradableAssetImageManager.LoadImage(tradableAsset.AssetSymbol, img => assetImage.sprite = img);
    }
}