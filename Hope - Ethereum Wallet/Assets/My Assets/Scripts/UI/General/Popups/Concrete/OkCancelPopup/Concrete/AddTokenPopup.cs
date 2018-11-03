using UnityEngine.UI;
using Hope.Utils.Ethereum;
using TMPro;
using Zenject;
using UnityEngine;
using System;
using System.Linq;
using static ERC20.Queries;
using System.Collections.Generic;

/// <summary>
/// Class which is manages the popup for adding a token to the list of tokens.
/// </summary>
public sealed class AddTokenPopup : OkCancelPopupComponent<AddTokenPopup>, IEnterButtonObservable, ITabButtonObservable
{
    public event Action<Status> OnStatusChanged;

    [SerializeField] private HopeInputField addressField, symbolField, decimalsField;
    [SerializeField] private Image tokenIcon;
    [SerializeField] private TextMeshProUGUI tokenName;
    [SerializeField] private Transform addableTokenSpawnTransform;
	[SerializeField] private GameObject invalidTokenSection;

    private readonly List<AddableTokenButton> addableTokens = new List<AddableTokenButton>();

	private List<Selectable> selectableFields = new List<Selectable>();

    private TokenListManager tokenListManager;
    private TokenContractManager tokenContractManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;
    private AddableTokenButton.Factory addableTokenButtonFactory;
	private ButtonClickObserver buttonClickObserver;

    private AddableTokenButton activelySelectedButton;

    new private string name;
    private string symbol;
    private int? decimals;
    private dynamic balance;

    private bool isInvalidAddress;

    private const int MAX_TOKEN_COUNT = 10;

    /// <summary>
    /// The actively selected AddableTokenButton.
    /// </summary>
    public AddableTokenButton ActivelySelectedButton
    {
        get
        {
            return activelySelectedButton;
        }
        set
        {
            activelySelectedButton = value;
            okButton.interactable = activelySelectedButton != null;
        }
    }

    /// <summary> 
    /// Injects dependencies into this popup.
    /// </summary>
    /// <param name="tokenListManager"> The active TokenListManager. </param>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(
        TokenListManager tokenListManager,
        TokenContractManager tokenContractManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager,
        AddableTokenButton.Factory addableTokenButtonFactory,
		ButtonClickObserver buttonClickObserver)
    {
        this.tokenListManager = tokenListManager;
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
        this.addableTokenButtonFactory = addableTokenButtonFactory;
		this.buttonClickObserver = buttonClickObserver;

		buttonClickObserver.SubscribeObservable(this);

		selectableFields.Add(addressField.InputFieldBase);
		selectableFields.Add(symbolField.InputFieldBase);
		selectableFields.Add(decimalsField.InputFieldBase);
	}

    /// <summary>
    /// Gets the input field in the children and makes sure the ok button is disabled.
    /// </summary>
    protected override void OnStart()
    {
        addressField.OnInputUpdated += _ => OnAddressChanged();
        symbolField.OnInputUpdated += _ => OnSymbolChanged();
        decimalsField.OnInputUpdated += _ => OnDecimalsChanged();

        addressField.Error = false;
    }

	/// <summary>
	/// Unsubscribes the button click observer
	/// </summary>
	private void OnDisable() => buttonClickObserver.UnsubscribeObservable(this);

	/// <summary>
	/// Start the token add process via the ContractManager.
	/// </summary>
	protected override void OnOkClicked()
    {
        if (ActivelySelectedButton == null)
            tokenContractManager.AddAndUpdateToken(new TokenInfo(addressField.Text, name, symbol, decimals.Value));
        else
            tokenContractManager.AddAndUpdateToken(ActivelySelectedButton.ButtonInfo);
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
        isInvalidAddress = !AddressUtils.IsValidEthereumAddress(addressField.Text);

        if (isInvalidAddress)
            CheckInvalidAddress();
        else
            CheckValidAddress();
    }

    private void CheckInvalidAddress()
    {
        if (!isInvalidAddress)
            return;

        var loweredText = addressField.Text.ToLower();
        var possibleTokens = tokenListManager.TokenList.Where(token => token.Name.ToLower().Contains(loweredText) || token.Symbol.ToLower().StartsWith(loweredText)).ToList();

        ActivelySelectedButton?.Toggle();

        if (string.IsNullOrEmpty(loweredText) || possibleTokens.Count == 0)
        {
            OnStatusChanged?.Invoke(Status.NoTokenFound);
            okButton.interactable = false;
        }
        else if (possibleTokens.Count > MAX_TOKEN_COUNT)
        {
            OnStatusChanged?.Invoke(Status.TooManyTokensFound);
            okButton.interactable = false;
        }
        else
        {
            DisplayAddableTokens(possibleTokens);
        }
    }

    private void DisplayAddableTokens(List<TokenInfo> newTokenList)
    {
        newTokenList.Sort((t1, t2) => t1.Symbol.CompareTo(t2.Symbol));

        for (int i = newTokenList.Count; i < addableTokens.Count; i++)
            addableTokens[i].transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < newTokenList.Count; i++)
        {
            if (i < addableTokens.Count)
            {
                addableTokens[i].transform.parent.gameObject.SetActive(true);
                addableTokens[i].SetButtonInfo(newTokenList[i]);
            }
            else
            {
                var newTokenButton = addableTokenButtonFactory.Create();
                newTokenButton.SetButtonInfo(newTokenList[i]);
                newTokenButton.transform.parent.parent = addableTokenSpawnTransform;
                newTokenButton.transform.parent.localScale = Vector3.one;
                addableTokens.Add(newTokenButton);
            }
        }

        OnStatusChanged?.Invoke(Status.MultipleTokensFound);
    }

    private void CheckValidAddress()
    {
        if (isInvalidAddress)
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

        TokenInfo tokenInfo = tokenListManager.GetToken(addressField.Text);
        name = tokenInfo.Name;
        symbol = tokenInfo.Symbol;
        decimals = tokenInfo.Decimals;

        tokenName.text = name.LimitEnd(40, "...") + (!string.IsNullOrEmpty(symbol) ? $" ({symbol})" : "");
        tradableAssetImageManager.LoadImage(symbol, icon => tokenIcon.sprite = icon);

        OnStatusChanged?.Invoke(Status.ValidToken);

        addressField.InputFieldBase.interactable = true;
        okButton.interactable = true;
    }

    private void CheckTokenContract(bool existsInTokenList)
    {
        if (existsInTokenList)
            return;

        addressField.InputFieldBase.interactable = false;

        OnStatusChanged?.Invoke(Status.Loading);

        string addressText = addressField.Text;

        ERC20 erc20 = new ERC20(addressText);

        erc20.OnInitializationSuccessful(() =>
        {
            tradableAssetImageManager.LoadImage(erc20.Symbol, img =>
            {
                tokenIcon.sprite = img;

                CheckStatus(erc20.Symbol, erc20.Name, erc20.Decimals, 0);
            });
        });

        erc20.OnInitializationUnsuccessful(() =>
        {
            SimpleContractQueries.QueryUInt256Output<BalanceOf>(addressText, userWalletManager.GetWalletAddress(), userWalletManager.GetWalletAddress())
                                 .OnSuccess(balance => CheckStatus(null, null, null, balance.Value))
                                 .OnError(_ => CheckStatus(null, null, null, null));
        });
    }

    private void CheckStatus(string symbol, string name, int? decimals, dynamic balance)
    {
        addressField.InputFieldBase.interactable = true;

        this.symbol = symbol;
        this.name = name;
        this.decimals = decimals;
        this.balance = balance;

        if (balance == null)
            NoTokenFound();
        else if (string.IsNullOrEmpty(symbol) || !decimals.HasValue)
            InvalidTokenFound();
        else
            ValidTokenFound();
    }

    private void NoTokenFound()
    {
        OnStatusChanged?.Invoke(Status.NoTokenFound);
        okButton.interactable = false;
    }

    private void InvalidTokenFound()
    {
        decimalsField.Text = string.Empty;
        symbolField.Text = string.Empty;

        OnStatusChanged?.Invoke(Status.InvalidToken);
        symbolField.InputFieldBase.ActivateInputField();

        okButton.interactable = false;
    }

    private void ValidTokenFound()
    {
        tokenName.text = name.LimitEnd(40, "...") + (!string.IsNullOrEmpty(symbol) ? $" ({symbol})" : "");
        tokenListManager.AddToken(addressField.Text, name, symbol, decimals.Value);

        OnStatusChanged?.Invoke(Status.ValidToken);

        okButton.interactable = true;
    }

	/// <summary>
	/// Moves to the next input field
	/// </summary>
	/// <param name="clickType"> The tab button ClickType </param>
	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (invalidTokenSection.activeInHierarchy)
			selectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// Clicks the send button if on the last input field 
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down)
			return;

		if (invalidTokenSection.activeInHierarchy && InputFieldUtils.GetActiveInputField() == decimalsField.InputFieldBase && okButton.interactable)
			okButton.Press();
		else if (invalidTokenSection.activeInHierarchy)
			selectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// The status of the AddTokenPopup.
	/// <para> <see cref="Loading"/> - The entered address is being searched for the name/symbol/decimals. </para>
	/// <para> <see cref="NoTokenFound"/> - The entered address is not a full length address and cannot be searched for. </para>
	/// <para> <see cref="InvalidToken"/> - The entered address was searched for but cannot be verified as a valid address, therefore the fields for Symbol and Decimals needs to be available. </para>
	/// <para> <see cref="ValidToken"/> - The entered address was searched for and found, therefore the image and symbol text can be displayed. </para>
	/// </summary>
	public enum Status { NoTokenFound, MultipleTokensFound, TooManyTokensFound, InvalidToken, ValidToken, Loading };
}