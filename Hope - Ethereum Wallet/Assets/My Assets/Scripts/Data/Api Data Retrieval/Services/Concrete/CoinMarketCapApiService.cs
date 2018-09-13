using Hope.Utils.Promises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class CoinMarketCapApiService : ApiService
{
    protected override string ApiUrl => "https://api.coinmarketcap.com/v2/";

    protected override int MaximumCallsPerMinute => 30;

    public SimplePromise<string> SendListingRequest()
    {
        return SendRequest(BuildRequest("listings/"));
    }

    public SimplePromise<string> SendTickerRequest(int coinId, CurrencyManager.CurrencyType currencyType)
    {
        return SendRequest(BuildRequest("ticker/" + coinId + "/?convert=" + currencyType.ToString()));
    }
}