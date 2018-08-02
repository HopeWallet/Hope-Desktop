/// <summary>
/// Class which holds the info for certain saved wallets.
/// </summary>
public sealed class WalletInfo
{
	/// <summary>
	/// The number of the wallet.
	/// </summary>
	public int WalletNum { get; }

	/// <summary>
	/// The name of the wallet.
	/// </summary>
	public string WalletName { get; }

	/// <summary>
	/// Initializes the WalletInfo with the name and number of the wallet.
	/// </summary>
	/// <param name="walletName"> The name of the wallet. </param>
	/// <param name="walletNum"> The number of the wallet. </param>
	public WalletInfo(string walletName, int walletNum)
    {
        WalletNum = walletNum;
        WalletName = walletName;
    }

}