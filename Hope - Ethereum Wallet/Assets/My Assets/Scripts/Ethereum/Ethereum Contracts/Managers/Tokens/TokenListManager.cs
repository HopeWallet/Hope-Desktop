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

    public bool ContainsToken(string address)
    {
        return addableTokens.Contains(address.ToLower());
    }

    public TokenInfo GetToken(string address)
    {
        return !ContainsToken(address = address.ToLower()) ? null : addableTokens[address];
    }

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