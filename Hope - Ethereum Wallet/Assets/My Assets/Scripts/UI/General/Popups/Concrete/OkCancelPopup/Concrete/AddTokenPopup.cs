using UnityEngine.UI;
using Hope.Utils.EthereumUtils;
using TMPro;
using Zenject;

/// <summary>
/// Class which is manages the popup for adding a token to the list of tokens.
/// </summary>
public sealed class AddTokenPopup : OkCancelPopupComponent<AddTokenPopup>
{
    public TMP_InputField addressField, symbolField, decimalsField;

	public Image tokenIcon;

	public TextMeshProUGUI tokenSymbol;

	private AddTokenPopupAnimator addTokenPopupAnimator;
    private TokenContractManager tokenContractManager;

    /// <summary> 
    /// Injects more dependencies into this popup.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    [Inject]
    public void Construct(TokenContractManager tokenContractManager) => this.tokenContractManager = tokenContractManager;

    /// <summary>
    /// Gets the input field in the children and makes sure the ok button is disabled.
    /// </summary>
    protected override void OnStart()
    {
		addTokenPopupAnimator = transform.GetComponent<AddTokenPopupAnimator>();

        addressField.onValueChanged.AddListener(OnAddressChanged);
    }

    /// <summary>
    /// Start the token add process via the ContractManager.
    /// </summary>
    protected override void OnOkClicked() => tokenContractManager.AddToken(addressField.text);

	/// <summary>
	/// Method called every time the text in the input field changed.
	/// Sets the button to interactable if the text is a valid ethereum address.
	/// </summary>
	/// <param name="address"> The inputted text in the address input field. </param>
	private void OnAddressChanged(string address)
	{
		if (!AddressUtils.CorrectAddressLength(address))
			addressField.text = address.LimitEnd(42);

		string updatedAddress = addressField.text;

		addTokenPopupAnimator.ValidAddress = string.IsNullOrEmpty(updatedAddress) || AddressUtils.IsValidEthereumAddress(updatedAddress);
		addTokenPopupAnimator.AnimateLoadingLine(addTokenPopupAnimator.ValidAddress && !string.IsNullOrEmpty(updatedAddress));

		addTokenPopupAnimator.AnimateFieldError(addressField, !addTokenPopupAnimator.ValidAddress);

		okButton.interactable = addTokenPopupAnimator.RealTokenAddress;
	}
}
