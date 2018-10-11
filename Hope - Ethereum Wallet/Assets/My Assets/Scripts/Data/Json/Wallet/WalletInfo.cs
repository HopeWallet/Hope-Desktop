using Newtonsoft.Json;
using System;

/// <summary>
/// Class which holds the info for certain saved wallets.
/// </summary>
[Serializable]
public sealed class WalletInfo
{
    /// <summary>
    /// Class containing important wallet encrypted info.
    /// </summary>
    [Serializable]
    public class EncryptedDataContainer
    {
        /// <summary>
        /// The encrypted hashes used to encrypt the seed.
        /// </summary>
        public string[] EncryptionHashes { get; set; }

        /// <summary>
        /// The encrypted wallet seed.
        /// </summary>
        public string EncryptedSeed { get; set; }

        /// <summary>
        /// The pbkdf2 password hash of the password used to encrypt the wallet.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Initializes the EncryptedDataContainer.
        /// </summary>
        /// <param name="encryptionHashes"> The encrypted hashes used to encrypt the seed. </param>
        /// <param name="encryptedSeed"> The encrypted wallet seed. </param>
        /// <param name="passwordHash"> The pbkdf2 password hash. </param>
        public EncryptedDataContainer(string[] encryptionHashes, string encryptedSeed, string passwordHash)
        {
            EncryptionHashes = encryptionHashes;
            EncryptedSeed = encryptedSeed;
            PasswordHash = passwordHash;
        }
    }

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
    /// The encrypted wallet data container.
    /// </summary>
    public EncryptedDataContainer EncryptedWalletData { get; set; }

    /// <summary>
    /// Initializes the WalletInfo with the name, number, and addresses of a wallet.
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

    /// <summary>
    /// Initializes the WalletInfo with the name, number, addresses, and encrypted data of a wallet.
    /// </summary>
    /// <param name="encryptedWalletData"> The encrypted data for the wallet. </param>
    /// <param name="walletName"> The name of the wallet. </param>
    /// <param name="walletAddresses"> The addresses of this wallet. </param>
    /// <param name="walletNum"> The number of the wallet. </param>
    [JsonConstructor]
    public WalletInfo(EncryptedDataContainer encryptedWalletData, string walletName, string[][] walletAddresses, int walletNum)
    {
        EncryptedWalletData = encryptedWalletData;
        WalletNum = walletNum;
        WalletAddresses = walletAddresses;
        WalletName = walletName;
    }
}