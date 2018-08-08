using System;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenJson> tokenList;

    public TokenListManager(Settings settings)
    {
        tokenList = new SecurePlayerPrefList<AddableTokenJson>(settings.tokenListPrefName);
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string tokenListPrefName;
    }
}