using System;

[Serializable]
public sealed class AddableTokenJson
{
    public TokenInfoJson tokenInfo;
    public bool enabled;
    public bool listed;

    public AddableTokenJson(TokenInfoJson tokenInfo, bool enabled, bool listed)
    {
        this.tokenInfo = tokenInfo;
        this.enabled = enabled;
        this.listed = listed;
    }

    public AddableTokenJson(
        string address,
        string name,
        string symbol,
        int decimals,
        bool enabled,
        bool listed) : this(new TokenInfoJson(address, name, symbol, decimals), enabled, listed)
    {
    }
}