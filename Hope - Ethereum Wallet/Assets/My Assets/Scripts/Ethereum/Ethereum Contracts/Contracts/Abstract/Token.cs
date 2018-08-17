using System;

public abstract class Token : DynamicSmartContract
{
    public string Name { get; }

    public string Symbol { get; }

    public int? Decimals { get; }

    protected Token(string contractAddress, string name, string symbol, int decimals) : base(contractAddress)
    {
        Name = name;
        Symbol = symbol;
        Decimals = decimals;
    }
}