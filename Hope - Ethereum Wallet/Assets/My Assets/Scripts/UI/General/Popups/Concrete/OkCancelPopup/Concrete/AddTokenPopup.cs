using UnityEngine.UI;
using Hope.Utils.EthereumUtils;
using TMPro;
using Zenject;
using UnityEngine;
using System;
using static ERC20.Queries;

/// <summary>
/// Class which is manages the popup for adding a token to the list of tokens.
/// </summary>
public sealed class AddTokenPopup : OkCancelPopupComponent<AddTokenPopup>
{
    public event Action<Status> OnStatusChanged;

    [SerializeField] private TMP_InputField addressField, symbolField, decimalsField;
    [SerializeField] private Image tokenIcon;
    [SerializeField] private TextMeshProUGUI tokenSymbol;

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
        addressField.text = address.LimitEnd(42);

        if (!AddressUtils.IsValidEthereumAddress(addressField.text))
        {
            OnStatusChanged?.Invoke(Status.NoTokenFound);
        }
    }

    private void Test()
    {
        SimpleContractQueries.QueryStringOutput<Name>("0x0", null, output => Debug.Log(output.Value));
        SimpleContractQueries.QueryStringOutput<Symbol>("0x0", null, output => Debug.Log(output.Value));
        SimpleContractQueries.QueryUInt256Output<Decimals>("0x0", null, output => Debug.Log(output.Value));
    }

    /// <summary>
    /// The status of the AddTokenPopup.
    /// Loading - The entered address is being searched for the name/symbol/decimals.
    /// NoTokenFound - The entered address is not a full length address and cannot be searched for.
    /// InvalidToken - The entered address was searched for but cannot be verified as a valid address, therefore the fields for Symbol and Decimals needs to be available.
    /// ValidToken - The entered address was searched for and found, therefore the image and symbol text can be displayed.
    /// </summary>
    public enum Status { Loading, NoTokenFound, InvalidToken, ValidToken };
}