using Hope.Utils.Promises;

/// <summary>
/// Class used for querying data from CoinMarketCap api.
/// </summary>
public sealed class CoinMarketCapApiService : ApiService
{
    /// <summary>
    /// The base api url for CoinMarketCap.
    /// </summary>
    protected override string ApiUrl => "https://api.coinmarketcap.com/v2/";

    /// <summary>
    /// 30 requests allowed per minute.
    /// </summary>
    protected override int MaximumCallsPerMinute => 30;

    /// <summary>
    /// Sends a request for the list of CoinMarketCap listings.
    /// </summary>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendListingRequest() => SendRequest(BuildRequest("listings/"));

    /// <summary>
    /// Sends a request for the ticker info of a certain cryptocurrency.
    /// </summary>
    /// <param name="coinId"> The id of the cryptocurrency. </param>
    /// <param name="currencyType"> The CurrencyType to use as our price base for the ticker info. </param>
    /// <returns> The promise returning the string data from the api. </returns>
    public SimplePromise<string> SendTickerRequest(int coinId, CurrencyManager.CurrencyType currencyType) => SendRequest(BuildRequest("ticker/" + coinId + "/?convert=" + currencyType.ToString()));
}