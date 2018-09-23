using Hope.Utils.Promises;
using System;
/// <summary>
/// Base class for dynamic ethereum tokens.
/// </summary>
public abstract class Token : DynamicSmartContract
{
    private event Action OnTokenInitializationSuccessful;
    private event Action OnTokenInitializationUnsuccessful;

    private int initializationCounter;
    private bool initializationSuccessful;

    /// <summary>
    /// The name of the ethereum token.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The symbol of the ethereum token.
    /// </summary>
    public string Symbol { get; private set; }

    /// <summary>
    /// The decimal count of the ethereum token.
    /// </summary>
    public int? Decimals { get; private set; }

    /// <summary>
    /// Initializes the ethereum token with the required values.
    /// </summary>
    /// <param name="mainnetAddress"> The mainnet address of the token contract. </param>
    /// <param name="name"> The name of the token. </param>
    /// <param name="symbol"> The symbol of the token. </param>
    /// <param name="decimals"> The decimal count of the token. </param>
    protected Token(string mainnetAddress, string name, string symbol, int decimals) : base(mainnetAddress)
    {
        Name = name;
        Symbol = symbol;
        Decimals = decimals;
    }

    protected Token(string mainnetAddress) : base(mainnetAddress)
    {
        InitializationQuery(QueryName, name => Name = name, _ => Name = null);
        InitializationQuery(QuerySymbol, symbol => Symbol = symbol, _ => Symbol = null);
        InitializationQuery(QueryDecimals, decimals => Decimals = decimals, _ => Decimals = null);
    }

    private void InitializationQuery<T>(Func<EthCallPromise<T>> query, Action<T> onSuccess, Action<string> onError)
    {
        query().OnSuccess(onSuccess).OnError(onError).OnSuccess(_ => CheckInitializationStatus()).OnError(_ => CheckInitializationStatus());
    }

    public void OnInitializationSuccessful(Action onInitializationSuccessful)
    {
        if (initializationCounter == 3 && initializationSuccessful)
            onInitializationSuccessful?.Invoke();
        else
            OnTokenInitializationSuccessful += onInitializationSuccessful;
    }

    public void OnInitializationUnsuccessful(Action onInitializationUnsuccessful)
    {
        if (initializationCounter == 3 && !initializationSuccessful)
            onInitializationUnsuccessful?.Invoke();
        else
            OnTokenInitializationUnsuccessful += onInitializationUnsuccessful;
    }

    private void CheckInitializationStatus()
    {
        if (++initializationCounter == 3)
        {
            if (initializationSuccessful = Decimals != null && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Symbol))
                OnTokenInitializationSuccessful?.Invoke();
            else
                OnTokenInitializationUnsuccessful?.Invoke();
        }
    }

    public abstract EthCallPromise<string> QueryName();

    public abstract EthCallPromise<string> QuerySymbol();

    public abstract EthCallPromise<int?> QueryDecimals();
}