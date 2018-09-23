using Hope.Utils.Ethereum;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenInfo> addableTokens;

    public List<AddableTokenInfo> TokenList => addableTokens.ToList();

    public List<AddableTokenInfo> OldTokenList { get; } = new List<AddableTokenInfo>();

    public TokenListManager(
        TokenContractManager tokenContractManager,
        PRPS prps,
        DUBI dubi,
        Settings settings,
        EthereumNetworkManager.Settings networkSettings)
    {
        addableTokens = new SecurePlayerPrefList<AddableTokenInfo>(settings.tokenListPrefName, (int)networkSettings.networkType);

        InitializeDefaultTokenList(tokenContractManager, prps, dubi, networkSettings);

        //UserWalletManager.OnWalletLoadSuccessful += () => ScanForNewTokens(userWalletManager);
    }

    public void AddToken(string address, string name, string symbol, int decimals, bool enabled, bool listed)
    {
        addableTokens.Add(new AddableTokenInfo(address.ToLower(), name, symbol, decimals, enabled, listed));
        OldTokenList.Add(new AddableTokenInfo(address.ToLower(), name, symbol, decimals, false, listed));
    }

    public void UpdateToken(string address, bool enabled, bool listed)
    {
        if (!addableTokens.Contains(address = address.ToLower()))
            return;

        AddableTokenInfo currentToken = addableTokens[address];
        TokenInfo tokenInfo = currentToken.TokenInfo;

        if (!OldTokenList.Select(token => token.TokenInfo.Address.ToLower()).Contains(address))
            OldTokenList.Add(new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, currentToken.Enabled, currentToken.Listed));

        addableTokens[address] = new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, enabled, listed);
    }

    public bool ContainsToken(string address)
    {
        return addableTokens.Contains(address.ToLower());
    }

    public AddableTokenInfo GetToken(string address)
    {
        return !ContainsToken(address = address.ToLower()) ? null : addableTokens[address];
    }

    private void ScanForNewTokens(UserWalletManager userWalletManager)
    {
        //var asset = Resources.Load("Data/tokens") as TextAsset;
        //var text = asset.text;

        //var deserializedData = JsonUtils.DeserializeDynamicCollection(text);
        //var count = (int)deserializedData.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    var data = deserializedData[i];
        //    var address = (string)data.address;
        //    ContractUtils.QueryContract<ERC20.Queries.BalanceOf, SimpleOutputs.UInt256>((string)data.address, null, userWalletManager.GetWalletAddress()).OnSuccess(balance =>
        //        {

        //        });
        //}
    }

    private void InitializeDefaultTokenList(TokenContractManager tokenContractManager, PRPS prps, DUBI dubi, EthereumNetworkManager.Settings ethereumNetworkSettings)
    {
        if (addableTokens.Count > 0)
            return;

        if (ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet)
        {
            var defaultTokenList = Resources.Load("Data/default_token_list") as TextAsset;
            var deserializedData = JsonUtils.DeserializeDynamicCollection(defaultTokenList.text);

            for (int i = 0; i < deserializedData.Count; i++)
            {
                var obj = deserializedData[i];

                var address = (string)obj.address;
                var symbol = (string)obj.symbol;
                var name = (string)obj.name;
                var decimals = (int)obj.decimals;

                var isEnabled = address.EqualsIgnoreCase(prps.ContractAddress) || address.EqualsIgnoreCase(dubi.ContractAddress);

                addableTokens.Add(new AddableTokenInfo(address.ToLower(), name, symbol, decimals, isEnabled, true));

                if (isEnabled)
                    tokenContractManager.AddToken(new TokenInfo(address.ToLower(), name, symbol, decimals));
            }
        }
        else
        {
            addableTokens.Add(new AddableTokenInfo(prps.ContractAddress.ToLower(), "Purpose", "PRPS", 18, true, true));
            addableTokens.Add(new AddableTokenInfo(dubi.ContractAddress.ToLower(), "Decentralized Universal Basic Income", "DUBI", 18, true, true));
            tokenContractManager.AddToken(new TokenInfo(prps.ContractAddress.ToLower(), "Purpose", "PRPS", 18));
            tokenContractManager.AddToken(new TokenInfo(dubi.ContractAddress.ToLower(), "Decentralized Universal Basic Income", "DUBI", 18));
        }
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}