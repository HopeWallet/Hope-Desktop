using Hope.Utils.EthereumUtils;
using Nethereum.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which displays the info of a transaction as a popup.
/// </summary>
public class TransactionInfoPopup : ExitablePopupComponent<TransactionInfoPopup>
{

	public TMP_InputField transactionHash,
						  fromAddress,
						  toAddress;

	public TextMeshProUGUI valueText,
						   timestampText,
						   gasLimitText,
						   gasPriceText,
						   gasUsedText,
						   txCostText,
						   fromAddressName,
						   toAddressName;

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
	protected override void OnStart() => AssignTransactionInfo();

	/// <summary>
	/// Assigns the transaction info to all elements in this popup.
	/// </summary>
	private void AssignTransactionInfo()
    {
        var sendTransaction = transactionInfo.Type == TransactionInfo.TransactionType.Send;
        var valSymbol = sendTransaction ? "-" : "+";
        var tradableAsset = tradableAssetManager.GetTradableAsset(transactionInfo.AssetAddress);

        tradableAssetImageManager.LoadImage(tradableAsset.AssetSymbol, img => assetImage.sprite = img);

        valueText.color = sendTransaction ? UIColors.Red : UIColors.Green;

        transactionHash.text = transactionInfo.TxHash;
        valueText.text = StringUtils.LimitEnd(valSymbol + SolidityUtils.ConvertFromUInt(transactionInfo.Value, tradableAsset.AssetDecimals), 23, "...");
        fromAddress.text = transactionInfo.From;
        toAddress.text = transactionInfo.To;
        timestampText.text = DateTimeUtils.TimeStampToDateTime(transactionInfo.TimeStamp).ToString();
        gasUsedText.text = transactionInfo.GasUsed.ToString();
        txCostText.text = (UnitConversion.Convert.FromWei(transactionInfo.GasPrice) * transactionInfo.GasUsed) + " Ether";

        TransactionUtils.CheckTransactionDetails(transactionInfo.TxHash, tx =>
        {
            gasPriceText.SetText(UnitConversion.Convert.FromWei(tx.GasPrice.Value, UnitConversion.EthUnit.Gwei) + " Gwei");
            gasLimitText.SetText(tx.Gas.Value.ToString());
        });
    }

    /// <summary>
    /// Switches the detailed transaction info to active or inactive.
    /// </summary>
    /// <param name="active"> The new state of the detailed transaction options. </param>
    private void SwitchDetailedOptions(bool active)
    {
        if (transactionInfo.GasUsed > 0)
        {
            gasUsedText.transform.parent.gameObject.SetActive(active);
            txCostText.transform.parent.gameObject.SetActive(active);
        }

        gasLimitText.transform.parent.gameObject.SetActive(active);
        gasPriceText.transform.parent.gameObject.SetActive(active);
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, active ? 300f : 225f);
    }

}