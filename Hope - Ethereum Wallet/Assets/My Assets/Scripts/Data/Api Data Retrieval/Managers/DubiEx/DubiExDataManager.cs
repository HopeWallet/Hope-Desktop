using Hope.Utils.Promises;
using System;
using System.Linq;

public sealed class DubiExDataManager
{
    private readonly DubiExApiService dubiExApiService;
    private readonly TradableAssetManager tradableAssetManager;

    public DubiExDataManager(DubiExApiService dubiExApiService, TradableAssetManager tradableAssetManager)
    {
        this.dubiExApiService = dubiExApiService;
        this.tradableAssetManager = tradableAssetManager;
    }

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

    private void ProcessData(SimplePromise<decimal?> promise, string jsonData)
    {
        dynamic deserializedData = JsonUtils.DeserializeDynamic(jsonData);

        if ((int)deserializedData.itemCount == 0)
            promise.ResolveResult(null);
        else
            promise.ResolveResult((decimal?)decimal.Parse(deserializedData.result[0].metadata.price));
    }
}