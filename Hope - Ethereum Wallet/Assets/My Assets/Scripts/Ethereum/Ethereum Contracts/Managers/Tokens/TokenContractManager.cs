using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class which initializes and manages all contracts for the game.
/// </summary>
public sealed class TokenContractManager
{
    public event Action OnTokensLoaded;
    public event Action<TradableAsset> OnTokenAdded;
    public event Action<string> OnTokenRemoved;

    private readonly PopupManager popupManager;
    private readonly TradableAssetImageManager tradableAssetImageManager;
    private readonly UserWalletManager userWalletManager;

    private readonly SecurePlayerPrefList<TokenInfo> tokens;

    /// <summary>
    /// The list of token info.
    /// </summary>
    public List<TokenInfo> TokenList => tokens.ToList();

    /// <summary>
    /// Initializes the TokenContractManager by creating all collections and getting the settings.
    /// </summary>
    /// <param name="settings"> The settings to use with this TokenContractManager. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="networkSettings"> The current EthereumNetworkManager settings. </param>
    public TokenContractManager(Settings settings,
        PopupManager popupManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager,
        EthereumNetworkManager.Settings networkSettings)
    {
        this.popupManager = popupManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;

        tokens = new SecurePlayerPrefList<TokenInfo>(settings.tokenPrefName, (int)networkSettings.networkType);
    }

    /// <summary>
    /// Adds a token given the info of the token.
    /// </summary>
    /// <param name="tokenInfo"> The info of the token to add. </param>
    /// <returns> The newly created ERC20 token from this TokenInfo. </returns>
    public ERC20 AddToken(TokenInfo tokenInfo)
    {
        ERC20 erc20Token = new ERC20(tokenInfo.Address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals);
        tokens.Add(tokenInfo);

        return erc20Token;
    }

    /// <summary>
    /// Adds a token given the info of the token and updates the balances in the TradableAssetManager.
    /// </summary>
    /// <param name="tokenInfo"> The info of the token to add. </param>
    /// <returns> The newly created ERC20 token from this TokenInfo. </returns>
    public ERC20 AddAndUpdateToken(TokenInfo tokenInfo)
    {
        var token = AddToken(tokenInfo);

        new ERC20TokenAsset(token, asset => UpdateTradableAssets(asset, () =>
        {
            if (popupManager.ActivePopupType == typeof(LoadingPopup))
                popupManager.CloseActivePopup();
        }), tradableAssetImageManager, userWalletManager);

        return token;
    }

    /// <summary>
    /// Removes the token address from the list of tokens in the player prefs and the TradableAssetManager.
    /// </summary>
    /// <param name="tokenAddress"> The contract address of the token to remove. </param>
    public void RemoveToken(string tokenAddress)
    {
        tokens.Remove(tokenAddress);
        OnTokenRemoved?.Invoke(tokenAddress);
    }

    /// <summary>
    /// Starts to load the tokens which were put in queue earlier.
    /// Also adds the first TradableAsset which would be the EtherAsset.
    /// </summary>
    /// <param name="onTokensLoaded"> Action to call once the tokens have finshed loading. </param>
    public void StartTokenLoad(Action onTokensLoaded)
    {
        var popup = popupManager.GetPopup<LoadingPopup>();

        if (popup != null)
            popup.Text = "Loading wallet";

        Action onLoadFinished = (() => popupManager.CloseAllPopups()) + onTokensLoaded;
        new EtherAsset(asset => UpdateTradableAssets(asset, () => LoadToken(0, onLoadFinished)), tradableAssetImageManager, userWalletManager);
    }

    /// <summary>
    /// Loads a token of the given index.
    /// </summary>
    /// <param name="index"> The index of the token to load. </param>
    /// <param name="onTokenLoadFinished"> Action to call once the token load is finished. </param>
    private void LoadToken(int index, Action onTokenLoadFinished)
    {
        if (CheckLoadStatus(index, onTokenLoadFinished))
            return;

        TokenInfo tokenInfo = tokens[index];
        ERC20 erc20Token = new ERC20(tokenInfo.Address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals);
        new ERC20TokenAsset(erc20Token, asset => UpdateTradableAssets(asset, () => LoadToken(++index, onTokenLoadFinished)), tradableAssetImageManager, userWalletManager);
    }

    /// <summary>
    /// Checks whether the initial token load status has completed or not.
    /// </summary>
    /// <param name="index"> The current token index. </param>
    /// <param name="onTokenLoadFinished"> Action to call once the tokens have finished loading. </param>
    /// <returns> True if the tokens are finished loading. </returns>
    private bool CheckLoadStatus(int index, Action onTokenLoadFinished)
    {
        if (index == tokens.Count)
        {
            OnTokensLoaded?.Invoke();
            onTokenLoadFinished?.Invoke();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the newly created TradableAsset to the TradableAssetManager and TradableAssetButtonManager.
    /// </summary>
    /// <param name="tradableAsset"> The asset to add. </param>
    /// <param name="onUpdateFinished"> Action to call once the tradable asset updating has finished. </param>
    private void UpdateTradableAssets(TradableAsset tradableAsset, Action onUpdateFinished)
    {
        if (tradableAsset.AssetSymbol != null)
            OnTokenAdded?.Invoke(tradableAsset);

        onUpdateFinished?.Invoke();
    }

    /// <summary>
    /// Class which contains the settings for the TokenContractManager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        [RandomizeText] public string tokenPrefName;
    }
}