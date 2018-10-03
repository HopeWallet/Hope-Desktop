using System;

/// <summary>
/// Class which represents a token that can be added to the list of active tradable assets.
/// </summary>
[Serializable]
public sealed class AddableTokenInfo
{
    /// <summary>
    /// The TokenInfo object.
    /// </summary>
    public TokenInfo TokenInfo { get; set; }

    /// <summary>
    /// Whether this token is enabled in the TradablelAsset list or not.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Whether this token is listed in the addable token list or not.
    /// </summary>
    public bool Listed { get; set; }

    /// <summary>
    /// Initializes the AddableTokenInfo with all the info of a token.
    /// </summary>
    /// <param name="address"> The token address. </param>
    /// <param name="name"> The token name. </param>
    /// <param name="symbol"> The token symbol. </param>
    /// <param name="decimals"> The token decimals. </param>
    /// <param name="enabled"> Whether the token is enabled in the TradableAsset list. </param>
    /// <param name="listed"> Whether the token is listed in the addable token list. </param>
    public AddableTokenInfo(
        string address,
        string name,
        string symbol,
        int decimals,
        bool enabled,
        bool listed) : this(new TokenInfo(address, name, symbol, decimals), enabled, listed)
    {
    }

    /// <summary>
    /// Initializes the AddableTokenInfo given the TokenInfo.
    /// </summary>
    /// <param name="tokenInfo"> The TokenInfo object containing all required token info. </param>
    /// <param name="enabled"> Whether the token is enabled in the TradableAsset list. </param>
    /// <param name="listed"> Whether the token is listed in the addable token list. </param>
    private AddableTokenInfo(TokenInfo tokenInfo, bool enabled, bool listed)
    {
        TokenInfo = tokenInfo;
        Enabled = enabled;
        Listed = listed;
    }
}