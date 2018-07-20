using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class which contains the list of coins from CoinMarketCap.
/// </summary>
public class CoinList
{
    private readonly Dictionary<string, int> coinIDs = new Dictionary<string, int>();

    // https://api.coinmarketcap.com/v2/listings/

    /// <summary>
    /// Initializes the CoinList.
    /// </summary>
    public CoinList()
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

    /// <summary>
    /// Initializes the CoinList by getting the reference to the json data and starting to retrieve the data for ids and symbols.
    /// </summary>
    private void InitializeCoinList()
    {
        RetrieveData(Resources.Load<TextAsset>("Data/cmcdata").text);
    }

    /// <summary>
    /// Retrieves the ids and symbols from the json.
    /// </summary>
    /// <param name="coinList"> The json string containing all data from CoinMarketCap. </param>
    private async void RetrieveData(string coinList)
    {
        CMCData cmcData = await Task.Run(() => JsonUtils.GetJsonData<CMCData>(coinList)).ConfigureAwait(false);

        await Task.Run(() => cmcData.data.ForEach(coin =>
        {
            if (!coinIDs.ContainsKey(coin.symbol))
                coinIDs.Add(coin.symbol, coin.id);
        })).ConfigureAwait(false);
    }
}
