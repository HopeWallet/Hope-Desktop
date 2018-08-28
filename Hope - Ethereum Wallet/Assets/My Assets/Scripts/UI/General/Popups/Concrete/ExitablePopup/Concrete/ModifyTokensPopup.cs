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
    [SerializeField] private Button addCustomToken, confirmButton;
    [SerializeField] private HopeInputField searchBar;

    private List<AddableTokenInfo> removedTokens = new List<AddableTokenInfo>();

    private AddableTokenButton.Factory addableTokenButtonFactory;
    private TokenContractManager tokenContractManager;
    private TokenListManager tokenListManager;

    private bool confirmClicked;

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
        if (!addableTokenInfo.Listed)
            return;

        if (AddableTokens.Select(tokenButton => tokenButton.ButtonInfo.TokenInfo.Address).ContainsIgnoreCase(addableTokenInfo.TokenInfo.Address))
            AddableTokens.First(tokenButton => tokenButton.ButtonInfo.TokenInfo.Address.EqualsIgnoreCase(addableTokenInfo.TokenInfo.Address)).SetButtonInfo(addableTokenInfo);
        else
            AddableTokens.Add(CreateNewButton(addableTokenInfo));
    }

    public void RemoveToken(AddableTokenInfo addableTokenInfo)
    {
        var itemToRemove = AddableTokens.Single(info => info.ButtonInfo.TokenInfo.Address.EqualsIgnoreCase(addableTokenInfo.TokenInfo.Address));
        removedTokens.Add(itemToRemove.ButtonInfo);
        AddableTokens.Remove(itemToRemove);
        Destroy(itemToRemove.transform.parent.gameObject);

        tokenListManager.UpdateToken(addableTokenInfo.TokenInfo.Address, false, false);
    }

    protected override void OnStart()
    {
        addCustomToken.onClick.AddListener(CustomTokenButtonClicked);
        searchBar.OnInputUpdated += SearchInputChanged;
        confirmButton.onClick.AddListener(ConfirmButtonClicked);
    }

    private void OnDestroy()
    {
        if (confirmClicked)
        {
            tokenListManager.OldTokenList.Clear();
        }
        else
        {
            tokenListManager.OldTokenList.ForEach(token => tokenListManager.UpdateToken(token.TokenInfo.Address, token.Enabled, token.Listed));
            tokenListManager.OldTokenList.Clear();
        }

        TokenListButton.popupClosed?.Invoke();
    }

    private void ConfirmButtonClicked()
    {
        confirmClicked = true;

        var activeTokenAddresses = tokenContractManager.TokenList.Select(token => token.Address);
        var tokensToEnable = AddableTokens.Where(token => !activeTokenAddresses.Contains(token.ButtonInfo.TokenInfo.Address) && token.ButtonInfo.Enabled);
        var tokensToDisable = AddableTokens.Where(token => activeTokenAddresses.Contains(token.ButtonInfo.TokenInfo.Address) && !token.ButtonInfo.Enabled);

        removedTokens.ForEach(info => tokenContractManager.RemoveToken(info.TokenInfo.Address));
        tokensToDisable.ForEach(tokenButton => tokenContractManager.RemoveToken(tokenButton.ButtonInfo.TokenInfo.Address));
        tokensToEnable.ForEach(tokenButton => tokenContractManager.AddToken(tokenButton.ButtonInfo.TokenInfo));

        popupManager.CloseActivePopup();
    }

    private void CustomTokenButtonClicked() => popupManager.GetPopup<AddTokenPopup>(true);

    private void SearchInputChanged()
    {
        string search = searchBar.Text.ToUpper();

        AddableTokens
            .Where(token => token.ButtonInfo.TokenInfo.Name.ToUpper().Contains(search) || token.ButtonInfo.TokenInfo.Symbol.Contains(search))
            .ForEach(token => token.transform.parent.gameObject.SetActive(true));

        AddableTokens
            .Where(token => !token.ButtonInfo.TokenInfo.Name.ToUpper().Contains(search) && !token.ButtonInfo.TokenInfo.Symbol.Contains(search))
            .ForEach(token => token.transform.parent.gameObject.SetActive(false));

        var visibleTokens = AddableTokens.Where(token => token.gameObject.activeInHierarchy).ToList();

        searchBar.Error = visibleTokens.Count == 0;
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