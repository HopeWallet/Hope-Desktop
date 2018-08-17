/// <summary>
/// Base class for dynamic ethereum tokens.
/// </summary>
public abstract class Token : DynamicSmartContract
{
    /// <summary>
    /// The name of the ethereum token.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The symbol of the ethereum token.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// The decimal count of the ethereum token.
    /// </summary>
    public int? Decimals { get; }

    /// <summary>
    /// Initializes the ethereum token with the required values.
    /// </summary>
    /// <param name="contractAddress"> The address of the token contract. </param>
    /// <param name="name"> The name of the token. </param>
    /// <param name="symbol"> The symbol of the token. </param>
    /// <param name="decimals"> The decimal count of the token. </param>
    protected Token(string contractAddress, string name, string symbol, int decimals) : base(contractAddress)
    {
        Name = name;
        Symbol = symbol;
        Decimals = decimals;
    }
}