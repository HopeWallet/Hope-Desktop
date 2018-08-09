using System;

public sealed class TokenListManager
{
    public SecurePlayerPrefList<AddableTokenJson> AddableTokens { get; }

    public TokenListManager(Settings settings)
    {
        AddableTokens = new SecurePlayerPrefList<AddableTokenJson>(settings.tokenListPrefName);
    }

    public void Add(string address, string name, string symbol, int decimals)
    {
        AddableTokens.Add(new AddableTokenJson(address, name, symbol, decimals, false, false));
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}