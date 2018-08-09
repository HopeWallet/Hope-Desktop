using System;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenJson> addableTokens;

    public TokenListManager(Settings settings)
    {
        addableTokens = new SecurePlayerPrefList<AddableTokenJson>(settings.tokenListPrefName);
    }

    public void AddToken(string address, string name, string symbol, int decimals, bool enabled, bool listed)
    {
        addableTokens.Add(new AddableTokenJson(address, name, symbol, decimals, enabled, listed));
    }

    public void UpdateToken(string address, bool enabled, bool listed)
    {
        if (!addableTokens.Contains(address))
            return;

        TokenInfoJson tokenInfo = addableTokens[address].tokenInfo;
        addableTokens[address] = new AddableTokenJson(address, tokenInfo.name, tokenInfo.symbol, tokenInfo.decimals, enabled, listed);
    }

    public bool ContainsToken(string address)
    {
        return addableTokens.Contains(address);
    }

    public AddableTokenJson GetToken(string address)
    {
        return !ContainsToken(address) ? null : addableTokens[address];
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}