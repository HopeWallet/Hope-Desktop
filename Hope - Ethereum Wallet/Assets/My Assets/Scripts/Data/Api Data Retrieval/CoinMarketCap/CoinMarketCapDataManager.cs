using Hope.Utils.Promises;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Class which manages data retrieved by CoinMarketCap.
/// </summary>
public sealed class CoinMarketCapDataManager
{
    private readonly Dictionary<string, int> CoinIDs = new Dictionary<string, int>();

    private const string LISTING_API_URL = "https://api.coinmarketcap.com/v2/listings/";
    private const string TICKER_API_URL = "https://api.coinmarketcap.com/v2/ticker/";

    /// <summary>
    /// Initializes the CoinList.
    /// </summary>
    public CoinMarketCapDataManager()
    {
        InitializeCoinList();
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
        if (!CoinIDs.ContainsKey(symbol))
            return null;

        SimplePromise<decimal?> promise = new SimplePromise<decimal?>();
        UnityWebUtils.DownloadString(TICKER_API_URL + CoinIDs[symbol], jsonData => promise.Resolve((decimal?)JsonUtils.DeserializeDynamic(jsonData).data.quotes.USD.price));

        return promise;
    }

    /// <summary>
    /// Initializes the CoinList by getting the reference to the json data and starting to retrieve the data for ids and symbols.
    /// </summary>
    private void InitializeCoinList()
    {
        UnityWebUtils.DownloadString(LISTING_API_URL, RetrieveData);
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
            {
                string symbol = coin.symbol;
                int id = (int)coin.id;
                if (!CoinIDs.ContainsKey(symbol))
                    CoinIDs.Add(symbol, id);
            }
        }).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => GetCoinPrice("ETH").OnSuccess(price => UnityEngine.Debug.Log(price)));
    }
}
