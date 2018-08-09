using System;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenInfo> addableTokens;

    public TokenListManager(Settings settings)
    {
        addableTokens = new SecurePlayerPrefList<AddableTokenInfo>(settings.tokenListPrefName);
    }

    public void AddToken(string address, string name, string symbol, int decimals, bool enabled, bool listed)
    {
        addableTokens.Add(new AddableTokenInfo(address, name, symbol, decimals, enabled, listed));
    }

    public void UpdateToken(string address, bool enabled, bool listed)
    {
        if (!addableTokens.Contains(address))
            return;

        TokenInfo tokenInfo = addableTokens[address].tokenInfo;
        addableTokens[address] = new AddableTokenInfo(address, tokenInfo.Name, tokenInfo.Symbol, tokenInfo.Decimals, enabled, listed);
    }

    public bool ContainsToken(string address)
    {
        return addableTokens.Contains(address);
    }

    public AddableTokenInfo GetToken(string address)
    {
        return !ContainsToken(address) ? null : addableTokens[address];
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}