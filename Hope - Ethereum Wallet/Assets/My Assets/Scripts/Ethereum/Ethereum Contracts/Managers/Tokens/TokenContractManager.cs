using System;

/// <summary>
/// Class which initializes and manages all contracts for the game.
/// </summary>
public sealed class TokenContractManager
{
    public static event Action OnTokensLoaded;
    public static event Action<TradableAsset> OnTokenAdded;
    public static event Action<string> OnTokenRemoved;

    private readonly PopupManager popupManager;
    private readonly TradableAssetImageManager tradableAssetImageManager;
    private readonly UserWalletManager userWalletManager;

    private readonly SecurePlayerPrefList<TokenInfo> tokens;

    /// <summary>
    /// Initializes the TokenContractManager by creating all collections and getting the settings.
    /// </summary>
    /// <param name="settings"> The settings to use with this TokenContractManager. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    public TokenContractManager(Settings settings,
        PopupManager popupManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager)
    {
        this.popupManager = popupManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;

        tokens = new SecurePlayerPrefList<TokenInfo>(settings.tokenPrefName);
    }

    /// <summary>
    /// Gets the index of an asset which was saved to the prefs.
    /// </summary>
    /// <param name="tokenAddress"> The address of the token to find the index for. </param>
    /// <returns> The index of the token. </returns>
    public int GetTokenIndex(string tokenAddress)
    {
        if (!tokens.Contains(tokenAddress))
            return 0;

        return tokens.IndexOf(tokenAddress);
    }

    /// <summary>
    /// Adds a token to the list of tokens in the ContractManager.
    /// </summary>
    /// <param name="tokenAddress"> The token address of the token to add to the ContractManager. </param>
    /// <param name="ignorePopup"> Whether the loading popup should be ignored. </param>
    public void AddToken(string tokenAddress)
    {
        AddToken(tokenAddress, true);
    }

    public void AddToken(string tokenAddress, bool showLoadingPopup)
    {
        tokenAddress = tokenAddress.ToLower();

        if (tokens.Contains(tokenAddress))
            return;

        if (showLoadingPopup)
            popupManager.GetPopup<LoadingPopup>();

        ERC20 erc20Token = null;
        erc20Token = new ERC20(tokenAddress, () => AddToken(erc20Token.ContractAddress, erc20Token.Name, erc20Token.Symbol, erc20Token.Decimals.Value));
    }

    public void AddToken(string tokenAddress, string tokenName, string tokenSymbol, int tokenDecimals)
    {
        ERC20 erc20Token = new ERC20(tokenAddress, tokenName, tokenSymbol, tokenDecimals);
        tokens.Add(new TokenInfo(tokenAddress, tokenName, tokenSymbol, tokenDecimals));

        new ERC20TokenAsset(erc20Token, asset => UpdateTradableAssets(asset, () =>
        {
            if (popupManager.ActivePopupType == typeof(LoadingPopup))
                popupManager.CloseActivePopup();
        }), tradableAssetImageManager, userWalletManager);
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
        popup.Text = "Loading wallet";

        Action onLoadFinished = (() => popupManager.CloseAllPopups()) + onTokensLoaded;
        new EtherAsset(asset => UpdateTradableAssets(asset, () => LoadToken(0, onLoadFinished)), tradableAssetImageManager, userWalletManager);
    }

    private void LoadToken(int index, Action onTokenLoadFinished)
    {
        if (CheckLoadStatus(index, onTokenLoadFinished))
            return;

        TokenInfo tokenInfo = tokens[index];
        ERC20 erc20Token = new ERC20(tokenInfo.Address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals);
        new ERC20TokenAsset(erc20Token, asset => UpdateTradableAssets(asset, () => LoadToken(++index, onTokenLoadFinished)), tradableAssetImageManager, userWalletManager);
    }

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