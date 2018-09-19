using TMPro;

public sealed partial class OpenWalletMenu
{
    public sealed class PriceManager
    {
        private readonly CurrencyManager currencyManager;
        private readonly TradableAssetPriceManager tradableAssetPriceManager;
        private readonly TradableAssetManager tradableAssetManager;

        private readonly TMP_Text priceText;

        public PriceManager(
            CurrencyManager currencyManager,
            TradableAssetPriceManager tradableAssetPriceManager,
            TradableAssetManager tradableAssetManager,
            TMP_Text priceText)
        {
            this.currencyManager = currencyManager;
            this.tradableAssetPriceManager = tradableAssetPriceManager;
            this.tradableAssetManager = tradableAssetManager;
            this.priceText = priceText;

            TradableAssetManager.OnBalancesUpdated += UpdateAssetValue;

            currencyManager.OnCurrencyChanged += OnCurrencyChanged;
            tradableAssetPriceManager.OnPriceUpdateSucceeded += UpdateAssetValue;
        }

        private void OnCurrencyChanged()
        {
            priceText.text = "__________\n\n";
        }

        private void UpdateAssetValue()
        {
            var tradableAsset = tradableAssetManager.ActiveTradableAsset;
            priceText.gameObject.SetActive(tradableAsset.AssetBalance > 0);

            if (priceText.gameObject.activeInHierarchy)
            {
                decimal netWorth = tradableAssetPriceManager.GetPrice(tradableAsset.AssetSymbol) * tradableAsset.AssetBalance;

                priceText.gameObject.SetActive(netWorth > 0);
                priceText.text = "<size=90%>" + currencyManager.ActiveCurrencySymbol + "</size>" + netWorth.ToString("0.00") + "<style=Symbol> " + currencyManager.ActiveCurrency.ToString() + "</style>";
            }
        }
    }
}