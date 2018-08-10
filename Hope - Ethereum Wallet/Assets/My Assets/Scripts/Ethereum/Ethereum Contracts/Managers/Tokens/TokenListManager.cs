using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenInfo> addableTokens;

    public List<AddableTokenInfo> TokenList => addableTokens.ToList();

    public TokenListManager(Settings settings)
    {
        addableTokens = new SecurePlayerPrefList<AddableTokenInfo>(settings.tokenListPrefName);
    }

    public void AddToken(string address, string name, string symbol, int decimals, bool enabled, bool listed)
    {
        addableTokens.Add(new AddableTokenInfo(address.ToLower(), name, symbol, decimals, enabled, listed));
    }

    public void UpdateToken(string address, bool enabled, bool listed)
    {
        if (!addableTokens.Contains(address = address.ToLower()))
            return;

        TokenInfo tokenInfo = addableTokens[address].TokenInfo;
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