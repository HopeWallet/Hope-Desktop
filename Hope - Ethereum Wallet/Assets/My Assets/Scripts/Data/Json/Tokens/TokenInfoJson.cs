using System;

/// <summary>
/// Class which is used to serialize/deserialize ethereum token data into json format.
/// </summary>
[Serializable]
public sealed class TokenInfoJson
{
    public string address;
    public string name;
    public string symbol;
    public int decimals;

    /// <summary>
    /// Initializes the <see cref="TokenInfoJson"/> by assigning all values.
    /// </summary>
    /// <param name="address"> The contract address of the token. </param>
    /// <param name="name"> The name of the token. </param>
    /// <param name="symbol"> The symbol of the token. </param>
    /// <param name="decimals"> The number of decimal places of the token. </param>
    public TokenInfoJson(string address, string name, string symbol, int decimals)
    {
        this.address = address;
        this.name = name;
        this.symbol = symbol;
        this.decimals = decimals;
    }
}