using Hope.Utils.Promises;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Class which manages data retrieved by CoinMarketCap.
/// </summary>
public sealed class CoinMarketCapDataManager
{
    private readonly Dictionary<string, int> CoinIDs = new Dictionary<string, int>();

    private readonly CurrencyManager currencyManager;

    private const string LISTING_API_URL = "https://api.coinmarketcap.com/v2/listings/";
    private const string TICKER_API_URL = "https://api.coinmarketcap.com/v2/ticker/";

    /// <summary>
    /// Initializes the CoinList.
    /// </summary>
    /// <param name="currencyManager"> The active <see cref="CurrencyManager"/>. </param>
    public CoinMarketCapDataManager(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;

        UnityWebUtils.DownloadString(LISTING_API_URL, RetrieveData);
    }

    /// <summary>
    /// Gets the coin id given the symbol.
    /// </summary>
    /// <param name="symbol"> The symbol of the coin to get the id for. </param>
    /// <returns> The coin's id on CoinMarketCap. </returns>
    public int? GetCoinID(string symbol)
    {
        if (!CoinIDs.ContainsKey(symbol))
            return null;

        return CoinIDs[symbol];
    }

    /// <summary>
    /// Gets the price of the coin of a given symbol.
    /// </summary>
    /// <param name="symbol"> The symbol of the coin on CoinMarketCap. </param>
    /// <returns> The promise of an eventual decimal price or null. </returns>
    public SimplePromise<decimal?> GetCoinPrice(string symbol)
    {
        SimplePromise<decimal?> promise = new SimplePromise<decimal?>();

        if (!CoinIDs.ContainsKey(symbol))
            return promise.ResolveResult(null);

        UnityWebUtils.DownloadString(TICKER_API_URL + CoinIDs[symbol] + "/?convert=" + currencyManager.ActiveCurrency.ToString(), jsonData => promise.ResolveResult((decimal?)GetCoinPriceData(JsonUtils.DeserializeDynamic(jsonData).data.quotes).price));

        return promise;
    }

    /// <summary>
    /// Gets the price data from the CoinMarketCap quotes section.
    /// </summary>
    /// <param name="coinQuotes"> The quotes section of the CoinMarketCap crypto coin price. </param>
    /// <returns> The respective price section based on the active currency. </returns>
    private dynamic GetCoinPriceData(dynamic coinQuotes)
    {
        switch (currencyManager.ActiveCurrency)
        {
            case CurrencyManager.CurrencyType.AUD:
                return coinQuotes.AUD;
            case CurrencyManager.CurrencyType.BRL:
                return coinQuotes.BRL;
            case CurrencyManager.CurrencyType.CAD:
                return coinQuotes.CAD;
            case CurrencyManager.CurrencyType.CHF:
                return coinQuotes.CHF;
            case CurrencyManager.CurrencyType.CLP:
                return coinQuotes.CLP;
            case CurrencyManager.CurrencyType.CNY:
                return coinQuotes.CNY;
            case CurrencyManager.CurrencyType.CZK:
                return coinQuotes.CZK;
            case CurrencyManager.CurrencyType.DKK:
                return coinQuotes.DKK;
            case CurrencyManager.CurrencyType.EUR:
                return coinQuotes.EUR;
            case CurrencyManager.CurrencyType.GBP:
                return coinQuotes.GBP;
            case CurrencyManager.CurrencyType.HKD:
                return coinQuotes.HKD;
            case CurrencyManager.CurrencyType.HUF:
                return coinQuotes.HUF;
            case CurrencyManager.CurrencyType.IDR:
                return coinQuotes.IDR;
            case CurrencyManager.CurrencyType.ILS:
                return coinQuotes.ILS;
            case CurrencyManager.CurrencyType.INR:
                return coinQuotes.INR;
            case CurrencyManager.CurrencyType.JPY:
                return coinQuotes.JPY;
            case CurrencyManager.CurrencyType.KRW:
                return coinQuotes.KRW;
            case CurrencyManager.CurrencyType.MXN:
                return coinQuotes.MXN;
            case CurrencyManager.CurrencyType.MYR:
                return coinQuotes.MYR;
            case CurrencyManager.CurrencyType.NOK:
                return coinQuotes.NOK;
            case CurrencyManager.CurrencyType.NZD:
                return coinQuotes.NZD;
            case CurrencyManager.CurrencyType.PHP:
                return coinQuotes.PHP;
            case CurrencyManager.CurrencyType.PKR:
                return coinQuotes.PKR;
            case CurrencyManager.CurrencyType.PLN:
                return coinQuotes.PLN;
            case CurrencyManager.CurrencyType.RUB:
                return coinQuotes.RUB;
            case CurrencyManager.CurrencyType.SEK:
                return coinQuotes.SEK;
            case CurrencyManager.CurrencyType.SGD:
                return coinQuotes.SGD;
            case CurrencyManager.CurrencyType.THB:
                return coinQuotes.THB;
            case CurrencyManager.CurrencyType.TRY:
                return coinQuotes.TRY;
            case CurrencyManager.CurrencyType.TWD:
                return coinQuotes.TWD;
            case CurrencyManager.CurrencyType.USD:
                return coinQuotes.USD;
            case CurrencyManager.CurrencyType.ZAR:
                return coinQuotes.ZAR;
            default:
                return coinQuotes.USD;
        }
    }

    /// <summary>
    /// Retrieves the ids and symbols from the json.
    /// </summary>
    /// <param name="jsonData"> The json string containing all data from CoinMarketCap. </param>
    private async void RetrieveData(string jsonData)
    {
        dynamic deserializedData = JsonUtils.DeserializeDynamic(jsonData);

        await Task.Run(() =>
        {
            foreach (var coin in deserializedData.data)
                if (!CoinIDs.ContainsKey((string)coin.symbol))
                    CoinIDs.Add((string)coin.symbol, (int)coin.id);
        }).ConfigureAwait(false);
    }
}
