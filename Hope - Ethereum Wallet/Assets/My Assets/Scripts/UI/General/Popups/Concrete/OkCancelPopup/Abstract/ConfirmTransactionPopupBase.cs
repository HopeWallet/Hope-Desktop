using Nethereum.Hex.HexTypes;
using System;

/// <summary>
/// Base class used for confirming transaction requests.
/// </summary>
/// <typeparam name="T"> The type of the request to confirm. </typeparam>
public abstract class ConfirmTransactionPopupBase<T> : OkCancelPopupComponent<T> where T : FactoryPopup<T>
{
    private Action onConfirmPressed;

    private HexBigInteger gasLimit,
                          gasPrice;

    /// <summary>
    /// Sets the transaction request values that need to be confirmed.
    /// </summary>
    /// <param name="onConfirmPressed"> Action to call if the transaction is confirmed to be sent through. </param>
    /// <param name="gasLimit"> The gas limit of the transaction request. </param>
    /// <param name="gasPrice"> The gas price of the transaction request. </param>
    /// <param name="transactionInput"> The transaction input of the request being called. </param>
    public void SetConfirmationValues(Action onConfirmPressed, HexBigInteger gasLimit, HexBigInteger gasPrice, params object[] transactionInput)
    {
        this.onConfirmPressed = onConfirmPressed;
        this.gasLimit = gasLimit;
        this.gasPrice = gasPrice;

        InternalSetConfirmationValues(transactionInput);
    }

	/// <summary>
	/// Passes the transaction input to the class inheriting this ConfirmTransactionRequestPopup so the confirmation can be displayed.
	/// </summary>
	/// <param name="transactionInput"> The transaction input to confirm. </param>
	protected abstract void InternalSetConfirmationValues(object[] transactionInput);

    /// <summary>
    /// Called when the confirm button is clicked, which executes the transfer of the asset.
    /// </summary>
    public override void OkButton()
    {
        onConfirmPressed?.Invoke();
        popupManager.CloseAllPopups();
    }
}