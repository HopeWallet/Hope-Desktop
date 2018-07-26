/// <summary>
/// Class which contains certain utility methods for interacting with ethereum api.
/// </summary>
public class EthereumAPI
{

    private const string CONTRACT_ABI = "module=contract&action=getabi&address=";

    private const string TRANSACTION_LIST_START = "module=account&action=txlist&address=";
    private const string INTERNAL_TRANSACTION_LIST_START = "module=account&action=txlistinternal&address=";

    private const string TOKEN_TRANSFER_START = "module=logs&action=getLogs&fromBlock=0&toBlock=latest";
    private const string TOKEN_TRANSFER_TOPIC = "&topic0=0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

    private const string TOKEN_BALANCE_START = "module=account&action=tokenbalance&contractaddress=";

    private readonly string contractApiUrl;
    private readonly string transactionListUrl;
    private readonly string internalTransactionListUrl;
    private readonly string tokenTransfersUrl;
    private readonly string getTokenBalanceUrl;

    /// <summary>
    /// Initializes the EthereumAPI with the api url.
    /// </summary>
    /// <param name="apiUrl"> The url to retrieve info from. </param>
    public EthereumAPI(string apiUrl)
    {
        contractApiUrl = apiUrl + CONTRACT_ABI;
        transactionListUrl = apiUrl + TRANSACTION_LIST_START;
        internalTransactionListUrl = apiUrl + INTERNAL_TRANSACTION_LIST_START;
        tokenTransfersUrl = apiUrl + TOKEN_TRANSFER_START;
        getTokenBalanceUrl = apiUrl + TOKEN_BALANCE_START;
    }

    /// <summary>
    /// Gets the url containing the string data of this contract's abi.
    /// </summary>
    /// <param name="contractAddress"> The contract to get the abi for. </param>
    /// <returns> The url containing this contract's abi. </returns>
    public string GetContractAbiUrl(string contractAddress) => contractApiUrl + contractAddress;

    /// <summary>
    /// Gets the url containing the transaction list of this ethereum address.
    /// </summary>
    /// <param name="address"> The address to get the transaction list for. </param>
    /// <returns> The url containing the transaction list of this ethereum address. </returns>
    public string GetTransactionListUrl(string address) => transactionListUrl + address;

    /// <summary>
    /// Gets the url containing the internal transaction list of this ethereum address.
    /// </summary>
    /// <param name="address"> The address to get the internal transaction list for. </param>
    /// <returns> The url containing the internal transaction list of this ethereum address. </returns>
    public string GetInternalTransactionListUrl(string address) => internalTransactionListUrl + address;

    /// <summary>
    /// Gets the api url for token transfers going to an ethereum address.
    /// </summary>
    /// <param name="walletAddress"> The ethereum address which received the tokens. </param>
    /// <param name="tokenAddress"> The token address being sent. </param>
    /// <returns> The api url for the token transfers sent to an address. </returns>
    public string GetTokenTransfersToWalletUrl(string walletAddress, string tokenAddress)
        => tokenTransfersUrl + "&address=" + tokenAddress + TOKEN_TRANSFER_TOPIC + "&topic2=0x000000000000000000000000" + walletAddress.Remove(0, 2);

    /// <summary>
    /// Gets the api url for token transfers coming from an ethereum address.
    /// </summary>
    /// <param name="walletAddress"> The ethereum address which sent out the token transactions. </param>
    /// <param name="tokenAddress"> The token address being sent. </param>
    /// <returns> The api url for the token transfers sent from an address. </returns>
    public string GetTokenTransfersFromWalletUrl(string walletAddress, string tokenAddress)
    => tokenTransfersUrl + "&address=" + tokenAddress + TOKEN_TRANSFER_TOPIC + "&topic1=0x000000000000000000000000" + walletAddress.Remove(0, 2);

    /// <summary>
    /// Gets the api url for token transfers coming from one address and into another address.
    /// </summary>
    /// <param name="tokenAddress"> The address of the token being sent. </param>
    /// <param name="fromAddress"> The address sending the tokens. </param>
    /// <param name="toAddress"> The address receiving the tokens. </param>
    /// <returns> The api url for the token transfers sent from an address and into another. </returns>
    public string GetTokenTransfersFromAndToUrl(string tokenAddress, string fromAddress, string toAddress)
        => tokenTransfersUrl + "&address=" + tokenAddress + TOKEN_TRANSFER_TOPIC + "&topic1=0x000000000000000000000000" + fromAddress.Remove(0, 2) + "&topic2=0x000000000000000000000000" + toAddress.Remove(0, 2);

    /// <summary>
    /// Gets the api url for retrieving the token balance of a wallet.
    /// </summary>
    /// <param name="tokenAddress"> The token address to get the balance of. </param>
    /// <param name="walletAddress"> The wallet to get the token balance of. </param>
    /// <returns> The api url to use to retrieve the token balance of a wallet address. </returns>
    public string GetTokenBalanceUrl(string tokenAddress, string walletAddress) => getTokenBalanceUrl + tokenAddress + "&address=" + walletAddress;
}