using TMPro;

/// <summary>
/// Class used as a general transaction confirmation when the action being performed is not very sensitive.
/// </summary>
public sealed class GeneralTransactionConfirmationPopup : ConfirmTransactionPopupBase<GeneralTransactionConfirmationPopup>
{
    public TMP_Text transactionText;

    /// <summary>
    /// Sets the transaction text to display what the confirmation popup is for.
    /// </summary>
    /// <param name="transactionInput"> The transaction input array containing the string to display. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput) => transactionText.text = transactionInput[0].ToString();
}