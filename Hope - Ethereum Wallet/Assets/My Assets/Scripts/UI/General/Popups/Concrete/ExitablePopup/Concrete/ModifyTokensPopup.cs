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
    [SerializeField] private Button addCustomToken;
	[SerializeField] private TMP_InputField searchBar;

    private AddableTokenButton.Factory addableTokenButtonFactory;
    private TokenListManager tokenListManager;

    public List<AddableTokenButton> AddableTokens { get; } = new List<AddableTokenButton>();

    [Inject]
    public void Construct(
        AddableTokenButton.Factory addableTokenButtonFactory,
        TokenListManager tokenListManager)
    {
        this.addableTokenButtonFactory = addableTokenButtonFactory;
        this.tokenListManager = tokenListManager;

        tokenListManager.GetTokenList().ForEach(UpdateTokens);
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

    private void AddCustomToken()
    {
        popupManager.GetPopup<AddTokenPopup>(true);
    }

    private void SearchInputChanged(string search)
    {
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