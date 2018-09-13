using Hope.Utils.Promises;

/// <summary>
/// Class used for querying data from the Etherscan api.
/// </summary>
public sealed class EtherscanApiService : ApiService
{
    private readonly EthereumNetworkManager.Settings networkSettings;

    /// <summary>
    /// The api url based on the current network type.
    /// </summary>
    protected override string ApiUrl => networkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? "https://api.etherscan.io/api?" : "https://api-rinkeby.etherscan.io/api?";

    /// <summary>
    /// 300 Etherscan api calls per minute allowed (5 per second).
    /// </summary>
    protected override int MaximumCallsPerMinute => 300;

    /// <summary>
    /// Initializes the EtherscanApiService by assigning the network settings.
    /// </summary>
    /// <param name="networkSettings"> The ethereum network settings object. </param>
    public EtherscanApiService(EthereumNetworkManager.Settings networkSettings)
    {
        this.networkSettings = networkSettings;
    }

    /// <summary>
    /// Sends a request for an ethereum contract.
    /// </summary>
    /// <param name="contractAddress"> The contract address to get the abi for. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendContractAbiRequest(string contractAddress) => SendRequest(BuildRequest($"module=contract&action=getabi&address={contractAddress}"));

    /// <summary>
    /// Sends a request for the transaction list of an address.
    /// </summary>
    /// <param name="address"> The address to get the transaction list for. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTransactionListRequest(string address) => SendRequest(BuildRequest($"module=account&action=txlist&address={address}"));

    /// <summary>
    /// Sends a request for the internal transaction list of an address.
    /// </summary>
    /// <param name="address"> The address to get the internal transaction list for. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendInternalTransactionListRequest(string address) => SendRequest(BuildRequest($"module=account&action=txlistinternal&address={address}"));

    /// <summary>
    /// Sends a request for the token balance of an address.
    /// </summary>
    /// <param name="address"> The address to get the token balance for. </param>
    /// <param name="tokenAddress"> The address of the ethereum token. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTokenBalanceRequest(string address, string tokenAddress) => SendRequest(BuildRequest($"module=account&action=tokenbalance&contractaddress={tokenAddress}&address={address}"));

    /// <summary>
    /// Sends a request for the token transfers sent to an address.
    /// </summary>
    /// <param name="address"> The address the tokens were sent to. </param>
    /// <param name="tokenAddress"> The address of the ethereum token. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTokenTransfersToAddressRequest(string address, string tokenAddress)
        => SendRequest(BuildRequest($"module=logs&action=getLogs&fromBlock=0&toBlock=latest{tokenAddress}&topic0=0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef&topic2=0x000000000000000000000000{address.Remove(0, 2)}"));

    /// <summary>
    /// Sends a request for the token transfers from an address.
    /// </summary>
    /// <param name="address"> The address that sent the tokens. </param>
    /// <param name="tokenAddress"> The address of the ethereum token. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTokenTransfersFromAddressRequest(string address, string tokenAddress)
        => SendRequest(BuildRequest($"module=logs&action=getLogs&fromBlock=0&toBlock=latest{tokenAddress}&topic0=0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef&topic1=0x000000000000000000000000{address.Remove(0, 2)}"));

    /// <summary>
    /// Sends a request for the token transfers from one address and sent to another.
    /// </summary>
    /// <param name="fromAddress"> The address that sent the tokens. </param>
    /// <param name="toAddress"> The address that received the tokens. </param>
    /// <param name="tokenAddress"> The address of the ethereum token. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTokenTransfersFromAndToAddressRequest(string fromAddress, string toAddress, string tokenAddress)
        => SendRequest(BuildRequest($"module=logs&action=getLogs&fromBlock=0&toBlock=latest{tokenAddress}&topic0=0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef&topic1=0x000000000000000000000000{fromAddress.Remove(0, 2)}&topic2=0x000000000000000000000000{toAddress.Remove(0, 2)}"));
}