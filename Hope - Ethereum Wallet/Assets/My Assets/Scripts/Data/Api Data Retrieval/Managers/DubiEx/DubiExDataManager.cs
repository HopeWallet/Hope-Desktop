using Hope.Utils.Promises;
using System;
using System.Linq;

/// <summary>
/// Class used for retrieving and managing data retrieved from the DubiEx decentralized exchange.
/// </summary>
public sealed class DubiExDataManager
{
    private readonly DubiExApiService dubiExApiService;
    private readonly TradableAssetManager tradableAssetManager;

    /// <summary>
    /// Initializes the DubiExDataManager.
    /// </summary>
    /// <param name="dubiExApiService"> The active DubiExApiService. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    public DubiExDataManager(DubiExApiService dubiExApiService, TradableAssetManager tradableAssetManager)
    {
        this.dubiExApiService = dubiExApiService;
        this.tradableAssetManager = tradableAssetManager;
    }

    /// <summary>
    /// Gets the recent eth value of an asset traded on DubiEx.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset. </param>
    /// <returns> The most recent eth price of the asset on DubiEx. Returns null if no price was found. </returns>
    public SimplePromise<decimal?> GetRecentEthPrice(string assetSymbol)
    {
        SimplePromise<decimal?> promise = new SimplePromise<decimal?>();

        if (tradableAssetManager.TradableAssets.Values.Select(asset => asset.AssetSymbol).ContainsIgnoreCase(assetSymbol))
        {
            dubiExApiService.SendTradeHistoryRequest(tradableAssetManager.TradableAssets.Values.First(asset => asset.AssetSymbol == assetSymbol).AssetAddress)
                            .OnSuccess(jsonData => ProcessData(promise, jsonData));
        }
        else
        {
            promise.ResolveException(new Exception("Asset symbol not found in TradableAsset collection."));
        }

        return promise;
    }

    /// <summary>
    /// Processes the data retrieved from the trade history request on the DubiEx api.
    /// </summary>
    /// <param name="promise"> The promise returning the resulting eth price. </param>
    /// <param name="jsonData"> The json data returned from the rest api request. </param>
    private void ProcessData(SimplePromise<decimal?> promise, string jsonData)
    {
        dynamic deserializedData = JsonUtils.DeserializeDynamic(jsonData);

        if ((int)deserializedData.itemCount == 0)
            promise.ResolveResult(null);
        else
            promise.ResolveResult((decimal?)decimal.Parse(deserializedData.result[0].metadata.price));
    }
}