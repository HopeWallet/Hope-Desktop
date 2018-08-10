using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class ModifyTokensPopup : ExitablePopupComponent<ModifyTokensPopup>
{
    public event Action<AddableTokenButton> OnAddableTokenAdded;

    [SerializeField] private Transform tokenListTransform;
    [SerializeField] private Button addCustomToken, 
									ConfirmButton;
	[SerializeField] private TMP_InputField searchBar;

	private AddableTokenButton.Factory addableTokenButtonFactory;
    private TokenContractManager tokenContractManager;
    private TokenListManager tokenListManager;

    public List<AddableTokenButton> AddableTokens { get; } = new List<AddableTokenButton>();

    [Inject]
    public void Construct(
        AddableTokenButton.Factory addableTokenButtonFactory,
        TokenContractManager tokenContractManager,
        TokenListManager tokenListManager)
    {
        this.addableTokenButtonFactory = addableTokenButtonFactory;
        this.tokenContractManager = tokenContractManager;
        this.tokenListManager = tokenListManager;

        tokenListManager.TokenList.ForEach(UpdateTokens);
    }

    public void UpdateTokens(AddableTokenInfo addableTokenInfo)
    {
        if (AddableTokens.Select(tokenButton => tokenButton.ButtonInfo.TokenInfo.Address).ContainsIgnoreCase(addableTokenInfo.TokenInfo.Address))
            AddableTokens.First(tokenButton => tokenButton.ButtonInfo.TokenInfo.Address.EqualsIgnoreCase(addableTokenInfo.TokenInfo.Address)).SetButtonInfo(addableTokenInfo);
        else
            AddableTokens.Add(CreateNewButton(addableTokenInfo));
    }

    protected override void OnStart()
    {
		addCustomToken.onClick.AddListener(AddCustomToken);
		searchBar.onValueChanged.AddListener(SearchInputChanged);
	}

    private void OnDestroy()
    {
        var activeTokenAddresses = tokenContractManager.TokenList.Select(token => token.Address);
        var tokensToEnable = AddableTokens.Where(token => !activeTokenAddresses.Contains(token.ButtonInfo.TokenInfo.Address) && token.ButtonInfo.Enabled);
        var tokensToDisable = AddableTokens.Where(token => activeTokenAddresses.Contains(token.ButtonInfo.TokenInfo.Address) && !token.ButtonInfo.Enabled);

        tokensToDisable.ForEach(tokenButton => tokenContractManager.RemoveToken(tokenButton.ButtonInfo.TokenInfo.Address));
        tokensToEnable.ForEach(tokenButton => tokenContractManager.AddToken(tokenButton.ButtonInfo.TokenInfo));
    }

    private void AddCustomToken()
    {
        popupManager.GetPopup<AddTokenPopup>(true);
    }

    private void SearchInputChanged(string search)
    {
        search = search.ToUpper();

        AddableTokens
            .Where(token => token.ButtonInfo.TokenInfo.Name.ToUpper().Contains(search) || token.ButtonInfo.TokenInfo.Symbol.Contains(search))
            .ForEach(token => token.transform.parent.gameObject.SetActive(true));

        AddableTokens
            .Where(token => !token.ButtonInfo.TokenInfo.Name.ToUpper().Contains(search) && !token.ButtonInfo.TokenInfo.Symbol.Contains(search))
            .ForEach(token => token.transform.parent.gameObject.SetActive(false));
    }

    private AddableTokenButton CreateNewButton(AddableTokenInfo addableTokenInfo)
    {
        AddableTokenButton tokenButton = addableTokenButtonFactory.Create().SetButtonInfo(addableTokenInfo);
        Transform componentTransform = tokenButton.transform;
        Transform parentTransform = componentTransform.parent;

        parentTransform.parent = tokenListTransform;
        parentTransform.localScale = new Vector3(0f, 1f, 1f);
        componentTransform.localScale = Vector3.one;

        OnAddableTokenAdded?.Invoke(tokenButton);

        return tokenButton;
    }
}