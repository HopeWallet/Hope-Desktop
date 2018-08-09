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
    private TokenListManager tokenListManager;
    private TradableAssetImageManager tradableAssetImageManager;

    new private string name;
    private string symbol;
    private int decimals;

    private bool updatedName, updatedSymbol, updatedDecimals, updatedLogo;

    /// <summary> 
    /// Injects more dependencies into this popup.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    [Inject]
    public void Construct(
        TokenContractManager tokenContractManager,
        TokenListManager tokenListManager,
        TradableAssetImageManager tradableAssetImageManager)
    {
        this.tokenContractManager = tokenContractManager;
        this.tokenListManager = tokenListManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
    }

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

        bool validAddress = AddressUtils.IsValidEthereumAddress(addressField.text);
        CheckForInvalidAddress(validAddress);
        CheckForValidAddress(validAddress);
    }

    private void CheckForInvalidAddress(bool validAddress)
    {
        if (validAddress)
            return;

        OnStatusChanged?.Invoke(Status.NoTokenFound);
    }

    private void CheckForValidAddress(bool validAddress)
    {
        if (!validAddress)
            return;

        addressField.interactable = false;

        bool existsInTokenList = tokenListManager.AddableTokens.Contains(addressField.text);
        CheckTokenList(existsInTokenList);
        CheckTokenContract(existsInTokenList);
    }

    private void CheckTokenList(bool existsInTokenList)
    {
        if (!existsInTokenList)
            return;


    }

    private void CheckTokenContract(bool existsInTokenList)
    {
        if (existsInTokenList)
            return;

        updatedName = false;
        updatedSymbol = false;
        updatedDecimals = false;
        updatedLogo = false;

        OnStatusChanged?.Invoke(Status.Loading);

        //SimpleContractQueries.QueryStringOutput<Name>(addressField.text, null, output => CheckStatus(ref updatingName, ))
    }

    private void OnNameReceived(string value)
    {
        name = string.IsNullOrEmpty(value) ? name : value;
        CheckStatus(ref updatedName);
    }

    private void OnSymbolReceived(string value)
    {
        name = string.IsNullOrEmpty(name) ? value : name;
        symbol = value;

        tradableAssetImageManager.LoadImage(symbol, OnLogoReceived);

        CheckStatus(ref updatedSymbol);
    }

    private void OnDecimalsReceived(dynamic value)
    {
        decimals = value == null ? 0 : (int)value;
        CheckStatus(ref updatedDecimals);
    }

    private void OnLogoReceived(Sprite value)
    {
        tokenIcon.sprite = value;
        CheckStatus(ref updatedLogo);
    }

    private void CheckStatus(ref bool updatingVar)
    {
        updatingVar = true;

        if (updatedName && updatedSymbol && updatedDecimals && updatedLogo)
        {

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