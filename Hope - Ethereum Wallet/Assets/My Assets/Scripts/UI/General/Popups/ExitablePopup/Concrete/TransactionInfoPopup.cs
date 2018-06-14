using Nethereum.Util;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which displays the info of a transaction as a popup.
/// </summary>
public class TransactionInfoPopup : ExitablePopupComponent<TransactionInfoPopup>
{

    public Text transactionInfoText,
                txHashText,
                valueText,
                sendingAddressText,
                receivingAddressText,
                timestampText,
                gasLimitText,
                gasPriceText,
                gasUsedText,
                txCostText;

    public Button copyTxHashButton,
                  copyFromAddressButton,
                  copyToAddressButton,
                  moreInfoButton;

    public Image assetImage;

    private TransactionInfo transactionInfo;

    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;

    /// <summary>
    /// Injects the required dependencies.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    [Inject]
    public void Construct(TradableAssetManager tradableAssetManager, TradableAssetImageManager tradableAssetImageManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
    }

    /// <summary>
    /// Sets the transaction info of this popup.
    /// </summary>
    /// <param name="transactionInfo"> The transaction info for this popup to display. </param>
    public void SetTransactionInfo(TransactionInfo transactionInfo) => this.transactionInfo = transactionInfo;

    /// <summary>
    /// Initializes the popup by setting up the popup info.
    /// </summary>
    protected override void OnStart()
    {
        AssignButtonCallbacks();
        AssignTransactionInfo();
    }

    private void AssignButtonCallbacks()
    {
        copyTxHashButton.onClick.AddListener(() => ClipboardUtils.CopyToClipboard(transactionInfo.TxHash));
        copyFromAddressButton.onClick.AddListener(() => ClipboardUtils.CopyToClipboard(transactionInfo.From));
        copyToAddressButton.onClick.AddListener(() => ClipboardUtils.CopyToClipboard(transactionInfo.To));
    }

    private void AssignTransactionInfo()
    {
        var sendTransaction = transactionInfo.Type == TransactionInfo.TransactionType.Send;
        var valSymbol = sendTransaction ? "-" : "+";
        var tradableAsset = tradableAssetManager.GetTradableAsset(transactionInfo.AssetAddress);

        tradableAssetImageManager.LoadImage(tradableAsset.AssetSymbol, img => assetImage.sprite = img);

        transactionInfoText.text = tradableAsset.AssetSymbol + " Transaction Info";
        txHashText.text = transactionInfo.TxHash.LimitEnd(46, "...");
        valueText.text = valSymbol + SolidityUtils.ConvertFromUInt(transactionInfo.Value, tradableAsset.AssetDecimals) + " " + tradableAsset.AssetSymbol;
        sendingAddressText.text = transactionInfo.From;
        receivingAddressText.text = transactionInfo.To;
        timestampText.text = DateTimeUtils.TimeStampToDateTime(transactionInfo.TimeStamp) + "";

        valueText.color = sendTransaction ? UIColors.Red : UIColors.Green;
        //UnityEngine.Debug.Log("TxHash: " + transactionInfo.TxHash);
        //UnityEngine.Debug.Log("Value: " + transactionInfo.Value);
        //UnityEngine.Debug.Log("From: " + transactionInfo.From);
        //UnityEngine.Debug.Log("To: " + transactionInfo.To);
        //UnityEngine.Debug.Log("Time: " + transactionInfo.TimeStamp);
        //UnityEngine.Debug.Log("Gas Price: " + transactionInfo.GasPrice);
        //UnityEngine.Debug.Log("Gas Used: " + transactionInfo.GasUsed);
        //TransactionUtils.CheckTransactionDetails(transactionInfo.TxHash, tx =>
        //{
        //    UnityEngine.Debug.Log("Gas Limit: " + tx.Gas.Value);
        //    UnityEngine.Debug.Log("Actual Tx Cost/Fee: " + (UnitConversion.Convert.FromWei(transactionInfo.GasPrice) * transactionInfo.GasUsed));
        //});
    }
}