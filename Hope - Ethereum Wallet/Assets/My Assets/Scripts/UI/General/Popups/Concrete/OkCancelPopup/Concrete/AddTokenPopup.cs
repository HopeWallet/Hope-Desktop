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

    [SerializeField] private HopeInputField addressField, symbolField, decimalsField;
    [SerializeField] private Image tokenIcon;
    [SerializeField] private TextMeshProUGUI tokenName;

    private TokenListManager tokenListManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    new private string name;
    private string symbol;
    private int? decimals;
    private dynamic balance;

    private bool updatedName, updatedSymbol, updatedDecimals, updatedBalance, updatedLogo;

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
        addressField.OnInputUpdated += _ => OnAddressChanged();
        symbolField.OnInputUpdated += _ => OnSymbolChanged();
        decimalsField.OnInputUpdated += _ => OnDecimalsChanged();
    }

    /// <summary>
    /// Start the token add process via the ContractManager.
    /// </summary>
    protected override void OnOkClicked()
    {
        if (!tokenListManager.ContainsToken(addressField.Text))
            tokenListManager.AddToken(addressField.Text, name, symbol, decimals.Value, true, true);
        else
            tokenListManager.UpdateToken(addressField.Text, true, true);

        popupManager.GetPopup<ModifyTokensPopup>().UpdateTokens(tokenListManager.GetToken(addressField.Text));
    }

    private void OnSymbolChanged()
    {
		string text = symbolField.Text;

		symbol = text;
		name = text;

		symbolField.Error = string.IsNullOrEmpty(text);
        okButton.interactable = !symbolField.Error && !decimalsField.Error;
    }

    private void OnDecimalsChanged()
    {
        int newDecimals;
        int.TryParse(decimalsField.Text, out newDecimals);
        decimals = newDecimals;

		decimalsField.Error = !string.IsNullOrEmpty(decimalsField.Text) && decimals.Value > 36;
		okButton.interactable = !decimalsField.Error && !symbolField.Error;
    }

    /// <summary>
    /// Method called every time the text in the input field changed.
    /// Sets the button to interactable if the text is a valid ethereum address.
    /// </summary>
    private void OnAddressChanged()
    {
        addressField.Error = !AddressUtils.IsValidEthereumAddress(addressField.Text);

		if (addressField.Error)
			CheckForInvalidAddress();
		else
			CheckForValidAddress();
	}

    private void CheckForInvalidAddress()
    {
		if (!addressField.Error)
			return;

        OnStatusChanged?.Invoke(Status.NoTokenFound);
        okButton.interactable = false;
    }

    private void CheckForValidAddress()
    {
        if (addressField.Error)
            return;

        addressField.InputFieldBase.interactable = false;

        bool existsInTokenList = tokenListManager.ContainsToken(addressField.Text);
        CheckTokenList(existsInTokenList);
        CheckTokenContract(existsInTokenList);
    }

    private void CheckTokenList(bool existsInTokenList)
    {
        if (!existsInTokenList)
            return;

        AddableTokenInfo addableToken = tokenListManager.GetToken(addressField.Text);
        TokenInfo tokenInfo = addableToken.TokenInfo;
        name = tokenInfo.Name;
		symbol = tokenInfo.Symbol;
		decimals = tokenInfo.Decimals;

		tokenName.text = name.LimitEnd(40, "...") + (!string.IsNullOrEmpty(symbol) ? " (" + symbol + ")" : "");
		tradableAssetImageManager.LoadImage(symbol, icon => tokenIcon.sprite = icon);

        OnStatusChanged?.Invoke(Status.ValidToken);

        addressField.InputFieldBase.interactable = true;
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

        addressField.InputFieldBase.interactable = false;

        OnStatusChanged?.Invoke(Status.Loading);

		string addressText = addressField.Text;

		SimpleContractQueries.QueryStringOutput<Name>(addressText, null).OnSuccess(output => NameQueryCompleted(output.Value));
        SimpleContractQueries.QueryStringOutput<Symbol>(addressText, null).OnSuccess(output => SymbolQueryCompleted(output.Value));
        SimpleContractQueries.QueryUInt256Output<Decimals>(addressText, null).OnSuccess(output => DecimalsQueryCompleted(output.Value));
        SimpleContractQueries.QueryUInt256Output<BalanceOf>(addressText, userWalletManager.GetWalletAddress(), userWalletManager.GetWalletAddress()).OnSuccess(output => BalanceQueryCompleted(output.Value));
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
			addressField.InputFieldBase.interactable = true;

			if (balance == null)
            {
                OnStatusChanged?.Invoke(Status.NoTokenFound);
            }
            else if (string.IsNullOrEmpty(symbol) || !decimals.HasValue)
            {
                decimalsField.Text = string.Empty;
                symbolField.Text = string.Empty;

                OnStatusChanged?.Invoke(Status.InvalidToken);
				symbolField.InputFieldBase.ActivateInputField();

                okButton.interactable = false;
            }
            else
            {
                tokenName.text = symbol;
                tokenListManager.AddToken(addressField.Text, name, symbol, decimals.Value, false, false);

                OnStatusChanged?.Invoke(Status.ValidToken);

                okButton.interactable = true;
            }
        }
    }

    /// <summary>
    /// The status of the AddTokenPopup.
    /// <para> <see cref="Loading"/> - The entered address is being searched for the name/symbol/decimals. </para>
    /// <para> <see cref="NoTokenFound"/> - The entered address is not a full length address and cannot be searched for. </para>
    /// <para> <see cref="InvalidToken"/> - The entered address was searched for but cannot be verified as a valid address, therefore the fields for Symbol and Decimals needs to be available. </para>
    /// <para> <see cref="ValidToken"/> - The entered address was searched for and found, therefore the image and symbol text can be displayed. </para>
    /// </summary>
    public enum Status { Loading, NoTokenFound, InvalidToken, ValidToken };
}