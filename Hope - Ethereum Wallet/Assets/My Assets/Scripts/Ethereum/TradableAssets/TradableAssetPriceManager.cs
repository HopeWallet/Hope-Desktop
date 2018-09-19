using System;
using System.Collections.Generic;

public sealed class TradableAssetPriceManager : IPeriodicUpdater
{
    public event Action OnPriceUpdateStarted;
    public event Action OnPriceUpdateSucceeded;
    public event Action OnPriceUpdateFailed;

    private readonly CoinMarketCapDataManager coinMarketCapDataManager;
    private readonly DubiExDataManager dubiexDataManager;
    private readonly TradableAssetManager tradableAssetManager;

    private readonly Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

    public float UpdateInterval => 300;

    public TradableAssetPriceManager(
        CoinMarketCapDataManager coinMarketCapDataManager,
        DubiExDataManager dubiexDataManager,
        CurrencyManager currencyManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetButtonManager tradableAssetButtonManager,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.coinMarketCapDataManager = coinMarketCapDataManager;
        this.dubiexDataManager = dubiexDataManager;
        this.tradableAssetManager = tradableAssetManager;

        tradableAssetButtonManager.OnActiveButtonChanged += activeButton => UpdatePrice(activeButton.ButtonInfo);
        currencyManager.OnCurrencyChanged += ClearPrices;

        UserWalletManager.OnWalletLoadSuccessful += () => periodicUpdateManager.AddPeriodicUpdater(this);
    }

    public void ClearPrices()
    {
        prices.Clear();

        UpdatePrice(tradableAssetManager.ActiveTradableAsset);
    }

    public decimal GetPrice(string assetSymbol)
    {
        return prices.ContainsKey(assetSymbol) ? prices[assetSymbol] : 0;
    }

    public void PeriodicUpdate()
    {
        UpdatePrice(tradableAssetManager.ActiveTradableAsset);
    }

    private void UpdatePrice(TradableAsset tradableAsset)
    {
        OnPriceUpdateStarted?.Invoke();

        coinMarketCapDataManager.GetCoinPrice(tradableAsset.AssetSymbol)
                                .OnSuccess(price => OnCoinMarketCapPriceFound(tradableAsset, price))
                                .OnError(_ => OnCoinMarketCapPriceNotFound(tradableAsset));
    }

    private void OnCoinMarketCapPriceFound(TradableAsset tradableAsset, decimal? price)
    {
        if (!prices.ContainsKey(tradableAsset.AssetSymbol))
            prices.Add(tradableAsset.AssetSymbol, price.Value);
        else
            prices[tradableAsset.AssetSymbol] = price.Value;

        OnPriceUpdateSucceeded?.Invoke();
    }

    private void OnCoinMarketCapPriceNotFound(TradableAsset tradableAsset)
    {
        coinMarketCapDataManager.GetCoinPrice("ETH").OnSuccess(ethPrice =>
        {
            if (!prices.ContainsKey("ETH"))
                prices.Add("ETH", ethPrice.Value);
            else
                prices["ETH"] = ethPrice.Value;

            dubiexDataManager.GetRecentEthPrice(tradableAsset.AssetSymbol)
                             .OnSuccess(price => OnDubiExPriceFound(tradableAsset, price))
                             .OnError(_ => OnPriceUpdateFailed?.Invoke());
        });
    }

    private void OnDubiExPriceFound(TradableAsset tradableAsset, decimal? price)
    {
        if (!prices.ContainsKey(tradableAsset.AssetSymbol))
            prices.Add(tradableAsset.AssetSymbol, price.Value * prices["ETH"]);
        else
            prices[tradableAsset.AssetSymbol] = price.Value * prices["ETH"];

        OnPriceUpdateSucceeded?.Invoke();
    }
}