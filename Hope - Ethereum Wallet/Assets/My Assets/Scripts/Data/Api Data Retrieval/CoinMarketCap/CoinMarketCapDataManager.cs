using Hope.Utils.Promises;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Class which manages data retrieved by CoinMarketCap.
/// </summary>
public sealed class CoinMarketCapDataManager
{
    private readonly Dictionary<string, int> coinIDs = new Dictionary<string, int>();

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
        if (!coinIDs.ContainsKey(symbol))
            return null;

        return coinIDs[symbol];
    }

    public SimplePromise<decimal?> GetCoinPrice(string symbol)
    {
        if (!coinIDs.ContainsKey(symbol))
            return null;

        SimplePromise<decimal?> promise = new SimplePromise<decimal?>();
        UnityWebUtils.DownloadString(TICKER_API_URL + coinIDs[symbol], jsonData => promise.Resolve((decimal?)JsonUtils.DeserializeDynamic(jsonData).data.quotes.USD.price));

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
                if (!coinIDs.ContainsKey(symbol))
                    coinIDs.Add(symbol, id);
            }
        }).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(() => GetCoinPrice("ETH").OnSuccess(price => UnityEngine.Debug.Log(price)));
    }
}
