using System;

public abstract class Token : DynamicSmartContract
{
    public string Name { get; private set; }

    public string Symbol { get; private set; }

    public int? Decimals { get; private set; }

    protected Token(string contractAddress, string name, string symbol, int decimals) : base(contractAddress)
    {
        Name = name;
        Symbol = symbol;
        Decimals = decimals;
    }
}