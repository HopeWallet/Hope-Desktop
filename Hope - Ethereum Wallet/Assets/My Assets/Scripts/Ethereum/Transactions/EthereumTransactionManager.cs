using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;

/// <summary>
/// Class which manages the loading and updating of ethereum and token transaction data.
/// </summary>
public sealed class EthereumTransactionManager : IPeriodicUpdater, IUpdater
{
    public static event Action OnTransactionsAdded;

    private readonly Queue<AssetToScrape> assetsToScrape = new Queue<AssetToScrape>();
    private readonly Dictionary<string, List<TransactionInfo>> transactionsByAddress = new Dictionary<string, List<TransactionInfo>>();

    private readonly TradableAssetManager tradableAssetManager;
    private readonly UserWalletManager userWalletManager;
    private readonly EtherscanApiService apiService;
    //private readonly EthereumAPI api;

    private bool isScraping;

    /// <summary>
    /// The update interval between each transaction check.
    /// </summary>
    public float UpdateInterval => 10f;

    /// <summary>
    /// Initializes the EthereumTransactionManager by initializing all collections and adding the proper methods to the events needed.
    /// </summary>
    /// <param name="periodicUpdateManager"> Thge PeriodicUpdateManager to use when periodically checking for new transactions. </param>
    /// <param name="updateManager"> The UpdateManager to use when getting the transactions for each asset. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="apiService"> The active EtherscanApiService. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    public EthereumTransactionManager(PeriodicUpdateManager periodicUpdateManager,
        UpdateManager updateManager,
        TradableAssetManager tradableAssetManager,
        UserWalletManager userWalletManager,
        EtherscanApiService apiService)
    {
        TradableAssetManager.OnTradableAssetAdded += AddAssetToScrape;
        TokenContractManager.OnTokensLoaded += () => periodicUpdateManager.AddPeriodicUpdater(this);

        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
        this.apiService = apiService;

        updateManager.AddUpdater(this);
    }

    /// <summary>
    /// Gets the list of transactions of a specific asset address.
    /// </summary>
    /// <param name="address"> The address of the asset to find transactions for. </param>
    /// <returns> The list of transactions. </returns>
    public List<TransactionInfo> GetTransactionListByAddress(string address)
    {
        return transactionsByAddress.ContainsKey(address) ? transactionsByAddress[address] : null;
    }

    /// <summary>
    /// Adds each asset to the list of assets to scrape for a routine check of their transactions.
    /// </summary>
    public void PeriodicUpdate() => tradableAssetManager.TradableAssets.ForEach(asset => AddAssetToScrape(asset.Value));

    /// <summary>
    /// Scrapes the transaction list for each tradable asset sequentially.
    /// </summary>
    public void UpdaterUpdate()
    {
        if (assetsToScrape.Count == 0 || isScraping)
            return;

        isScraping = true;

        var asset1 = assetsToScrape.Dequeue();
        var asset2 = assetsToScrape.Dequeue();

        Observable.WhenAll(ObservableWWW.Get(asset1.Url), ObservableWWW.Get(asset2.Url)).Subscribe(resultData =>
        {
            Observable.Start(() =>
            {
                asset1.ProcessTransactionList(resultData[0], asset1.AssetAddress, asset1.IgnoreReceipt);
                asset2.ProcessTransactionList(resultData[1], asset2.AssetAddress, asset2.IgnoreReceipt);
            }).SubscribeOnMainThread().Subscribe(_ =>
            {
                MainThreadExecutor.QueueAction(() => OnTransactionsAdded?.Invoke());
                isScraping = false;
            });
        });
    }

    /// <summary>
    /// Adds an asset to the list of assets to scrape transactions for.
    /// </summary>
    /// <param name="asset"> The asset to scrape transactions for. </param>
    private void AddAssetToScrape(TradableAsset asset)
    {
        if (asset is EtherAsset)
            QueueEther(userWalletManager.WalletAddress);
        else
            QueueToken(asset.AssetAddress, userWalletManager.WalletAddress);
    }

    /// <summary>
    /// Queue's the ether asset to have its transactions searched for.
    /// </summary>
    /// <param name="walletAddress"> The main wallet address. </param>
    private void QueueEther(string walletAddress)
    {
        //QueueAsset(api.GetInternalTransactionListUrl(walletAddress),
        //           api.GetTransactionListUrl(walletAddress),
        //           EtherAsset.ETHER_ADDRESS,
        //           ProcessEtherTransactionData);
    }

    /// <summary>
    /// Queue's an ethereum token to have its transactions searched for.
    /// </summary>
    /// <param name="assetAddress"> The asset address to search for transactions. </param>
    /// <param name="walletAddress"> The main wallet address. </param>
    private void QueueToken(string assetAddress, string walletAddress)
    {
        //QueueAsset(api.GetTokenTransfersFromWalletUrl(walletAddress, assetAddress),
        //           api.GetTokenTransfersToWalletUrl(walletAddress, assetAddress),
        //           assetAddress,
        //           ProcessTokenTransactionData);
    }

    /// <summary>
    /// Queue's an asset to have its transactions searched for.
    /// </summary>
    /// <param name="url1"> The first url containing transactions for the asset. </param>
    /// <param name="url2"> The second url containing transactions for the asset. </param>
    /// <param name="assetAddress"> The address of the asset. </param>
    /// <param name="processTransactionsAction"> Called after the transaction list has been received, which processes the transactions. </param>
    private void QueueAsset(string url1, string url2, string assetAddress, Action<string, string, bool> processTransactionsAction)
    {
        assetsToScrape.Enqueue(new AssetToScrape(url1, assetAddress, true, processTransactionsAction));
        assetsToScrape.Enqueue(new AssetToScrape(url2, assetAddress, false, processTransactionsAction));
    }

    /// <summary>
    /// Processes the transaction data received for any ethereum token.
    /// </summary>
    /// <param name="transactionData"> The token transaction data received from etherscan. </param>
    /// <param name="assetAddress"> The address of the token. </param>
    /// <param name="ignoreReceipt"> Whether the receipt should be ignored. Useful for ether transaction processing only. </param>
    private void ProcessTokenTransactionData(string transactionData, string assetAddress, bool ignoreReceipt)
    {
        ReadJsonData<TokenTransactionJson>(transactionData,
                                           assetAddress,
                                           _ => true,
                                           info => TransactionSimplifier.CreateTokenTransaction(info, userWalletManager));
    }

    /// <summary>
    /// Processes the transaction data received for ethereum.
    /// </summary>
    /// <param name="transactionData"> The general transaction data received from etherscan. </param>
    /// <param name="assetAddress"> The address of this asset, which is 0x000...  </param>
    /// <param name="ignoreReceipt"> Whether the receipt should be ignored, which is the case for internal transactions. </param>
    private void ProcessEtherTransactionData(string transactionData, string assetAddress, bool ignoreReceipt)
    {
        ReadJsonData<EtherTransactionJson>(transactionData,
                                           assetAddress,
                                           info => info.isError != 1 && (ignoreReceipt || info.txreceipt_status != 0),
                                           info => TransactionSimplifier.CreateEtherTransaction(info, userWalletManager));
    }

    /// <summary>
    /// Reads the json data from the transaction list received.
    /// </summary>
    /// <typeparam name="T"> The type of the transaction json object. Either EtherTransactionJson or TokenTransactionJson. </typeparam>
    /// <param name="transactionList"> The transaction data to process. </param>
    /// <param name="assetAddress"> The address of this asset. </param>
    /// <param name="isValidTransaction"> Func to call to determine if the current transaction is valid or not. </param>
    /// <param name="getTransaction"> Action called to get the TransactionInfo object from the json. </param>
    private void ReadJsonData<T>(string transactionList, string assetAddress, Func<T, bool> isValidTransaction, Func<T, TransactionInfo> getTransaction)
    {
        EtherscanAPIJson<T> transactionsJson = null;

        try
        {
            transactionsJson = JsonUtils.Deserialize<EtherscanAPIJson<T>>(transactionList);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            UnityEngine.Debug.Log(transactionList);
        }

        if (transactionsJson == null)
            return;

        AddTransactions(assetAddress, GetValidTransactions(transactionsJson, isValidTransaction, getTransaction));
    }

    /// <summary>
    /// Adds an array of valid transactions to the dictionary of transactions.
    /// </summary>
    /// <param name="assetAddress"> The asset address of the transactions. </param>
    /// <param name="transactions"> The array of transactions to add. </param>
    private void AddTransactions(string assetAddress, TransactionInfo[] transactions)
    {
        if (!transactionsByAddress.ContainsKey(assetAddress))
            transactionsByAddress.Add(assetAddress, new List<TransactionInfo>());

        if (transactions != null)
        {
            transactions.ForEach(tx =>
            {
                if (!transactionsByAddress[assetAddress].Select(savedTx => savedTx.TxHash).Contains(tx.TxHash))
                    transactionsByAddress[assetAddress].Add(tx);
            });

            transactionsByAddress[assetAddress].Sort((info1, info2) => info1.TimeStamp.CompareTo(info2.TimeStamp));
        }
    }

    /// <summary>
    /// Gets a list of valid transactions.
    /// </summary>
    /// <typeparam name="T"> The type of the transaction json object. Either EtherTransactionJson or TokenTransactionJson. </typeparam>
    /// <param name="transactionsJson"> The json object which holds all the transaction data. </param>
    /// <param name="isValidTransaction"> Func to call to determine if the current transaction is valid or not. </param>
    /// <param name="getTransaction"> Action called to get the TransactionInfo object from the json. </param>
    /// <returns> The array of transactions which are valid based on the Func criteria. </returns>
    private TransactionInfo[] GetValidTransactions<T>(EtherscanAPIJson<T> transactionsJson, Func<T, bool> isValidTransaction, Func<T, TransactionInfo> getTransaction)
    {
        return transactionsJson.result
                       .Where(isValidTransaction)
                       .Select(getTransaction)
                       .Where(tx => tx != null)
                       .ToArray();
    }
}
