using Hope.Utils.Promises;
using Nethereum.Util;
using System.Linq;

public sealed class DubiExDataManager
{
    private readonly TradableAssetManager tradableAssetManager;

    private const string DUBIEX_HISTORY_API = "https://api.dubiex.com/api/orders/history/query/takeOrder/all/0x0000000000000000000000000000000000000000/";

    public DubiExDataManager(TradableAssetManager tradableAssetManager)
    {
        this.tradableAssetManager = tradableAssetManager;
    }

    public SimplePromise<decimal?> GetRecentEthPrice(string assetSymbol)
    {
        SimplePromise<decimal?> promise = new SimplePromise<decimal?>();

        QueryDubiExData(promise, tradableAssetManager.TradableAssets.Values.First(asset => asset.AssetSymbol == assetSymbol));

        return promise;
    }

    private static void QueryDubiExData(SimplePromise<decimal?> promise, TradableAsset tradableAsset)
    {
        UnityWebUtils.DownloadString(DUBIEX_HISTORY_API + tradableAsset.AssetAddress.ConvertToEthereumChecksumAddress(), jsonData =>
        {
            dynamic deserializedData = JsonUtils.DeserializeDynamic(jsonData);

            if ((int)deserializedData.itemCount == 0)
                promise.Resolve(null);
            else
                promise.Resolve((decimal?)decimal.Parse(deserializedData.result[0].metadata.price));
        });
    }
}