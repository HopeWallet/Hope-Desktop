using System;

[Serializable]
public sealed class TokenInfoJson
{
    public string address;
    public string name;
    public string symbol;
    public int decimals;

    public TokenInfoJson(string address, string name, string symbol, int decimals)
    {
        this.address = address;
        this.name = name;
        this.symbol = symbol;
        this.decimals = decimals;
    }
}