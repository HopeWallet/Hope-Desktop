using TMPro;
using UnityEngine;

/// <summary>
/// Class used for displaying the PRPS release amounts on the ConfirmReleasePopup
/// </summary>
public sealed class ConfirmReleasePopup : ConfirmTransactionPopupBase<ConfirmReleasePopup>
{
    [SerializeField] private TextMeshProUGUI questionText,
                                             releaseAmountText;

    /// <summary>
    /// Displays the asset transfer request details.
    /// </summary>
    /// <param name="transactionInput"> The input of the send asset transaction request. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput)
    {
        questionText.text = $"Are you sure you would like to release {transactionInput[0]} Purpose?";
        releaseAmountText.text = $"+{transactionInput[0]}";
    }
}
