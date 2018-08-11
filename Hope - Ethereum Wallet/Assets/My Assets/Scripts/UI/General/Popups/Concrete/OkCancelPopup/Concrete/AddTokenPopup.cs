using UnityEngine.UI;
using Hope.Utils.Ethereum;
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

    private static int addressId;

    [SerializeField] private TMP_InputField addressField, symbolField, decimalsField;
    [SerializeField] private Image tokenIcon;
    [SerializeField] private TextMeshProUGUI tokenSymbol;

    private TokenListManager tokenListManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    new private string name;
    private string symbol;
    private int? decimals;
    private dynamic balance;

    private bool updatedName, updatedSymbol, updatedDecimals, updatedBalance, updatedLogo;
    private bool validSymbol, validDecimals;

    private int previousAddressLength;

    /// <summary> 
    /// Injects dependencies into this popup.
    /// </summary>
    /// <param name="tokenListManager"> The active TokenListManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(
        TokenListManager tokenListManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager)
    {
        this.tokenListManager = tokenListManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Gets the input field in the children and makes sure the ok button is disabled.
    /// </summary>
    protected override void OnStart()
    {
        addressField.onValueChanged.AddListener(OnAddressChanged);
        symbolField.onValueChanged.AddListener(OnSymbolChanged);
        decimalsField.onValueChanged.AddListener(OnDecimalsChanged);
    }

    /// <summary>
    /// Start the token add process via the ContractManager.
    /// </summary>
    protected override void OnOkClicked()
    {
        if (!tokenListManager.ContainsToken(addressField.text))
            tokenListManager.AddToken(addressField.text, name, symbol, decimals.Value, true, true);
        else
            tokenListManager.UpdateToken(addressField.text, true, true);

        popupManager.GetPopup<ModifyTokensPopup>().UpdateTokens(tokenListManager.GetToken(addressField.text));
    }

    private void OnSymbolChanged(string value)
    {
        symbolField.text = value.LimitEnd(5).ToUpper();
        symbol = symbolField.text;
        name = symbolField.text;

        validSymbol = !string.IsNullOrEmpty(symbolField.text);
        okButton.interactable = validDecimals && validSymbol;
    }

    private void OnDecimalsChanged(string value)
    {
        decimalsField.text = value.LimitEnd(2);
        decimals = int.Parse(decimalsField.text);

        validDecimals = decimals.Value < 36;
        okButton.interactable = validDecimals && validSymbol;
    }

    /// <summary>
    /// Method called every time the text in the input field changed.
    /// Sets the button to interactable if the text is a valid ethereum address.
    /// </summary>
    /// <param name="address"> The inputted text in the address input field. </param>
    private void OnAddressChanged(string address)
    {
        if (address.Length == 43 || previousAddressLength == 43)
        {
            previousAddressLength = address.Length;
            addressField.text = address.LimitEnd(42);
            return;
        }

        bool validAddress = AddressUtils.IsValidEthereumAddress(addressField.text);
        CheckForInvalidAddress(validAddress);
        CheckForValidAddress(validAddress);
    }

    private void CheckForInvalidAddress(bool validAddress)
    {
        if (validAddress)
            return;

        OnStatusChanged?.Invoke(Status.NoTokenFound);
        okButton.interactable = false;
    }

    private void CheckForValidAddress(bool validAddress)
    {
        if (!validAddress)
            return;

        addressField.readOnly = true;

        bool existsInTokenList = tokenListManager.ContainsToken(addressField.text);
        CheckTokenList(existsInTokenList);
        CheckTokenContract(existsInTokenList);
    }

    private void CheckTokenList(bool existsInTokenList)
    {
        if (!existsInTokenList)
            return;

        AddableTokenInfo addableToken = tokenListManager.GetToken(addressField.text);
        TokenInfo tokenInfo = addableToken.TokenInfo;
        name = tokenInfo.Name;
        symbol = tokenInfo.Symbol;
        decimals = tokenInfo.Decimals;

        tokenSymbol.text = symbol;
        tradableAssetImageManager.LoadImage(symbol, icon => tokenIcon.sprite = icon);

        OnStatusChanged?.Invoke(Status.ValidToken);

        addressField.readOnly = false;
        okButton.interactable = true;
    }

    private void CheckTokenContract(bool existsInTokenList)
    {
        if (existsInTokenList)
            return;

        updatedName = false;
        updatedSymbol = false;
        updatedDecimals = false;
        updatedLogo = false;
        updatedBalance = false;

        addressField.readOnly = true;

        OnStatusChanged?.Invoke(Status.Loading);

        SimpleContractQueries.QueryStringOutput<Name>(addressField.text, null, output => NameQueryCompleted(output.Value));
        SimpleContractQueries.QueryStringOutput<Symbol>(addressField.text, null, output => SymbolQueryCompleted(output.Value));
        SimpleContractQueries.QueryUInt256Output<Decimals>(addressField.text, null, output => DecimalsQueryCompleted(output.Value));
        SimpleContractQueries.QueryUInt256Output<BalanceOf>(addressField.text, userWalletManager.WalletAddress, output => BalanceQueryCompleted(output.Value), userWalletManager.WalletAddress);
    }

    private void NameQueryCompleted(string value)
    {
        name = string.IsNullOrEmpty(value) ? name : value;
        CheckLoadStatus(ref updatedName);
    }

    private void SymbolQueryCompleted(string value)
    {
        symbol = string.IsNullOrEmpty(value) ? string.Empty : value;
        name = string.IsNullOrEmpty(name) ? symbol : name;

        tradableAssetImageManager.LoadImage(symbol, LogoQueryCompleted);

        CheckLoadStatus(ref updatedSymbol);
    }

    private void DecimalsQueryCompleted(dynamic value)
    {
        decimals = value == null ? (int?)null : (int)value;
        CheckLoadStatus(ref updatedDecimals);
    }

    private void BalanceQueryCompleted(dynamic value)
    {
        balance = value;
        CheckLoadStatus(ref updatedBalance);
    }

    private void LogoQueryCompleted(Sprite value)
    {
        tokenIcon.sprite = value;
        CheckLoadStatus(ref updatedLogo);
    }

    private void CheckLoadStatus(ref bool updatingVar)
    {
        updatingVar = true;

        if (updatedName && updatedSymbol && updatedDecimals && updatedLogo && updatedBalance)
        {
            if (balance == null)
            {
                OnStatusChanged?.Invoke(Status.NoTokenFound);
            }
            else if (string.IsNullOrEmpty(symbol) || !decimals.HasValue)
            {
                decimalsField.text = string.Empty;
                symbolField.text = string.Empty;

                OnStatusChanged?.Invoke(Status.InvalidToken);

                okButton.interactable = false;
            }
            else
            {
                tokenSymbol.text = symbol;
                tokenListManager.AddToken(addressField.text, name, symbol, decimals.Value, false, false);

                OnStatusChanged?.Invoke(Status.ValidToken);

                okButton.interactable = true;
            }

            addressField.readOnly = false;
        }
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