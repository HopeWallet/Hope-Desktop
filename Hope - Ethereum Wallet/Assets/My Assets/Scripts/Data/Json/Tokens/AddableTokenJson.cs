using System;

[Serializable]
public sealed class AddableTokenJson
{
    public TokenInfoJson tokenInfo;
    public bool enabled;

    public AddableTokenJson(TokenInfoJson tokenInfo, bool enabled)
    {
        this.tokenInfo = tokenInfo;
        this.enabled = enabled;
    }

    public AddableTokenJson(string address, string name, string symbol, int decimals, bool enabled) : this(new TokenInfoJson(address, name, symbol, decimals), enabled)
    {
    }
}