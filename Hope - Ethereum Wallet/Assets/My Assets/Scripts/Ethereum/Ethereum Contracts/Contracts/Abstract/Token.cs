using System;

public abstract class Token : DynamicSmartContract
{
    public string Name { get; private set; }

    public string Symbol { get; private set; }

    public int? Decimals { get; private set; }

    protected Token(string contractAddress, Action onTokenInitialized) : base(contractAddress)
    {
        GetTokenName(name => { Name = string.IsNullOrEmpty(name) ? Symbol : name; CheckInitializationStatus(onTokenInitialized); });
        GetTokenSymbol(symbol => { Symbol = symbol; Name = string.IsNullOrEmpty(Name) ? symbol : Name; CheckInitializationStatus(onTokenInitialized); });
        GetTokenDecimals(decimals => { Decimals = decimals == null ? 0 : (int)decimals; CheckInitializationStatus(onTokenInitialized); });
    }

    protected Token(string contractAddress, string name, string symbol, int decimals) : base(contractAddress)
    {
        Name = name;
        Symbol = symbol;
        Decimals = decimals;
    }

    private void CheckInitializationStatus(Action onTokenInitialied)
    {
        if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Symbol) && Decimals.HasValue)
            onTokenInitialied?.Invoke();
    }

    protected abstract void GetTokenName(Action<string> onTokenNameReceived);

    protected abstract void GetTokenSymbol(Action<string> onTokenSymbolReceived);

    protected abstract void GetTokenDecimals(Action<dynamic> onTokenDecimalsReceived);
}