using System;

public sealed class TokenListManager
{
    public SecurePlayerPrefList<AddableTokenJson> AddableTokens { get; }

    public TokenListManager(Settings settings)
    {
        AddableTokens = new SecurePlayerPrefList<AddableTokenJson>(settings.tokenListPrefName);
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}