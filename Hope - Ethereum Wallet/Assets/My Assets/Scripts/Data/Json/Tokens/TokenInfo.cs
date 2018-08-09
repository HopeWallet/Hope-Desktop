using System;

/// <summary>
/// Class which is used to serialize/deserialize ethereum token data into json format.
/// </summary>
[Serializable]
public sealed class TokenInfo
{
    /// <summary>
    /// The address of the token contract.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// The name of the token.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The symbol of the token.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// The decimal precision of the token.
    /// </summary>
    public int Decimals { get; set; }

    /// <summary>
    /// Initializes the <see cref="TokenInfo"/> by assigning all values.
    /// </summary>
    /// <param name="address"> The contract address of the token. </param>
    /// <param name="name"> The name of the token. </param>
    /// <param name="symbol"> The symbol of the token. </param>
    /// <param name="decimals"> The number of decimal places of the token. </param>
    public TokenInfo(string address, string name, string symbol, int decimals)
    {
        this.Address = address;
        this.Name = name;
        this.Symbol = symbol;
        this.Decimals = decimals;
    }
}