using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is manages the popup for adding a token to the list of tokens.
/// </summary>
public class AddTokenPopup : OkCancelPopupComponent<AddTokenPopup>
{

    public InputField inputField;

    private TokenContractManager tokenContractManager;

    /// <summary>
    /// Gets the input field in the children and makes sure the ok button is disabled.
    /// </summary>
    protected override void OnStart()
    {
        inputField.onValueChanged.AddListener(OnTextChanged);
        okButton.interactable = false;
    }

    /// <summary>
    /// Start the token add process via the ContractManager.
    /// </summary>
    protected override void OnOkClicked() => tokenContractManager.AddToken(inputField.text);

    /// <summary>
    /// Method called every time the text in the input field changed.
    /// Sets the button to interactable if the text is a valid ethereum address.
    /// </summary>
    /// <param name="text"> The text of the input field. </param>
    public void OnTextChanged(string text) => okButton.interactable = AddressUtils.IsValidEthereumAddress(text);

    /// <summary> 
    /// Injects more dependencies into this popup.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    [Inject]
    public void Construct(TokenContractManager tokenContractManager) => this.tokenContractManager = tokenContractManager;

}
