using Hope.Utils.Ethereum;
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
	private Hodler hodler;
	private RestrictedAddressManager restrictedAddressManager;

	/// <summary>
	/// Injects the required dependencies.
	/// </summary>
	/// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
	/// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
	/// <param name="contactsManager"> The active ContactsManager. </param>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	/// <param name="userWalletInfoManager"> The active userWalletInfoManager. </param>
	/// <param name="hodler"> The active Hodler. </param>
	/// <param name="restrictedAddressManager"> The active RestrictedAddressManager. </param>
	[Inject]
    public void Construct(
		TradableAssetManager tradableAssetManager,
		TradableAssetImageManager tradableAssetImageManager,
		ContactsManager contactsManager,
		UserWalletManager userWalletManager,
		HopeWalletInfoManager userWalletInfoManager,
		Hodler hodler,
		RestrictedAddressManager restrictedAddressManager)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
		this.contactsManager = contactsManager;
		this.hodler = hodler;
		this.restrictedAddressManager = restrictedAddressManager;

		walletAddress = userWalletManager.GetWalletAddress();
		walletName = userWalletManager.ActiveWalletType == UserWalletManager.WalletType.Hope ? userWalletInfoManager.GetWalletInfo(walletAddress).WalletName : userWalletManager.ActiveWalletType.ToString();
    }

    /// <summary>
    /// Sets the transaction info of this popup.
    /// </summary>
    /// <param name="transactionInfo"> The transaction info for this popup to display. </param>
    public void SetTransactionInfo(TransactionInfo transactionInfo)
    {
        this.transactionInfo = transactionInfo;
    }

    /// <summary>
    /// Initializes the popup by setting up the popup info.
    /// </summary>
    protected override void OnStart()
    {
        AssignTransactionInfo();
    }

    private void OnDestroy()
    {
        TransactionInfoButton.popupClosed?.Invoke();
    }

    /// <summary>
    /// Assigns the transaction info to all elements in this popup.
    /// </summary>
    private void AssignTransactionInfo()
    {
        var sendTransaction = transactionInfo.Type == TransactionInfo.TransactionType.Send;
        var valSymbol = sendTransaction ? "-" : "+";
        var tradableAsset = tradableAssetManager.GetTradableAsset(transactionInfo.AssetAddress);

        assetImage.sprite = tradableAsset.AssetImage;

        valueText.color = sendTransaction ? UIColors.Red : UIColors.Green;

        transactionHash.text = transactionInfo.TxHash;
        valueText.text = StringUtils.LimitEnd(valSymbol + SolidityUtils.ConvertFromUInt(transactionInfo.Value, tradableAsset.AssetDecimals).ConvertDecimalToString(), 18, "...") + "<style=Symbol> " + tradableAsset.AssetSymbol + "</style>";

		fromAddress.text = transactionInfo.From;
        toAddress.text = transactionInfo.To;
		timestampText.text = DateTimeUtils.TimeStampToDateTime(transactionInfo.TimeStamp).GetFormattedDateString();
		gasUsedText.text = transactionInfo.GasUsed.ToString();
        txCostText.text = (UnitConversion.Convert.FromWei(transactionInfo.GasPrice) * transactionInfo.GasUsed).ConvertDecimalToString() + "<style=Symbol> Ether</style>";
		CheckIfContact(transactionInfo.From.ToLower(), fromAddressName);
		CheckIfContact(transactionInfo.To.ToLower(), toAddressName);

		TransactionUtils.GetTransactionDetails(transactionInfo.TxHash).OnSuccess(tx =>
        {
            gasPriceText.SetText(UnitConversion.Convert.FromWei(tx.GasPrice.Value, UnitConversion.EthUnit.Gwei) + "<style=Symbol> Gwei</style>");
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
		nameTextObject.text = string.Empty;
		address = address.ToLower();

		if (contactsManager.ContactList.Contains(address))
			nameTextObject.text = "<style=Contact>" + contactsManager.ContactList[address].ContactName + "</style>";
		else if (address.EqualsIgnoreCase(walletAddress))
			nameTextObject.text = "<style=Contact>" + walletName + "</style>";
		else if (address == hodler.ContractAddress)
			nameTextObject.text = "<style=Contact>PRPS Smart Contract</style>";

		nameTextObject.gameObject.SetActive(!string.IsNullOrEmpty(nameTextObject.text));
	}
}