using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmTransactionPopup : FactoryPopup<InfoPopup>
{

	[SerializeField] private TextMeshProUGUI walletName, fromAddress, contactName, toAddress, transaction, fee;
	[SerializeField] private Button confirmButton;

	private void OnStart() => confirmButton.onClick.AddListener(SendTransaction);

	/// <summary>
	/// Sets all the necessary text elements to the proper values
	/// </summary>
	/// <param name="fromAddressString"> The fromAddress string </param>
	/// <param name="toAddressString"> The toAddress string </param>
	/// <param name="transactionString"> The transaction string </param>
	/// <param name="feeString"> The fee string </param>
	/// <param name="walletNameString"> The walletName string </param>
	/// <param name="contactNameString"> The contactName string </param>
	private void SetTransactionDetails(string fromAddressString, string toAddressString, string transactionString, 
									   string feeString, string walletNameString, string contactNameString = "")
	{
		walletName.text = walletNameString;
		fromAddress.text = fromAddressString;
		contactName.text = contactNameString;
		toAddress.text = toAddressString;
		transaction.text = transactionString;
		fee.text = feeString;
	}

	/// <summary>
	/// Confirm button has been clicked and transaction is to be sent
	/// </summary>
	private void SendTransaction()
	{

	}
}
