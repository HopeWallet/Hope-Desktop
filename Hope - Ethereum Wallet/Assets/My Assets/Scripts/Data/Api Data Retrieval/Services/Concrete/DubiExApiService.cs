using Hope.Utils.Promises;
using Nethereum.Util;

/// <summary>
/// Class used for querying data from the DubiEx api.
/// </summary>
public sealed class DubiExApiService : ApiService
{
    /// <summary>
    /// The base api url for DubiEx.
    /// </summary>
    protected override string ApiUrl => "https://api.dubiex.com/api/";

    /// <summary>
    /// 100 requests allowed per minute.
    /// </summary>
    protected override int MaximumCallsPerMinute => 100;

    /// <summary>
    /// Sends a request for the trade history of a specific asset given the address.
    /// </summary>
    /// <param name="assetAddress"> The token address to get the trade history for. </param>
    /// <returns> The promise of the trade history json data. </returns>
    public SimplePromise<string> SendTradeHistoryRequest(string assetAddress)
    {
        return SendRequest(BuildRequest("orders/history/query/takeOrder/all/0x0000000000000000000000000000000000000000/" + assetAddress.ConvertToEthereumChecksumAddress()));
    }
}