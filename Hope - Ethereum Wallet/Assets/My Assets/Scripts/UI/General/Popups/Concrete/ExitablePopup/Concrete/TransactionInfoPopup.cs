using Hope.Utils.EthereumUtils;
using Nethereum.Util;
using TMPro;
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

	private string walletAddress, walletName;

    private TransactionInfo transactionInfo;

    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;
	private ContactsManager contactsManager;

	/// <summary>
	/// Injects the required dependencies.
	/// </summary>
	/// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
	/// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
	/// <param name="contactsManager"> The active ContactsManager. </param>
	[Inject]
    public void Construct(
		TradableAssetManager tradableAssetManager,
		TradableAssetImageManager tradableAssetImageManager,
		ContactsManager contactsManager,
		UserWalletManager userWalletManager,
		UserWalletInfoManager userWalletInfoManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
		this.contactsManager = contactsManager;
		walletAddress = userWalletManager.WalletAddress;
		walletName = userWalletInfoManager.GetWalletInfo(walletAddress).WalletName;
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
        valueText.text = StringUtils.LimitEnd(valSymbol + SolidityUtils.ConvertFromUInt(transactionInfo.Value, tradableAsset.AssetDecimals).ConvertDecimalToString(), 23, "...");
        fromAddress.text = transactionInfo.From;
        toAddress.text = transactionInfo.To;
		timestampText.text = DateTimeUtils.TimeStampToDateTime(transactionInfo.TimeStamp).GetFormattedDateString();
		gasUsedText.text = transactionInfo.GasUsed.ToString();
        txCostText.text = (UnitConversion.Convert.FromWei(transactionInfo.GasPrice) * transactionInfo.GasUsed).ConvertDecimalToString() + " Ether";
		CheckIfContact(transactionInfo.From.ToLower(), fromAddressName);
		CheckIfContact(transactionInfo.To.ToLower(), toAddressName);

		TransactionUtils.CheckTransactionDetails(transactionInfo.TxHash, tx =>
        {
            gasPriceText.SetText(UnitConversion.Convert.FromWei(tx.GasPrice.Value, UnitConversion.EthUnit.Gwei) + " Gwei");
            gasLimitText.SetText(tx.Gas.Value.ToString());
        });
    }

	/// <summary>
	/// Checks if the address is also from a saved contact
	/// </summary>
	/// <param name="address"> The address string </param>
	/// <param name="nameTextObject"> The name text object </param>
	private void CheckIfContact(string address, TextMeshProUGUI nameTextObject)
	{
		if (contactsManager.ContactList.Contains(address))
			nameTextObject.text = "[ " + contactsManager.ContactList[address].name + " ]";

		else if (address.EqualsIgnoreCase(walletAddress))
			nameTextObject.text = "[ " + walletName + " ]";

		else
			nameTextObject.text = "";

		nameTextObject.gameObject.SetActive(!string.IsNullOrEmpty(nameTextObject.text));
	}
}