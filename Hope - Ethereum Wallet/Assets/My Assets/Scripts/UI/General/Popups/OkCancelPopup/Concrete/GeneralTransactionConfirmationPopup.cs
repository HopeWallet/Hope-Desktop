using UnityEngine.UI;

/// <summary>
/// Class used as a general transaction confirmation when the action being performed is not very sensitive.
/// </summary>
public class GeneralTransactionConfirmationPopup : ConfirmTransactionRequestPopup<GeneralTransactionConfirmationPopup>
{

    public Text transactionText;

    /// <summary>
    /// Sets the transaction text to display what the confirmation popup is for.
    /// </summary>
    /// <param name="transactionInput"> The transaction input array containing the string to display. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput) => transactionText.text = transactionInput[0].ToString();

}