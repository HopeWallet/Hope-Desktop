using System.Linq;

/// <summary>
/// Class which manages the restricted addresses which cannot have tokens/eth sent to and cannot have contacts created from.
/// </summary>
public class RestrictedAddressManager
{
    private readonly TokenListManager tokenListManager;

    private readonly string[] constantRestrictedAddresses = new string[]
    {
        "0x0000000000000000000000000000000000000000"
    };

    /// <summary>
    /// Initializes the RestrictedAddressManager.
    /// </summary>
    /// <param name="tokenListManager"> The active TokenListManager. </param>
    public RestrictedAddressManager(TokenListManager tokenListManager)
    {
        this.tokenListManager = tokenListManager;
    }

    /// <summary>
    /// Checks if a given address is restricted.
    /// </summary>
    /// <param name="address"> The address to check. </param>
    /// <returns> True if the address is restricted and cannot be used. </returns>
    public bool IsRestrictedAddress(string address)
    {
        return constantRestrictedAddresses.ContainsIgnoreCase(address = address.ToLower()) || tokenListManager.TokenList.Select(token => token.Address).ContainsIgnoreCase(address);
    }
}
