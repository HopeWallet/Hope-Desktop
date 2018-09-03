using System.Collections.Generic;

public sealed class TradableAssetPriceManager : IPeriodicUpdater
{
    private readonly CoinMarketCapDataManager coinMarketCapDataManager;
    private readonly DubiExDataManager dubiexDataManager;
    private readonly TradableAssetManager tradableAssetManager;

    private Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

    public float UpdateInterval => 300;

    public TradableAssetPriceManager(
        CoinMarketCapDataManager coinMarketCapDataManager,
        DubiExDataManager dubiexDataManager,
        TradableAssetManager tradableAssetManager,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.coinMarketCapDataManager = coinMarketCapDataManager;
        this.dubiexDataManager = dubiexDataManager;
        this.tradableAssetManager = tradableAssetManager;

        TradableAssetManager.OnTradableAssetAdded += UpdatePrice;

        UserWallet.OnWalletLoadSuccessful += () => periodicUpdateManager.AddPeriodicUpdater(this);
    }

    public decimal GetPrice(string assetSymbol)
    {
        return prices.ContainsKey(assetSymbol) ? prices[assetSymbol] : 0;
    }

    public void PeriodicUpdate()
    {
        tradableAssetManager.TradableAssets.Values.ForEach(UpdatePrice);
    }

    private void UpdatePrice(TradableAsset tradableAsset)
    {
        coinMarketCapDataManager.GetCoinPrice(tradableAsset.AssetSymbol).OnSuccess(price =>
        {
            if (!prices.ContainsKey(tradableAsset.AssetSymbol))
                prices.Add(tradableAsset.AssetSymbol, price.Value);
        });
    }
}