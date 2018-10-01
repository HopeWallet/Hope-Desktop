using TMPro;
using UnityEngine;

/// <summary>
/// Class used for displaying the PRPS release amounts on the ConfirmReleasePopup
/// </summary>
public sealed class ConfirmReleasePopup : ConfirmTransactionPopupBase<ConfirmReleasePopup>
{
	[SerializeField] private TextMeshProUGUI questionText;

	// If user does not have enough ETH to release purpose, set the questionText
	// to "Not enough ETH" and change the text colour to UIColors.Red and
	// set the okButton.interactable = false;

	/// <summary>
	/// Displays the asset transfer request details.
	/// </summary>
	/// <param name="transactionInput"> The input of the send asset transaction request. </param>
	protected override void InternalSetConfirmationValues(object[] transactionInput)
	{

	}
}
