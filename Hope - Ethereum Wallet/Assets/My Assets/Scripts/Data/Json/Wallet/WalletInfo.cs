using System;

/// <summary>
/// Class which holds the info for certain saved wallets.
/// </summary>
[Serializable]
public sealed class WalletInfo
{
	/// <summary>
	/// The number of the wallet.
	/// </summary>
	public int WalletNum { get; set; }

	/// <summary>
	/// The name of the wallet.
	/// </summary>
	public string WalletName { get; set; }

    /// <summary>
    /// The addresses of the wallet.
    /// </summary>
    public string[][] WalletAddresses { get; set; }

    /// <summary>
    /// Initializes the WalletInfo with the name and number of the wallet.
    /// </summary>
    /// <param name="walletName"> The name of the wallet. </param>
    /// <param name="walletAddresses"> The addresses of this wallet. </param>
    /// <param name="walletNum"> The number of the wallet. </param>
    public WalletInfo(string walletName, string[][] walletAddresses, int walletNum)
    {
        WalletNum = walletNum;
        WalletAddresses = walletAddresses;
        WalletName = walletName;
    }
}