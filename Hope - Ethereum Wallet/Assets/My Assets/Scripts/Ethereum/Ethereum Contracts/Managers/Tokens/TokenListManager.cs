using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenInfo> addableTokens;

    public List<AddableTokenInfo> TokenList => addableTokens.ToList();

    public List<AddableTokenInfo> OldTokenList { get; } = new List<AddableTokenInfo>();

    public TokenListManager(Settings settings)
    {
        addableTokens = new SecurePlayerPrefList<AddableTokenInfo>(settings.tokenListPrefName);
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
            OldTokenList.Add(new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, !enabled, listed));

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

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}