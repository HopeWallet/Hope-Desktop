using Hope.Utils.Ethereum;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which represents the button which holds the transaction info.
/// </summary>
public sealed class TransactionInfoButton : InfoButton<TransactionInfoButton, TransactionInfo>
{
	public static Action popupClosed;

	[SerializeField]
	private TMP_Text amountText,
									  timeFromNowText,
									  addressText,
									  dateText,
									  statusText,
									  directionText;

	[SerializeField] private Image assetImage, circle;

	[SerializeField] private GameObject loadingLine, newText;

	private PopupManager popupManager;
	private TradableAssetManager tradableAssetManager;
	private TradableAssetImageManager tradableAssetImageManager;
	private ContactsManager contactsManager;
	private Hodler hodler;

	/// <summary>
	/// Adds the required dependencies to this class.
	/// </summary>
	/// <param name="popupManager"> The active PopupManager. </param>
	/// <param name="tradableAssetManager"> The current active TradableAssetManager. </param>
	/// <param name="tradableAssetImageManager"> The current active TradableAssetImageManager. </param>
	/// <param name="contactsManager"> The current active ContactsManager. </param>
	/// <param name="hodler"> The current active Hodler. </param>
	/// <param name="restrictedAddressManager"> The active RestrictedAddressManager. </param>
	[Inject]
	public void Construct(
		PopupManager popupManager,
		TradableAssetManager tradableAssetManager,
		TradableAssetImageManager tradableAssetImageManager,
		ContactsManager contactsManager,
		Hodler hodler)
	{
		this.popupManager = popupManager;
		this.tradableAssetManager = tradableAssetManager;
		this.tradableAssetImageManager = tradableAssetImageManager;
		this.contactsManager = contactsManager;
		this.hodler = hodler;
	}

	/// <summary>
	/// Adds the display popup method to the button listener.
	/// </summary>
	protected override void OnAwake()
	{
		Button.onClick.AddListener(DisplayTransactionInfoPopup);
	}

	/// <summary>
	/// Displays the transaction info popup.
	/// </summary>
	private void DisplayTransactionInfoPopup()
	{
		popupClosed = () => Button.interactable = true;
		Button.interactable = false;
		popupManager.GetPopup<TransactionInfoPopup>().SetTransactionInfo(ButtonInfo);
	}

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
		SetImage(tradableAsset);
		SetDirection(info);
	}

	/// <summary>
	/// Sets the direction label beside the date
	/// </summary>
	/// <param name="transaction"> The info of this transaction. </param>
	private void SetDirection(TransactionInfo transaction)
	{
		var sending = transaction.Type == TransactionInfo.TransactionType.Send;

		circle.color = sending ? UIColors.Red : UIColors.Green;
		statusText.text = sending ? "OUT" : "IN";
		directionText.text = sending ? "To:" : "From:";
		addressText.transform.localPosition = new Vector2(sending ? -210f : -185f, addressText.transform.localPosition.y);
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

		amountText.SetText(amount.LimitEnd(18 - symbol.Length, "...") + "<style=Symbol> " + symbol + "</style>");
		amountText.color = send ? UIColors.Red : UIColors.Green;
	}

	/// <summary>
	/// Sets the sending/receiving address of this transaction to the text component.
	/// </summary>
	/// <param name="transaction"> The info of this transaction. </param>
	private void SetAddress(TransactionInfo transaction)
	{
		var address = transaction.Type == TransactionInfo.TransactionType.Send ? transaction.To : transaction.From;

		string contactName = CheckIfSavedContact(address);
		addressText.SetText(string.IsNullOrEmpty(contactName) ? address.LimitEnd(10) + "...." + address.Substring(address.Length - 10, 10) : contactName);
	}

	/// <summary>
	/// Checks if the inputted address is from a saved contact.
	/// </summary>
	/// <param name="address"> The address in the input field. </param>
	private string CheckIfSavedContact(string address)
	{
		address = address.ToLower();

		if (address == hodler.ContractAddress)
			return "<style=Contact>PRPS Smart Contract</style>";

		return contactsManager.ContactList.Contains(address) ? "<style=Contact>" + contactsManager.ContactList[address].ContactName + "</style>" : string.Empty;
	}

	/// <summary>
	/// Sets the date this transaction took place.
	/// </summary>    
	/// <param name="transaction"> The info of this transaction. </param>
	private void SetDate(TransactionInfo transaction)
	{
		dateText.SetText(DateTimeUtils.TimeStampToDateTime(transaction.TimeStamp).GetStringFormattedDate(false));
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
	/// <param name="tradableAsset"> The asset to use to set the image. </param>
	private void SetImage(TradableAsset tradableAsset)
	{
		tradableAssetImageManager.LoadImage(tradableAsset.AssetSymbol, img => assetImage.sprite = img);
	}
}