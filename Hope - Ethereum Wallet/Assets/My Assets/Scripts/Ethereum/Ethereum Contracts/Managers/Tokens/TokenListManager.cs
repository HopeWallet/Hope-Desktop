using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TokenListManager
{
    private SecurePlayerPrefList<TokenInfo> addableTokens;

    public List<TokenInfo> TokenList => addableTokens.ToList();

    public TokenListManager(
        TokenContractManager tokenContractManager,
        PRPS prps,
        DUBI dubi,
        EthereumNetworkManager.Settings ethereumNetworkSettings)
    {
        Initialize(tokenContractManager, prps, dubi, ethereumNetworkSettings);
        ethereumNetworkSettings.OnNetworkChanged += _ => Initialize(tokenContractManager, prps, dubi, ethereumNetworkSettings);
    }

    public void AddToken(string address, string name, string symbol, int decimals)
    {
        if (addableTokens.Contains(address = address.ToLower()))
            return;

        addableTokens.Add(new TokenInfo(address, name, symbol, decimals));
    }

    //public void UpdateToken(string address, bool enabled, bool listed)
    //{
    //    if (!addableTokens.Contains(address = address.ToLower()))
    //        return;

    //    AddableTokenInfo currentToken = addableTokens[address];
    //    TokenInfo tokenInfo = currentToken.TokenInfo;

    //    if (!OldTokenList.Select(token => token.TokenInfo.Address.ToLower()).Contains(address))
    //        OldTokenList.Add(new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, currentToken.Enabled, currentToken.Listed));

    //    addableTokens[address] = new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, enabled, listed);
    //}

    public bool ContainsToken(string address)
    {
        return addableTokens.Contains(address.ToLower());
    }

    public TokenInfo GetToken(string address)
    {
        return !ContainsToken(address = address.ToLower()) ? null : addableTokens[address];
    }

    //private void ScanForNewTokens(TokenContractManager tokenContractManager, UserWalletManager userWalletManager)
    //{
    //    var asset = Resources.Load("Data/tokens") as TextAsset;
    //    var text = asset.text;

    //    var deserializedData = JsonUtils.DeserializeDynamicCollection(text);
    //    var count = (int)deserializedData.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        var data = deserializedData[i];
    //        var address = (string)data.address;

    //        if (addableTokens.Contains(address.ToLower()))
    //            continue;

    //        ContractUtils.QueryContract<ERC20.Queries.BalanceOf, SimpleOutputs.UInt256>((string)data.address, null, userWalletManager.GetWalletAddress()).OnSuccess(balance =>
    //        {
    //            if (balance.Value > 0)
    //            {
    //                ERC20 token = new ERC20(address);
    //                token.OnInitializationSuccessful(() =>
    //                {
    //                    addableTokens.Add(new AddableTokenInfo(address.ToLower(), token.Name, token.Symbol, token.Decimals.Value, true, true));
    //                    tokenContractManager.AddAndUpdateToken(new TokenInfo(address.ToLower(), token.Name, token.Symbol, token.Decimals.Value));
    //                });
    //            }
    //        });
    //    }
    //}

    private void Initialize(TokenContractManager tokenContractManager, PRPS prps, DUBI dubi, EthereumNetworkManager.Settings ethereumNetworkSettings)
    {
        addableTokens = new SecurePlayerPrefList<TokenInfo>(PlayerPrefConstants.CACHED_TOKEN_LIST, (int)ethereumNetworkSettings.networkType);
        InitializeDefaultTokenList(tokenContractManager, prps, dubi, ethereumNetworkSettings);
    }

    private void InitializeDefaultTokenList(TokenContractManager tokenContractManager, PRPS prps, DUBI dubi, EthereumNetworkManager.Settings ethereumNetworkSettings)
    {
        if (addableTokens.Count > 0)
            return;

        addableTokens.Add(new TokenInfo(prps.ContractAddress.ToLower(), "Purpose", "PRPS", 18));
        addableTokens.Add(new TokenInfo(dubi.ContractAddress.ToLower(), "Decentralized Universal Basic Income", "DUBI", 18));
        tokenContractManager.AddToken(new TokenInfo(prps.ContractAddress.ToLower(), "Purpose", "PRPS", 18));
        tokenContractManager.AddToken(new TokenInfo(dubi.ContractAddress.ToLower(), "Decentralized Universal Basic Income", "DUBI", 18));

        if (ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet)
        {
            var defaultTokenList = Resources.Load("Data/tokens") as TextAsset;
            var deserializedData = JsonUtils.DeserializeDynamicCollection(defaultTokenList.text);

            for (int i = 0; i < deserializedData.Count; i++)
            {
                var obj = deserializedData[i];

                var address = (string)obj.address;
                var symbol = (string)obj.symbol;
                var name = (string)obj.name;
                var decimals = (int)obj.decimals;

                var isEnabled = address.EqualsIgnoreCase(prps.ContractAddress) || address.EqualsIgnoreCase(dubi.ContractAddress);

                addableTokens.Add(new TokenInfo(address.ToLower(), name, symbol, decimals));

                if (isEnabled)
                    tokenContractManager.AddToken(new TokenInfo(address.ToLower(), name, symbol, decimals));
            }
        }
    }
}