using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Base class used for confirming transaction requests.
/// </summary>
/// <typeparam name="T"> The type of the request to confirm. </typeparam>
public abstract class ConfirmTransactionPopupBase<T> : OkCancelPopupComponent<T> where T : FactoryPopup<T>
{
	[SerializeField] private GameObject confirmText, exitButton;

	private Action onConfirmPressed;

    private BigInteger gasLimit,
                       gasPrice;

	protected UserWalletManager userWalletManager;

	/// <summary>
	/// Checks if the user is using a hardware wallet, and changes popup elements accordingly.
	/// </summary>
	/// <param name="userWalletManager"> The active UserWalletManager. </param>
	[Inject]
	public void Construct(UserWalletManager userWalletManager)
	{
		this.userWalletManager = userWalletManager;

		if (userWalletManager.ActiveWalletType != UserWalletManager.WalletType.Hope)
		{
			confirmText.SetActive(true);
			exitButton.SetActive(true);
			exitButton.GetComponent<Button>().onClick.AddListener(CancelButton);
		}
		else if (!SecurePlayerPrefs.GetBool(PlayerPrefConstants.SETTING_COUNTDOWN_TIMER))
		{
			okButton.GetComponent<Button>().interactable = true;
		}
	}

    /// <summary>
    /// Sets the transaction request values that need to be confirmed.
    /// </summary>
    /// <param name="onConfirmPressed"> Action to call if the transaction is confirmed to be sent through. </param>
    /// <param name="gasLimit"> The gas limit of the transaction request. </param>
    /// <param name="gasPrice"> The gas price of the transaction request. </param>
    /// <param name="displayInput"> The transaction input of the request being called. </param>
    public void SetConfirmationValues(Action onConfirmPressed, BigInteger gasLimit, BigInteger gasPrice, params object[] displayInput)
    {
        this.onConfirmPressed = onConfirmPressed;
        this.gasLimit = gasLimit;
        this.gasPrice = gasPrice;

        InternalSetConfirmationValues(displayInput);
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
        OnOkClicked();
    }
}