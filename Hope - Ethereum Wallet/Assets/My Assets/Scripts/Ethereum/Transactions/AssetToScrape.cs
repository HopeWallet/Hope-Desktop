using Hope.Utils.Promises;
using System;

/// <summary>
/// Class which represents an asset that needs its transactions scraped from the web.
/// </summary>
public class AssetToScrape
{
    /// <summary>
    /// Action to execute which processes the list of transactions found.
    /// </summary>
    public Action<string, string, bool> ProcessTransactionList { get; }

    /// <summary>
    /// The url used to find the transaction list.
    /// </summary>
    public Func<SimplePromise<string>> Query { get; }

    /// <summary>
    /// The address of the asset.
    /// </summary>
    public string AssetAddress { get; }

    /// <summary>
    /// Used in one scenario of the ether transaction scrape. Determines if the json variable tx_receipt should be ignored since it may not exist in all json types.
    /// </summary>
    public bool IgnoreReceipt { get; }

    /// <summary>
    /// Initializes the AssetToScrape.
    /// </summary>
    /// <param name="query"> The url which contains the transaction list to scrape for. </param>
    /// <param name="assetAddress"> The asset's address. </param>
    /// <param name="ignoreReceipt"> Whether the tx_receipt should be ignored for this asset. </param>
    /// <param name="processTxlist"> Action to execute which processes the transaction list. </param>
    public AssetToScrape(Func<SimplePromise<string>> query, string assetAddress, bool ignoreReceipt, Action<string, string, bool> processTxlist)
    {
        Query = query;
        AssetAddress = assetAddress;
        IgnoreReceipt = ignoreReceipt;
        ProcessTransactionList = processTxlist;
    }
}