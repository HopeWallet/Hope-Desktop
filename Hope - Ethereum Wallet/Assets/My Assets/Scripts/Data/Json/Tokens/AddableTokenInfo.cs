using System;

[Serializable]
public sealed class AddableTokenInfo
{
    public TokenInfo TokenInfo { get; set; }

    public bool Enabled { get; set; }

    public bool Listed { get; set; }

    public AddableTokenInfo(
        string address,
        string name,
        string symbol,
        int decimals,
        bool enabled,
        bool listed) : this(new TokenInfo(address, name, symbol, decimals), enabled, listed)
    {
    }

    private AddableTokenInfo(TokenInfo tokenInfo, bool enabled, bool listed)
    {
        TokenInfo = tokenInfo;
        Enabled = enabled;
        Listed = listed;
    }
}